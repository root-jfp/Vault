/* ============================================
   VAULT WIDGET RENDERERS — DASHBOARD
   Weather, Quick Actions, Daily Agenda,
   Streak & Gamification widgets.
   ============================================ */
(function () {
  'use strict';

  /* ===== SAMPLE DATA ===== */

  var weatherData = {
    city: 'Lisbon',
    temp: 18,
    condition: 'Partly cloudy',
    humidity: 62,
    wind: 14,
    uv: 5,
    forecast: [
      { day: 'Tue', temp: 20, low: 13, condition: 'sunny' },
      { day: 'Wed', temp: 17, low: 12, condition: 'cloudy' },
      { day: 'Thu', temp: 19, low: 14, condition: 'partly-cloudy' }
    ]
  };

  var quickActions = [
    { label: 'New task', icon: '<svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><line x1="12" y1="5" x2="12" y2="19"/><line x1="5" y1="12" x2="19" y2="12"/></svg>', color: 'var(--inf)' },
    { label: 'Add note', icon: '<svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><path d="M11 4H4a2 2 0 0 0-2 2v14a2 2 0 0 0 2 2h14a2 2 0 0 0 2-2v-7"/><path d="M18.5 2.5a2.121 2.121 0 0 1 3 3L12 15l-4 1 1-4 9.5-9.5z"/></svg>', color: 'var(--pu)' },
    { label: 'Log water', icon: '<svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><path d="M12 2.69l5.66 5.66a8 8 0 1 1-11.31 0z"/></svg>', color: 'var(--inf)' },
    { label: 'Log meal', icon: '<svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><path d="M18 8h1a4 4 0 0 1 0 8h-1"/><path d="M2 8h16v9a4 4 0 0 1-4 4H6a4 4 0 0 1-4-4V8z"/><line x1="6" y1="1" x2="6" y2="4"/><line x1="10" y1="1" x2="10" y2="4"/><line x1="14" y1="1" x2="14" y2="4"/></svg>', color: 'var(--wn)' },
    { label: 'Quick habit', icon: '<svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><path d="M22 11.08V12a10 10 0 1 1-5.93-9.14"/><polyline points="22 4 12 14.01 9 11.01"/></svg>', color: 'var(--ac)' },
    { label: 'Timer', icon: '<svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><circle cx="12" cy="12" r="10"/><polyline points="12 6 12 12 16 14"/></svg>', color: 'var(--tx3)' }
  ];

  var agendaSlots = [
    { time: '06:00', title: 'Morning routine', type: 'habit', color: 'var(--ac)', meta: 'Drink water, stretch' },
    { time: '07:00', title: '6hr deep work', type: 'habit', color: 'var(--ac)', meta: 'Block 1 of 3' },
    { time: '09:00', title: 'Review invoice module', type: 'task', color: 'var(--inf)', meta: 'High priority' },
    { time: '10:30', title: 'Study ASP.NET middleware', type: 'task', color: 'var(--inf)', meta: 'Productivity Vault' },
    { time: '12:00', title: 'Lunch break', type: 'habit', color: 'var(--wn)', meta: 'Eat healthy' },
    { time: '14:00', title: 'Check smoke detectors', type: 'maintenance', color: 'var(--dn)', meta: 'Overdue by 3 days' },
    { time: '16:00', title: 'FERRO workout', type: 'habit', color: 'var(--pu)', meta: '4x/week target' },
    { time: '20:00', title: 'Reading', type: 'habit', color: 'var(--wn)', meta: '30 min minimum' }
  ];

  var gamifyData = {
    jose: { name: 'Jose', score: 285, habits: 142, tasks: 89, maintenance: 54 },
    wife: { name: 'Maria', score: 271, habits: 138, tasks: 82, maintenance: 51 },
    streaks: [
      { name: 'Deep work', days: 12, module: 'habits' },
      { name: 'All tasks done', days: 5, module: 'tasks' },
      { name: 'Maintenance on time', days: 8, module: 'maintenance' }
    ]
  };

  /* ===== WEATHER ICONS ===== */
  var weatherIcons = {
    sunny: '<svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><circle cx="12" cy="12" r="5"/><line x1="12" y1="1" x2="12" y2="3"/><line x1="12" y1="21" x2="12" y2="23"/><line x1="4.22" y1="4.22" x2="5.64" y2="5.64"/><line x1="18.36" y1="18.36" x2="19.78" y2="19.78"/><line x1="1" y1="12" x2="3" y2="12"/><line x1="21" y1="12" x2="23" y2="12"/><line x1="4.22" y1="19.78" x2="5.64" y2="18.36"/><line x1="18.36" y1="5.64" x2="19.78" y2="4.22"/></svg>',
    cloudy: '<svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><path d="M18 10h-1.26A8 8 0 1 0 9 20h9a5 5 0 0 0 0-10z"/></svg>',
    'partly-cloudy': '<svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><path d="M18 10h-1.26A8 8 0 1 0 9 20h9a5 5 0 0 0 0-10z"/><circle cx="18" cy="6" r="3" stroke-dasharray="4 2"/></svg>'
  };

  /* ===== RENDER: WEATHER ===== */

  function renderWeather(container) {
    var w = weatherData;
    var mainIcon = weatherIcons['partly-cloudy'];
    var html = '<div class="sec-hdr"><svg viewBox="0 0 24 24" fill="none" stroke="var(--inf)" stroke-width="2"><path d="M18 10h-1.26A8 8 0 1 0 9 20h9a5 5 0 0 0 0-10z"/></svg> Weather</div>';

    html += '<div class="weather-main">';
    html += '<div class="weather-icon">' + mainIcon + '</div>';
    html += '<div><div class="weather-temp">' + w.temp + '\u00b0C</div>';
    html += '<div class="weather-cond">' + w.condition + '</div>';
    html += '<div class="weather-loc">' + w.city + ', Portugal</div></div>';
    html += '</div>';

    html += '<div class="weather-details">';
    html += '<div class="weather-detail"><svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><path d="M12 2.69l5.66 5.66a8 8 0 1 1-11.31 0z"/></svg> ' + w.humidity + '%</div>';
    html += '<div class="weather-detail"><svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><path d="M9.59 4.59A2 2 0 1 1 11 8H2m10.59 11.41A2 2 0 1 0 14 16H2m15.73-8.27A2.5 2.5 0 1 1 19.5 12H2"/></svg> ' + w.wind + ' km/h</div>';
    html += '<div class="weather-detail"><svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><circle cx="12" cy="12" r="5"/><line x1="12" y1="1" x2="12" y2="3"/><line x1="12" y1="21" x2="12" y2="23"/></svg> UV ' + w.uv + '</div>';
    html += '</div>';

    html += '<div class="weather-forecast">';
    w.forecast.forEach(function (d) {
      var icon = weatherIcons[d.condition] || weatherIcons.cloudy;
      html += '<div class="weather-day">';
      html += '<div class="weather-day-name">' + d.day + '</div>';
      html += '<div class="weather-day-icon">' + icon + '</div>';
      html += '<div class="weather-day-temp">' + d.temp + '\u00b0</div>';
      html += '<div class="weather-day-low">' + d.low + '\u00b0</div>';
      html += '</div>';
    });
    html += '</div>';

    container.innerHTML = html;
  }

  /* ===== RENDER: QUICK ACTIONS ===== */

  function renderQuickActions(container) {
    var html = '<div class="sec-hdr"><svg viewBox="0 0 24 24" fill="none" stroke="var(--ac)" stroke-width="2"><polygon points="13 2 3 14 12 14 11 22 21 10 12 10 13 2"/></svg> Quick actions</div>';
    html += '<div class="quick-actions-strip">';
    quickActions.forEach(function (a) {
      html += '<div class="qa-action" style="color:' + a.color + '">' + a.icon + ' <span style="color:var(--tx)">' + a.label + '</span></div>';
    });
    html += '</div>';
    container.innerHTML = html;
  }

  /* ===== RENDER: DAILY AGENDA ===== */

  function renderDailyAgenda(container) {
    var typeColors = {
      habit: { bg: 'var(--acbg)', tx: 'var(--actx)' },
      task: { bg: 'var(--infbg)', tx: 'var(--inftx)' },
      maintenance: { bg: 'var(--dnbg)', tx: 'var(--dntx)' }
    };

    var html = '<div class="sec-hdr"><svg viewBox="0 0 24 24" fill="none" stroke="var(--wn)" stroke-width="2"><rect x="3" y="4" width="18" height="18" rx="2"/><line x1="16" y1="2" x2="16" y2="6"/><line x1="8" y1="2" x2="8" y2="6"/><line x1="3" y1="10" x2="21" y2="10"/></svg> Daily agenda<span class="sec-new" style="font-size:9px">April 7, 2026</span></div>';
    html += '<div class="agenda-timeline">';

    agendaSlots.forEach(function (slot) {
      var tc = typeColors[slot.type] || typeColors.task;
      html += '<div class="agenda-slot">';
      html += '<div class="agenda-time">' + slot.time + '</div>';
      html += '<div class="agenda-dot" style="background:' + slot.color + '"></div>';
      html += '<div class="agenda-content">';
      html += '<div class="agenda-title">' + slot.title + '</div>';
      html += '<div class="agenda-meta">';
      html += '<span class="agenda-type-badge" style="background:' + tc.bg + ';color:' + tc.tx + '">' + slot.type + '</span> ';
      html += slot.meta;
      html += '</div></div></div>';
    });

    html += '</div>';
    container.innerHTML = html;
  }

  /* ===== RENDER: STREAK & GAMIFICATION ===== */

  function renderStreakGamification(container) {
    var g = gamifyData;
    var leading = g.jose.score >= g.wife.score ? 'jose' : 'wife';

    var html = '<div class="sec-hdr"><svg viewBox="0 0 24 24" fill="none" stroke="var(--wn)" stroke-width="2"><path d="M12 15l-3 3h6l-3-3z"/><circle cx="12" cy="8" r="4"/><path d="M5.52 19h12.96"/></svg> Streak &amp; gamification</div>';

    html += '<div class="gamify-competition">';
    html += '<div class="gamify-player' + (leading === 'jose' ? ' leading' : '') + '">';
    html += '<div class="gamify-player-name">' + g.jose.name + '</div>';
    html += '<div class="gamify-player-score">' + g.jose.score + '</div>';
    html += '<div class="gamify-player-label">weekly pts</div></div>';
    html += '<div class="gamify-vs">VS</div>';
    html += '<div class="gamify-player' + (leading === 'wife' ? ' leading' : '') + '">';
    html += '<div class="gamify-player-name">' + g.wife.name + '</div>';
    html += '<div class="gamify-player-score">' + g.wife.score + '</div>';
    html += '<div class="gamify-player-label">weekly pts</div></div>';
    html += '</div>';

    html += '<div class="gamify-breakdown">';
    html += '<div class="gamify-stat"><div class="gamify-stat-label">Habits</div><div class="gamify-stat-val">' + g.jose.habits + ' vs ' + g.wife.habits + '</div></div>';
    html += '<div class="gamify-stat"><div class="gamify-stat-label">Tasks</div><div class="gamify-stat-val">' + g.jose.tasks + ' vs ' + g.wife.tasks + '</div></div>';
    html += '<div class="gamify-stat"><div class="gamify-stat-label">Maint.</div><div class="gamify-stat-val">' + g.jose.maintenance + ' vs ' + g.wife.maintenance + '</div></div>';
    html += '</div>';

    html += '<div class="gamify-streaks">';
    g.streaks.forEach(function (s) {
      html += '<div class="gamify-streak">';
      html += '<div class="gamify-streak-name">' + s.name + '</div>';
      html += '<div class="gamify-streak-val">' + s.days + ' <span class="gamify-streak-unit">days</span></div>';
      html += '</div>';
    });
    html += '</div>';

    container.innerHTML = html;
  }

  /* ===== REGISTER ===== */

  if (typeof VaultWidgetRegistry !== 'undefined') {
    VaultWidgetRegistry.registerType('weather', {
      label: 'Weather',
      icon: '<svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><path d="M18 10h-1.26A8 8 0 1 0 9 20h9a5 5 0 0 0 0-10z"/></svg>',
      category: 'dashboard',
      defaultConfig: {},
      render: renderWeather
    });

    VaultWidgetRegistry.registerType('quick-actions', {
      label: 'Quick actions',
      icon: '<svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><polygon points="13 2 3 14 12 14 11 22 21 10 12 10 13 2"/></svg>',
      category: 'dashboard',
      defaultConfig: {},
      render: renderQuickActions
    });

    VaultWidgetRegistry.registerType('daily-agenda', {
      label: 'Daily agenda',
      icon: '<svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><rect x="3" y="4" width="18" height="18" rx="2"/><line x1="16" y1="2" x2="16" y2="6"/><line x1="8" y1="2" x2="8" y2="6"/><line x1="3" y1="10" x2="21" y2="10"/></svg>',
      category: 'dashboard',
      defaultConfig: {},
      render: renderDailyAgenda
    });

    VaultWidgetRegistry.registerType('streak-gamification', {
      label: 'Streak & gamification',
      icon: '<svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><path d="M12 15l-3 3h6l-3-3z"/><circle cx="12" cy="8" r="4"/><path d="M5.52 19h12.96"/></svg>',
      category: 'dashboard',
      defaultConfig: {},
      render: renderStreakGamification
    });
  }

})();
