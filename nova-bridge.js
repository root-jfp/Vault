/* Nova voice bridge — push-to-talk button + WebSocket to Nova (ws://localhost:8080/ws).
 * Uses browser SpeechRecognition for STT and speechSynthesis for TTS.
 * Config overrides: window.NOVA_WS_URL, window.NOVA_URL, localStorage keys 'nova_ws_url' / 'nova_url'.
 */
(function(){
  'use strict';

  var NOVA_URL = window.NOVA_URL || localStorage.getItem('nova_url') || 'http://localhost:8080';
  var NOVA_WS_URL = window.NOVA_WS_URL || localStorage.getItem('nova_ws_url')
    || NOVA_URL.replace(/^http/, 'ws').replace(/\/$/, '') + '/ws';

  // Stable per-browser identity
  function getOrCreate(key, factory){
    var v = localStorage.getItem(key);
    if (!v){ v = factory(); localStorage.setItem(key, v); }
    return v;
  }
  function uuid(){
    return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function(c){
      var r = Math.random()*16|0, v = c==='x' ? r : (r&0x3)|0x8;
      return v.toString(16);
    });
  }
  var DEVICE_ID = getOrCreate('nova_device_id', function(){ return 'vault-web-' + uuid(); });
  var state_convId = getOrCreate('nova_conv_id', uuid);

  // SpeechRecognition (Chromium: webkit-prefixed)
  var SR = window.SpeechRecognition || window.webkitSpeechRecognition;

  var state = {
    ws: null,
    wsReady: false,
    recognition: null,
    listening: false,
    currentRequestId: null,
    ttsQueue: [],
    speaking: false,
  };

  // ── UI ──────────────────────────────────────────────────────────────────────
  function buildUI(){
    var right = document.querySelector('.topbar-right');
    if (!right) return null;
    if (right.querySelector('.nova-btn')) return null; // already injected

    var btn = document.createElement('button');
    btn.className = 'nova-btn';
    btn.type = 'button';
    btn.title = 'Talk to Nova (push-to-talk)';
    btn.innerHTML =
      '<svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">' +
        '<rect x="9" y="3" width="6" height="11" rx="3"/>' +
        '<path d="M5 11a7 7 0 0 0 14 0"/>' +
        '<line x1="12" y1="18" x2="12" y2="22"/>' +
      '</svg>' +
      '<span class="nova-btn-label">Nova</span>';
    // Insert before the cog (grid settings) if present
    var cog = right.querySelector('.topbar-cog');
    if (cog) right.insertBefore(btn, cog); else right.appendChild(btn);

    var panel = document.createElement('div');
    panel.className = 'nova-panel';
    panel.innerHTML =
      '<div class="nova-panel-head">' +
        '<span>Nova</span>' +
        '<button class="nova-panel-close" type="button" aria-label="Close">&times;</button>' +
      '</div>' +
      '<div class="nova-panel-status">Idle</div>' +
      '<div class="nova-panel-user"></div>' +
      '<div class="nova-panel-reply"></div>' +
      '<div class="nova-panel-link">' +
        '<a href="' + NOVA_URL + '" target="_blank" rel="noopener">Open Nova &rarr;</a>' +
        '<span class="nova-panel-hint">Click mic to talk</span>' +
      '</div>';
    document.body.appendChild(panel);

    panel.querySelector('.nova-panel-close').addEventListener('click', function(){
      panel.classList.remove('visible');
    });

    btn.addEventListener('click', function(){
      panel.classList.add('visible');
      if (state.listening) { stopListening(); return; }
      startListening();
    });

    return { btn: btn, panel: panel };
  }

  function setStatus(msg){
    var el = document.querySelector('.nova-panel-status');
    if (el) el.textContent = msg;
  }
  function setUserText(msg){
    var el = document.querySelector('.nova-panel-user');
    if (el) el.textContent = msg;
  }
  function appendReply(txt){
    var el = document.querySelector('.nova-panel-reply');
    if (el) el.textContent += (el.textContent ? ' ' : '') + txt;
  }
  function resetReply(){
    var el = document.querySelector('.nova-panel-reply');
    if (el) el.textContent = '';
  }
  function setBtnState(cls){
    var b = document.querySelector('.nova-btn');
    if (!b) return;
    b.classList.remove('listening','thinking','speaking');
    if (cls) b.classList.add(cls);
  }

  // ── WebSocket ───────────────────────────────────────────────────────────────
  function ensureWS(){
    return new Promise(function(resolve, reject){
      if (state.ws && state.ws.readyState === 1) return resolve(state.ws);
      if (state.ws && state.ws.readyState === 0) {
        // Connection in flight — wait for it rather than opening a second socket.
        var pending = state.ws;
        pending.addEventListener('open', function(){ resolve(pending); }, { once: true });
        pending.addEventListener('error', function(e){ reject(e); }, { once: true });
        return;
      }
      try {
        var ws = new WebSocket(NOVA_WS_URL);
        state.ws = ws;
        ws.addEventListener('open', function(){
          ws.send(JSON.stringify({
            type: 'DEVICE_REGISTER',
            payload: { deviceId: DEVICE_ID, name: 'Vault Web', room: 'browser' }
          }));
          state.wsReady = true;
          resolve(ws);
        });
        ws.addEventListener('message', onWsMessage);
        ws.addEventListener('close', function(){
          state.wsReady = false; state.ws = null;
        });
        ws.addEventListener('error', function(e){
          state.wsReady = false;
          reject(e);
        });
      } catch (e) { reject(e); }
    });
  }

  function onWsMessage(ev){
    var msg;
    try { msg = JSON.parse(ev.data); } catch(e){ return; }
    if (!msg || !msg.type) return;

    if (msg.type === 'LLM_SENTENCE' && msg.payload) {
      if (msg.payload.requestId && state.currentRequestId &&
          msg.payload.requestId !== state.currentRequestId) return;
      if (msg.payload.isStatus) { setStatus(msg.payload.text); return; }
      appendReply(msg.payload.text);
      speak(msg.payload.text);
      setBtnState('speaking');
      setStatus('Nova is speaking…');
    } else if (msg.type === 'LLM_DONE') {
      if (msg.payload && msg.payload.conversationId) {
        state_convId = msg.payload.conversationId;
        localStorage.setItem('nova_conv_id', state_convId);
      }
      setStatus('Done');
      state.currentRequestId = null;
      // Keep 'speaking' until TTS queue drains; see speak() below.
      if (!state.speaking && state.ttsQueue.length === 0) setBtnState(null);
    } else if (msg.type === 'LLM_IGNORED') {
      setStatus('Ignored');
      state.currentRequestId = null;
      setBtnState(null);
    }
  }

  function sendChat(text){
    var requestId = uuid();
    state.currentRequestId = requestId;
    setUserText(text);
    resetReply();
    setStatus('Thinking…');
    setBtnState('thinking');
    ensureWS().then(function(ws){
      ws.send(JSON.stringify({
        type: 'CHAT_REQUEST',
        payload: {
          message: text,
          conversationId: state_convId,
          deviceId: DEVICE_ID,
          requestId: requestId,
        }
      }));
    }).catch(function(){
      setStatus('Cannot reach Nova at ' + NOVA_WS_URL);
      setBtnState(null);
    });
  }

  // ── STT ─────────────────────────────────────────────────────────────────────
  function startListening(){
    if (!SR) {
      setStatus('SpeechRecognition unsupported — use Chrome/Edge');
      return;
    }
    var rec = new SR();
    rec.lang = (navigator.language || 'en-US');
    rec.interimResults = true;
    rec.maxAlternatives = 1;
    rec.continuous = false;
    state.recognition = rec;

    var finalText = '';
    rec.onresult = function(ev){
      var interim = '';
      for (var i = ev.resultIndex; i < ev.results.length; i++) {
        var r = ev.results[i];
        if (r.isFinal) finalText += r[0].transcript;
        else interim += r[0].transcript;
      }
      setUserText(finalText + interim);
    };
    rec.onerror = function(ev){
      setStatus('Mic error: ' + ev.error);
      setBtnState(null);
      state.listening = false;
    };
    rec.onend = function(){
      state.listening = false;
      var text = finalText.trim();
      if (text) sendChat(text);
      else { setStatus('No speech detected'); setBtnState(null); }
    };

    try {
      rec.start();
      state.listening = true;
      setBtnState('listening');
      setStatus('Listening…');
    } catch(e){
      setStatus('Mic start failed: ' + (e && e.message || e));
      setBtnState(null);
    }
  }

  function stopListening(){
    if (state.recognition) try { state.recognition.stop(); } catch(e){}
  }

  // ── TTS ─────────────────────────────────────────────────────────────────────
  // Prefer Nova's /api/tts (Azure-backed) over browser Web Speech.
  // Falls back to speechSynthesis if the fetch fails or returns 503.
  function speak(text){
    state.ttsQueue.push(text);
    drainTTS();
  }
  function drainTTS(){
    if (state.speaking) return;
    var next = state.ttsQueue.shift();
    if (!next) {
      if (!state.currentRequestId) setBtnState(null);
      return;
    }
    state.speaking = true;
    speakViaNova(next).catch(function(){
      return speakViaWebSpeech(next);
    }).then(function(){
      state.speaking = false;
      drainTTS();
    });
  }

  function speakViaNova(text){
    return fetch(NOVA_URL.replace(/\/$/, '') + '/api/tts', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ text: text }),
    }).then(function(res){
      if (!res.ok) throw new Error('TTS HTTP ' + res.status);
      return res.blob();
    }).then(function(blob){
      return new Promise(function(resolve){
        var url = URL.createObjectURL(blob);
        var audio = new Audio(url);
        audio.onended = function(){ URL.revokeObjectURL(url); resolve(); };
        audio.onerror = function(){ URL.revokeObjectURL(url); resolve(); };
        audio.play().catch(function(){ URL.revokeObjectURL(url); resolve(); });
      });
    });
  }

  function speakViaWebSpeech(text){
    return new Promise(function(resolve){
      if (!('speechSynthesis' in window)) return resolve();
      var u = new SpeechSynthesisUtterance(text);
      u.rate = 1.0; u.pitch = 1.0;
      u.onend = resolve; u.onerror = resolve;
      try { window.speechSynthesis.speak(u); } catch(e){ resolve(); }
    });
  }

  // ── Init ────────────────────────────────────────────────────────────────────
  function init(){
    if (!document.querySelector('.topbar-right')) return;
    buildUI();
  }
  if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', init);
  } else { init(); }

  // Expose a minimal read-only helper. No `send` — any script on the page could
  // inject arbitrary chat messages through it, and CSP allows 'unsafe-inline'.
  window.Nova = Object.freeze({
    url: NOVA_URL,
    wsUrl: NOVA_WS_URL,
    start: startListening,
    stop: stopListening,
  });
})();
