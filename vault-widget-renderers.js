/* ============================================
   VAULT WIDGET RENDERERS
   Extracted render functions for dashboard widgets.
   Registers each with VaultWidgetRegistry.
   ============================================ */
(function () {
  'use strict';

  /* ===== SHARED DATA (demo/mock) ===== */

  var habits = [
    { name: '6hr deep work', color: 'var(--ac)', freq: 'Weekly: 7x', type: 'boolean', done: [1, 2, 3, 5, 6] },
    { name: 'Eat healthy', color: 'var(--inf)', freq: 'Weekly: 7x', type: 'boolean', done: [1, 2, 4, 5, 6] },
    { name: 'Reading', color: 'var(--wn)', freq: 'Weekly: 7x', type: 'boolean', done: [1, 3, 5] },
    { name: 'Workout', color: 'var(--pu)', freq: 'Weekly: 4x', type: 'boolean', done: [2, 5] },
    { name: 'Drink water', color: 'var(--inf)', freq: 'Daily', type: 'quantitative', uom: 'ml', goal: 2000, increment: 250, done: [], progress: { 1: 2000, 2: 1500, 3: 2000, 4: 750, 5: 2000, 6: 500 } },
    { name: 'Study', color: 'var(--wn)', freq: 'Daily', type: 'quantitative', uom: 'hrs', goal: 6, increment: 0.5, done: [], progress: { 1: 6, 2: 4, 3: 5.5, 4: 6, 5: 3, 6: 2 } }
  ];
  var startDay = 3, total = 30, today = 6;

  var tasks = [
    { id: 0, title: 'Notion template', description: 'Create a comprehensive Notion template for productivity tracking and content planning workflow.', column: 'Productivity Vault', columnColor: 'var(--ac)', label: 'Content creation', labelBg: 'var(--infbg)', labelTx: 'var(--inftx)', status: 'Not started', statusBg: 'var(--dnbg)', statusTx: 'var(--dntx)', due: '2026-04-06', dueLabel: 'due today!', priority: 'High', project: 'Notion template' },
    { id: 1, title: 'Content creation', description: 'Plan and draft content pieces for the week including social media posts and blog articles.', column: 'Email', columnColor: 'var(--inf)', label: null, labelBg: null, labelTx: null, status: 'Not started', statusBg: 'var(--dnbg)', statusTx: 'var(--dntx)', due: '2026-04-06', dueLabel: 'due today!', priority: 'Medium', project: 'Content creation' },
    { id: 2, title: 'Learn Framer', description: 'Watch Framer tutorial videos and practice building interactive prototypes and animations.', column: 'Framer 2nd video', columnColor: 'var(--wn)', label: null, labelBg: null, labelTx: null, status: 'Not started', statusBg: 'var(--dnbg)', statusTx: 'var(--dntx)', due: null, dueLabel: null, priority: 'Low', project: 'Learn Framer' }
  ];

  var projects = [
    { id: 0, name: 'Notion template', description: 'Build and publish a complete Notion productivity template for content creators.', totalTasks: 4, incomplete: 4, completed: 0, daysLeft: 7, status: 'In Progress', color: 'var(--inf)' },
    { id: 1, name: 'Content creation', description: 'Weekly content creation pipeline for social media and blog.', totalTasks: 4, incomplete: 4, completed: 0, daysLeft: 7, status: 'In Progress', color: 'var(--ac)' },
    { id: 2, name: 'CapCut + Notion', description: 'Learning video editing with CapCut and advanced Notion workflows.', totalTasks: 0, incomplete: 0, completed: 0, daysLeft: 7, status: 'In Progress', color: 'var(--pu)' },
    { id: 3, name: 'Learn Framer', description: 'Master Framer for interactive web design and prototyping.', totalTasks: 2, incomplete: 3, completed: 0, daysLeft: 60, status: 'In Progress', color: 'var(--wn)' }
  ];

  var calendarEvents = [
    { id: 0, title: 'Notion tutorial', day: 6, tags: [{ text: 'Notion template', bg: 'var(--infbg)', tx: 'var(--inftx)' }, { text: 'Not started', bg: 'var(--dnbg)', tx: 'var(--dntx)' }], action: 'Mark as completed', linkedModule: 'project', linkedId: 0 },
    { id: 1, title: 'Notion tutorial', day: 7, tags: [{ text: 'Notion template', bg: 'var(--infbg)', tx: 'var(--inftx)' }], action: 'Mark as completed', linkedModule: 'project', linkedId: 0 },
    { id: 2, title: 'Learn Framer', day: 7, tags: [{ text: 'Not started', bg: 'var(--dnbg)', tx: 'var(--dntx)' }], action: null, linkedModule: 'project', linkedId: 3 },
    { id: 3, title: 'Framer 2nd video', day: 8, tags: [{ text: 'Learn Framer', bg: 'var(--acbg)', tx: 'var(--actx)' }, { text: 'Not started', bg: 'var(--dnbg)', tx: 'var(--dntx)' }], action: null, linkedModule: 'task', linkedId: 2 },
    { id: 4, title: 'Email', day: 8, tags: [{ text: 'Not started', bg: 'var(--dnbg)', tx: 'var(--dntx)' }], action: null, linkedModule: 'task', linkedId: 1 },
    { id: 5, title: 'Content creation', day: 9, tags: [{ text: 'Not started', bg: 'var(--dnbg)', tx: 'var(--dntx)' }], action: null, linkedModule: 'task', linkedId: 1 },
    { id: 6, title: 'Email', day: 10, tags: [{ text: 'Content creation', bg: 'var(--infbg)', tx: 'var(--inftx)' }], action: 'Mark as completed', linkedModule: 'project', linkedId: 1 },
    { id: 7, title: 'CapCut', day: 11, tags: [], action: 'Mark as completed', linkedModule: 'project', linkedId: 2 },
    { id: 8, title: 'Notion tutorial', day: 12, tags: [{ text: 'Notion template', bg: 'var(--infbg)', tx: 'var(--inftx)' }], action: null, linkedModule: 'project', linkedId: 0 },
    { id: 9, title: 'Content creation', day: 12, tags: [{ text: 'Not started', bg: 'var(--dnbg)', tx: 'var(--dntx)' }], action: null, linkedModule: 'project', linkedId: 1 }
  ];

  /* ===== SHARED HELPERS ===== */

  /* Expose data + helpers for modal controller (still in dashboard inline script) */
  window.VaultWidgetData = {
    habits: habits,
    tasks: tasks,
    projects: projects,
    calendarEvents: calendarEvents,
    startDay: startDay,
    total: total,
    today: today,
    cal: null /* set below after cal() is defined */
  };

  function cal(h) {
    var s = '<div class="mcal"><div class="dh">S</div><div class="dh">M</div><div class="dh">T</div><div class="dh">W</div><div class="dh">T</div><div class="dh">F</div><div class="dh">S</div>';
    for (var i = 0; i < startDay; i++) s += '<div class="d empty"></div>';
    for (var d = 1; d <= total; d++) {
      var c = 'd';
      if (h.type === 'quantitative') {
        var amt = h.progress && h.progress[d] ? h.progress[d] : 0;
        var pct = h.goal > 0 ? amt / h.goal : 0;
        if (pct >= 1) c += ' intensity-3';
        else if (pct >= 0.5) c += ' intensity-2';
        else if (pct > 0) c += ' intensity-1';
      } else {
        if (h.done && h.done.indexOf(d) !== -1) c += ' dn';
      }
      if (d === today) c += ' today';
      s += '<div class="' + c + '">' + d + '</div>';
    }
    return s + '</div>';
  }

  window.VaultWidgetData.cal = cal;

  /* ===== RENDER: STREAK WIDGET ===== */

  function renderStreak(container, config, instanceId) {
    var checkSvg = '<svg viewBox="0 0 24 24" fill="none" stroke="var(--ac)" stroke-width="2"><path d="M22 11.08V12a10 10 0 1 1-5.93-9.14"/><polyline points="22 4 12 14.01 9 11.01"/></svg>';
    var html = '<div class="sec-hdr">' + checkSvg + ' Habit streak &amp; goal tracking';
    html += '<a href="vault_habits.html" class="sec-new" style="text-decoration:none;color:inherit">Manage</a>';
    html += '<a href="vault_habit_new.html" class="sec-new" style="text-decoration:none;color:inherit">+ New</a></div>';
    html += '<div class="hscroll-wrap" data-scroll="streak"><button class="arrow left hidden" data-dir="-1">&lt;</button><div class="hscroll" id="streak-scroll">';

    habits.forEach(function (x) {
      if (x.type === 'quantitative') {
        var todayAmt = x.progress && x.progress[today] ? x.progress[today] : 0;
        var pct = x.goal > 0 ? Math.round(todayAmt / x.goal * 100) : 0;
        var streak = todayAmt >= x.goal ? '1 day' : '0 days';
        html += '<div class="hcard"><div class="hcard-top"><div class="hcard-dot" style="background:' + x.color + '"></div><div class="hcard-name">' + x.name + '</div><span class="type-badge quant">qty</span></div><div class="hcard-freq">' + x.freq + '</div>' + cal(x) + '<div class="hcard-check">' + todayAmt + x.uom + ' / ' + x.goal + x.uom + '</div><div class="hcard-pbar"><div class="hcard-pbar-fill" style="width:' + Math.min(pct, 100) + '%;background:' + x.color + '"></div></div><div class="hcard-stats"><div>Progress: ' + pct + '%</div><div class="hi">Streak: ' + streak + '</div></div></div>';
      } else {
        var ct = x.done.filter(function (d) { return d <= today; }).length;
        var pct = Math.round(ct / total * 100);
        var streak = ct > 0 ? '1 day (new record!)' : '0 days';
        html += '<div class="hcard"><div class="hcard-top"><div class="hcard-dot" style="background:' + x.color + '"></div><div class="hcard-name">' + x.name + '</div><span class="type-badge bool">bool</span></div><div class="hcard-freq">' + x.freq + '</div>' + cal(x) + '<div class="hcard-check">Completed today</div><div class="hcard-pbar"><div class="hcard-pbar-fill" style="width:' + pct + '%;background:' + x.color + '"></div></div><div class="hcard-stats"><div>Progress: ' + pct + '%</div><div class="hi">Streak: ' + streak + '</div></div></div>';
      }
    });
    html += '<a href="vault_habit_new.html" class="sec-newpage-btn" style="flex:1 1 150px;min-width:150px;max-width:185px;align-self:stretch;text-decoration:none;color:inherit">+ New habit</a>';
    html += '</div><button class="arrow right" data-dir="1">&gt;</button></div>';
    container.innerHTML = html;
  }

  /* ===== RENDER: DAILY WIDGET ===== */

  function renderDaily(container, config, instanceId) {
    var checkIcon = '<svg viewBox="0 0 24 24" fill="none" stroke="var(--pu)" stroke-width="2"><path d="M22 11.08V12a10 10 0 1 1-5.93-9.14"/><polyline points="22 4 12 14.01 9 11.01"/></svg>';
    var html = '<div class="sec-hdr">' + checkIcon + ' Daily habit tracking<span class="sec-new">+ New</span></div>';
    html += '<div class="hscroll-wrap" data-scroll="daily"><button class="arrow left hidden" data-dir="-1">&lt;</button><div class="hscroll" id="daily-scroll">';

    var checkSvg = '<svg viewBox="0 0 24 24" fill="none" stroke="#fff" stroke-width="3"><polyline points="20 6 9 17 4 12"/></svg>';
    habits.forEach(function (x, idx) {
      if (x.type === 'quantitative') {
        var todayAmt = x.progress && x.progress[today] ? x.progress[today] : 0;
        var goalMet = todayAmt >= x.goal;
        var cardCls = goalMet ? 'dhcard completed' : 'dhcard';
        html += '<div class="' + cardCls + '" data-habit-idx="' + idx + '">';
        html += '<div class="dhcard-top"><div class="dhcard-name"><div style="width:7px;height:7px;border-radius:50%;background:' + x.color + ';flex-shrink:0"></div>' + x.name + '</div></div>';
        html += '<div class="dhcard-freq">' + x.freq + ' · ' + x.uom + '</div>';
        html += '<div style="font-size:9px;color:var(--tx3);margin-bottom:3px">Today &gt; 6/April/2026</div>';
        html += '<div class="stepper" data-idx="' + idx + '">';
        html += '<button class="stepper-btn" data-action="dec">\u2212</button>';
        html += '<div class="stepper-val"><span class="stepper-cur">' + todayAmt + '</span><span class="uom"> / ' + x.goal + x.uom + '</span></div>';
        html += '<button class="stepper-btn" data-action="inc">+</button>';
        html += '</div>';
        var pct = x.goal > 0 ? Math.min(Math.round(todayAmt / x.goal * 100), 100) : 0;
        html += '<div class="hcard-pbar" style="margin-top:6px"><div class="hcard-pbar-fill" style="width:' + pct + '%;background:' + x.color + '"></div></div>';
        html += cal(x);
        html += '<div class="hcard-stats" style="margin-top:6px"><div class="hi">Goal: ' + (goalMet ? 'Reached!' : pct + '%') + '</div></div>';
        html += '</div>';
      } else {
        var isDone = x.done.indexOf(today) !== -1;
        var dstreak = isDone ? '1 day (new record!)' : '0 days';
        var checkCls = isDone ? 'hcheck done' : 'hcheck';
        var cardCls = isDone ? 'dhcard completed' : 'dhcard';
        html += '<div class="' + cardCls + '" data-habit="' + x.name + '">';
        html += '<div class="dhcard-top"><div class="dhcard-name"><div style="width:7px;height:7px;border-radius:50%;background:' + x.color + ';flex-shrink:0"></div>' + x.name + '</div><div class="' + checkCls + '" data-habit="' + x.name + '">' + checkSvg + '</div></div>';
        html += '<div class="dhcard-freq">' + x.freq + '</div>';
        html += '<div style="font-size:9px;color:var(--tx3);margin-bottom:3px">Today &gt; 6/April/2026</div>';
        html += cal(x);
        html += '<div class="hcard-stats" style="margin-top:6px"><div class="hi">Streak: ' + dstreak + '</div><div>' + (isDone ? 'Completed today' : '') + '</div></div>';
        html += '</div>';
      }
    });
    html += '<a href="vault_habit_new.html" class="sec-newpage-btn" style="flex:1 1 150px;min-width:150px;max-width:185px;align-self:stretch;text-decoration:none;color:inherit">+ New habit</a>';
    html += '</div><button class="arrow right" data-dir="1">&gt;</button></div>';
    container.innerHTML = html;
  }

  /* ===== RENDER: TASKS WIDGET ===== */

  function renderTasks(container, config, instanceId) {
    var icon = '<svg viewBox="0 0 24 24" fill="none" stroke="var(--inf)" stroke-width="2"><rect x="3" y="3" width="18" height="18" rx="2"/><path d="M3 9h18M9 21V9"/></svg>';

    if (typeof TaskStore === 'undefined') {
      container.innerHTML = '<div class="sec-hdr">' + icon + ' Tasks</div><div style="font-size:11px;color:var(--tx3);padding:8px">Loading tasks…</div>';
      return;
    }

    var PMAP = {
      low:    {bg:'var(--acbg)',  tx:'var(--actx)',  label:'Low'},
      medium: {bg:'var(--infbg)', tx:'var(--inftx)', label:'Med'},
      high:   {bg:'var(--wnbg)',  tx:'var(--wntx)',  label:'High'},
      urgent: {bg:'var(--dnbg)',  tx:'var(--dntx)',  label:'Urgent'}
    };

    function taskCard(t, tier) {
      var p = PMAP[t.priority] || PMAP.medium;
      var lc = t.labelColor && t.labelColor.indexOf('var(') === -1 ? t.labelColor : '#378ADD';
      var tierDot = tier === 'overdue' ? 'var(--dn)' : tier === 'today' ? 'var(--wn)' : 'var(--ac)';
      var tierTx  = tier === 'overdue' ? 'var(--dntx)' : tier === 'today' ? 'var(--wntx)' : 'var(--actx)';
      var tierLbl = tier === 'overdue' ? 'Overdue' : tier === 'today' ? 'Today' : 'Upcoming';
      var h = '<a href="vault_task_edit.html?id=' + t.id + '" class="tcol task-dash-card" style="text-decoration:none;color:inherit;cursor:pointer;display:block">';
      h += '<div class="tcol-name"><div class="tdot" style="background:' + tierDot + '"></div><span style="color:' + tierTx + '">' + tierLbl + '</span></div>';
      h += '<div class="titem">';
      h += '<div style="font-weight:600;margin-bottom:4px">' + t.title + '</div>';
      h += '<div style="display:flex;gap:3px;flex-wrap:wrap;margin-bottom:3px">';
      h += '<span class="ev-tag" style="background:' + p.bg + ';color:' + p.tx + '">' + p.label + '</span>';
      if (t.label) h += '<span class="ev-tag" style="background:color-mix(in srgb,' + lc + ' 15%,transparent);color:' + lc + '">' + t.label + '</span>';
      h += '</div>';
      if (t.dueDate) {
        var iso = (new Date()).toISOString().slice(0, 10);
        var dc = t.dueDate < iso ? 'color:var(--dntx)' : t.dueDate === iso ? 'color:var(--wntx)' : 'color:var(--tx3)';
        h += '<div style="font-size:9px;' + dc + ';margin-top:2px">' + t.dueDate + '</div>';
      }
      h += '</div></a>';
      return h;
    }

    function buildSection(tabName) {
      var uid = TaskStore.getCurrentUser();
      var list;
      if (tabName === 'today') {
        var ov = TaskStore.getOverdue(uid).map(function(t){ return {t:t,tier:'overdue'}; });
        var td = TaskStore.getDueToday(uid).map(function(t){ return {t:t,tier:'today'}; });
        list = ov.concat(td);
      } else if (tabName === 'upcoming') {
        list = TaskStore.getUpcoming(uid, 7).map(function(t){ return {t:t,tier:'upcoming'}; });
      } else {
        list = TaskStore.getCompleted(uid).slice(0, 8).map(function(t){ return {t:t,tier:'done'}; });
      }
      var h = '';
      list.forEach(function(item){ h += taskCard(item.t, item.tier); });
      if (!list.length) {
        var msg = tabName === 'today' ? 'No overdue or today tasks' : tabName === 'upcoming' ? 'Nothing due this week' : 'No completed tasks';
        h = '<div class="tcol" style="color:var(--tx3);font-size:11px;padding:12px;text-align:center">' + msg + '</div>';
      }
      h += '<a href="vault_task_new.html" class="sec-newpage-btn tcol-size" style="text-decoration:none">+ New task</a>';
      return h;
    }

    var uid = TaskStore.getCurrentUser();
    var ovCnt  = TaskStore.getOverdue(uid).length;
    var tdCnt  = TaskStore.getDueToday(uid).length;
    var upCnt  = TaskStore.getUpcoming(uid, 7).length;

    var tab1 = 'Today';
    if (ovCnt > 0)      tab1 += ' <span style="background:var(--dn);color:#fff;border-radius:3px;padding:0 4px;font-size:8px">' + ovCnt + ' overdue</span>';
    else if (tdCnt > 0) tab1 += ' (' + tdCnt + ')';

    var html = '<div class="sec-hdr">' + icon + ' Tasks';
    html += '<a href="vault_tasks.html" class="sec-new" style="text-decoration:none;color:inherit">Manage</a>';
    html += '<a href="vault_task_new.html" class="sec-new" style="text-decoration:none;color:inherit">+ New</a></div>';
    html += '<div class="tabs">';
    html += '<span class="tab active" data-ttab="today">'    + tab1 + '</span>';
    html += '<span class="tab" data-ttab="upcoming">Upcoming (' + upCnt + ')</span>';
    html += '<span class="tab" data-ttab="completed">Completed</span>';
    html += '</div>';
    html += '<div class="hscroll-wrap" data-scroll="tasks"><button class="arrow left hidden" data-dir="-1">&lt;</button>';
    html += '<div class="hscroll" id="tasks-scroll">' + buildSection('today') + '</div>';
    html += '<button class="arrow right" data-dir="1">&gt;</button></div>';

    container.innerHTML = html;

    /* Tab switching */
    container.querySelectorAll('[data-ttab]').forEach(function(tab) {
      tab.addEventListener('click', function() {
        container.querySelectorAll('[data-ttab]').forEach(function(t){ t.classList.remove('active'); });
        tab.classList.add('active');
        var scroll = container.querySelector('#tasks-scroll');
        if (scroll) scroll.innerHTML = buildSection(tab.getAttribute('data-ttab'));
      });
    });
  }

  /* ===== RENDER: PROJECTS WIDGET ===== */

  function renderProjects(container, config, instanceId) {
    var icon = '<svg viewBox="0 0 24 24" fill="none" stroke="var(--pu)" stroke-width="2"><path d="M2 3h6a4 4 0 0 1 4 4v14a3 3 0 0 0-3-3H2z"/><path d="M22 3h-6a4 4 0 0 0-4 4v14a3 3 0 0 1 3-3h7z"/></svg>';
    var html = '<div class="sec-hdr">' + icon + ' Project manager<span class="sec-new">+ New</span></div>';
    html += '<div class="ptabs"><span class="ptab active">All projects</span><span class="ptab">Complete</span><span class="ptab">Inbox</span></div>';
    html += '<div class="hscroll-wrap" data-scroll="projects"><button class="arrow left hidden" data-dir="-1">&lt;</button><div class="hscroll" id="projects-scroll">';

    projects.forEach(function (p, i) {
      html += '<div class="pcard" data-proj-idx="' + i + '">';
      html += '<div class="pcard-name">' + p.name + '</div>';
      html += '<div class="pcard-desc">' + p.description + '</div>';
      html += '<div class="pcard-row">Total related tasks: ' + p.totalTasks + '</div>';
      html += '<div class="pcard-row">Total incomplete: ' + p.incomplete + '</div>';
      html += '<div class="pcard-row">Total completed: ' + p.completed + '</div>';
      html += '<div class="pcard-footer">' + p.daysLeft + ' days to go \u00b7 ' + p.status + '</div>';
      html += '</div>';
    });
    html += '<button class="sec-newpage-btn pcard-size">+ New page</button>';
    html += '</div><button class="arrow right" data-dir="1">&gt;</button></div>';
    container.innerHTML = html;
  }

  /* ===== RENDER: CALENDAR WIDGET ===== */

  function renderCalendar(container, config, instanceId) {
    var icon = '<svg viewBox="0 0 24 24" fill="none" stroke="var(--wn)" stroke-width="2"><rect x="3" y="4" width="18" height="18" rx="2"/><line x1="16" y1="2" x2="16" y2="6"/><line x1="8" y1="2" x2="8" y2="6"/><line x1="3" y1="10" x2="21" y2="10"/></svg>';
    var html = '<div class="sec-hdr">' + icon + ' Weekly &amp; monthly calendar</div>';
    html += '<div class="cal-tabs"><span class="cal-tab active">This week</span><span class="cal-tab">This month</span>';
    html += '<span style="margin-left:auto;font-size:10px;color:var(--tx3);cursor:pointer">Open in calendar</span>';
    html += '<span style="font-size:10px;color:var(--tx3);margin-left:8px;cursor:pointer">Today</span></div>';
    html += '<div class="cal-month">April 2026</div>';
    html += '<div class="wkgrid" id="cal-grid">';

    var days = ['Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat', 'Sun'];
    days.forEach(function (d) { html += '<div class="wkh">' + d + '</div>'; });
    for (var dayNum = 6; dayNum <= 12; dayNum++) {
      var isToday = dayNum === today;
      html += '<div class="wkday' + (isToday ? ' today-col' : '') + '">';
      html += '<div class="wkday-num' + (isToday ? ' today-num' : '') + '">' + dayNum + '</div>';
      var dayEvs = calendarEvents.filter(function (e) { return e.day === dayNum; });
      dayEvs.forEach(function (ev) {
        html += '<div class="wk-ev" data-ev-idx="' + ev.id + '">';
        var tTitle = ev.title.length > 12 ? ev.title.substring(0, 12) + '...' : ev.title;
        html += '<div>' + tTitle + '</div>';
        ev.tags.forEach(function (tag) {
          html += '<div class="ev-tag" style="background:' + tag.bg + ';color:' + tag.tx + '">' + tag.text + '</div>';
        });
        if (ev.action) html += '<div style="font-size:8px;color:var(--tx3);margin-top:2px">Mark as com...</div>';
        html += '</div>';
      });
      html += '</div>';
    }
    html += '</div>';
    container.innerHTML = html;
  }

  /* ===== REGISTER ALL WIDGET TYPES ===== */

  if (typeof VaultWidgetRegistry !== 'undefined') {
    VaultWidgetRegistry.registerType('streak', {
      label: 'Habit streak & goals',
      icon: '<svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><path d="M22 11.08V12a10 10 0 1 1-5.93-9.14"/><polyline points="22 4 12 14.01 9 11.01"/></svg>',
      category: 'habits',
      defaultConfig: {},
      render: renderStreak
    });

    VaultWidgetRegistry.registerType('daily', {
      label: 'Daily habit tracking',
      icon: '<svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><path d="M22 11.08V12a10 10 0 1 1-5.93-9.14"/><polyline points="22 4 12 14.01 9 11.01"/></svg>',
      category: 'habits',
      defaultConfig: {},
      render: renderDaily
    });

    VaultWidgetRegistry.registerType('tasks', {
      label: 'Tasks',
      icon: '<svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><rect x="3" y="3" width="18" height="18" rx="2"/><path d="M3 9h18M9 21V9"/></svg>',
      category: 'tasks',
      defaultConfig: {},
      render: renderTasks
    });

    VaultWidgetRegistry.registerType('projects', {
      label: 'Project manager',
      icon: '<svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><path d="M2 3h6a4 4 0 0 1 4 4v14a3 3 0 0 0-3-3H2z"/><path d="M22 3h-6a4 4 0 0 0-4 4v14a3 3 0 0 1 3-3h7z"/></svg>',
      category: 'projects',
      defaultConfig: {},
      render: renderProjects
    });

    VaultWidgetRegistry.registerType('calendar', {
      label: 'Calendar',
      icon: '<svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><rect x="3" y="4" width="18" height="18" rx="2"/><line x1="16" y1="2" x2="16" y2="6"/><line x1="8" y1="2" x2="8" y2="6"/><line x1="3" y1="10" x2="21" y2="10"/></svg>',
      category: 'calendar',
      defaultConfig: {},
      render: renderCalendar
    });
  }

})();
