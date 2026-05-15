/* ============================================
   VAULT GAMIFICATION SYSTEM
   XP/level data, badge definitions,
   sidebar widget renderer, toast system
   ============================================ */
var VAULT_GAMIFICATION = (function () {
  'use strict';

  /* ---------- LEVEL THRESHOLDS ---------- */
  var LEVELS = [
    0, 100, 250, 500, 850, 1300, 1850, 2500, 3300, 4250,
    5350, 6600, 8050, 9700, 11550, 13600, 15900, 18450, 21300, 24450,
    27900, 31700, 35850, 40400, 45350, 50750, 56600, 62950, 69800, 77200,
    85150, 93700, 102850, 112650, 123100, 134250, 146100, 158700, 172100, 186300,
    201350, 217300, 234150, 251950, 270750, 290550, 311400, 333350, 356400, 380600
  ];

  /* ---------- XP VALUES ---------- */
  var XP_VALUES = {
    habit_easy: 10,
    habit_medium: 25,
    habit_hard: 50,
    task_low: 20,
    task_medium: 40,
    task_high: 70,
    task_urgent: 100,
    maintenance_easy: 15,
    maintenance_hard: 30,
    project_complete: 200,
    streak_3: 25,
    streak_7: 75,
    streak_14: 150,
    streak_30: 300,
    template_use: 25
  };

  /* ---------- BADGES ---------- */
  var BADGES = [
    { id: 'first_habit', name: 'First Step', description: 'Complete your first habit', icon: '&#x2B50;', color: 'var(--ac)', unlocked: true, date: '2026-03-01' },
    { id: 'streak_3', name: 'On a Roll', description: '3-day streak on any habit', icon: '&#x1F525;', color: 'var(--wn)', unlocked: true, date: '2026-03-04' },
    { id: 'streak_7', name: 'Week Warrior', description: '7-day streak on any habit', icon: '&#x1F4AA;', color: 'var(--inf)', unlocked: true, date: '2026-03-10' },
    { id: 'task_10', name: 'Task Master', description: 'Complete 10 tasks', icon: '&#x2705;', color: 'var(--ac)', unlocked: true, date: '2026-03-15' },
    { id: 'maintenance_5', name: 'Home Hero', description: 'Complete 5 maintenance items', icon: '&#x1F3E0;', color: 'var(--wn)', unlocked: true, date: '2026-03-20' },
    { id: 'level_5', name: 'Rising Star', description: 'Reach level 5', icon: '&#x1F31F;', color: 'var(--pu)', unlocked: true, date: '2026-03-22' },
    { id: 'all_habits', name: 'Perfect Day', description: 'Complete all habits in one day', icon: '&#x1F3C6;', color: 'var(--ac)', unlocked: true, date: '2026-04-01' },
    { id: 'streak_14', name: 'Fortnight Force', description: '14-day streak on any habit', icon: '&#x26A1;', color: 'var(--inf)', unlocked: false, date: null },
    { id: 'streak_30', name: 'Monthly Master', description: '30-day streak on any habit', icon: '&#x1F48E;', color: 'var(--pu)', unlocked: false, date: null },
    { id: 'task_50', name: 'Productivity Pro', description: 'Complete 50 tasks', icon: '&#x1F680;', color: 'var(--inf)', unlocked: false, date: null },
    { id: 'task_100', name: 'Century Club', description: 'Complete 100 tasks', icon: '&#x1F451;', color: 'var(--wn)', unlocked: false, date: null },
    { id: 'project_1', name: 'Project Pioneer', description: 'Complete your first project', icon: '&#x1F4CB;', color: 'var(--ac)', unlocked: false, date: null },
    { id: 'project_5', name: 'Project Manager', description: 'Complete 5 projects', icon: '&#x1F4BC;', color: 'var(--inf)', unlocked: false, date: null },
    { id: 'level_10', name: 'Double Digits', description: 'Reach level 10', icon: '&#x1F3AF;', color: 'var(--pu)', unlocked: false, date: null },
    { id: 'level_25', name: 'Quarter Century', description: 'Reach level 25', icon: '&#x1F525;', color: 'var(--wn)', unlocked: false, date: null },
    { id: 'maintenance_all', name: 'Zero Overdue', description: 'Have 0 overdue maintenance items', icon: '&#x2728;', color: 'var(--ac)', unlocked: false, date: null },
    { id: 'template_use', name: 'Template Fan', description: 'Use your first template', icon: '&#x1F4DD;', color: 'var(--inf)', unlocked: false, date: null },
    { id: 'xp_5000', name: 'XP Collector', description: 'Earn 5,000 total XP', icon: '&#x1F4B0;', color: 'var(--wn)', unlocked: false, date: null }
  ];

  /* ---------- USER STATE ---------- */
  // Mutable state: populated from /api/gamification/leaderboard and /api/gamification/xp-history.
  // Starts at 0 so an empty/offline render is truthful rather than fabricated.
  var userState = {
    totalXp: 0,
    xpHistory: []
  };

  /* ---------- getLevelInfo ---------- */
  function getLevelInfo() {
    var xp = userState.totalXp;
    var level = 0;
    for (var i = LEVELS.length - 1; i >= 0; i--) {
      if (xp >= LEVELS[i]) {
        level = i + 1;
        break;
      }
    }
    if (level >= LEVELS.length) level = LEVELS.length;
    var currentLevelXp = LEVELS[level - 1] || 0;
    var nextLevelXp = LEVELS[level] || LEVELS[LEVELS.length - 1];
    var progress = nextLevelXp > currentLevelXp
      ? Math.round((xp - currentLevelXp) / (nextLevelXp - currentLevelXp) * 100)
      : 100;
    return {
      level: level,
      totalXp: xp,
      currentLevelXp: currentLevelXp,
      nextLevelXp: nextLevelXp,
      xpInLevel: xp - currentLevelXp,
      xpNeeded: nextLevelXp - currentLevelXp,
      progress: Math.min(progress, 100)
    };
  }

  /* ---------- getUnlockedCount ---------- */
  function getUnlockedCount() {
    return BADGES.filter(function (b) { return b.unlocked; }).length;
  }

  /* ---------- renderSidebarWidget ---------- */
  function renderSidebarWidget(container) {
    if (!container) return;
    var info = getLevelInfo();
    var html = '<div class="gm-sidebar-widget">';
    html += '<div class="gm-sidebar-top">';
    html += '<div class="gm-level-badge sm">' + info.level + '</div>';
    html += '<div class="gm-sidebar-info">';
    html += '<div class="gm-sidebar-level">Level ' + info.level + '</div>';
    html += '<div class="gm-sidebar-xp">' + info.totalXp.toLocaleString() + ' XP &middot; ' + info.progress + '%</div>';
    html += '</div></div>';
    html += '<div class="gm-xp-bar"><div class="gm-xp-fill" style="width:' + info.progress + '%"></div></div>';
    html += '<a href="achievements.html" class="gm-sidebar-link">' + getUnlockedCount() + '/' + BADGES.length + ' badges &rarr;</a>';
    html += '</div>';
    container.innerHTML = html;
  }

  /* ---------- Toast system ---------- */
  function ensureToastContainer() {
    var c = document.getElementById('gm-toast-container');
    if (!c) {
      c = document.createElement('div');
      c.id = 'gm-toast-container';
      c.className = 'gm-toast-container';
      document.body.appendChild(c);
    }
    return c;
  }

  function renderToast(message, xp) {
    var container = ensureToastContainer();
    var toast = document.createElement('div');
    toast.className = 'gm-toast';
    toast.innerHTML = '<div class="gm-toast-xp">+' + xp + ' XP</div><div class="gm-toast-msg">' + message + '</div>';
    container.appendChild(toast);

    userState.totalXp += xp;

    /* Auto-dismiss after 3s */
    setTimeout(function () {
      toast.classList.add('dismissing');
      setTimeout(function () {
        if (toast.parentNode) toast.parentNode.removeChild(toast);
      }, 250);
    }, 3000);

    /* Update sidebar widget if present */
    var widget = document.getElementById('gm-sidebar-container');
    if (widget) renderSidebarWidget(widget);
  }

  /* ---------- Public API ---------- */
  return {
    LEVELS: LEVELS,
    XP_VALUES: XP_VALUES,
    BADGES: BADGES,
    userState: userState,
    getLevelInfo: getLevelInfo,
    getUnlockedCount: getUnlockedCount,
    renderSidebarWidget: renderSidebarWidget,
    renderToast: renderToast
  };
})();

/* Load real XP from the backend leaderboard; falls back to 0 on failure. */
function loadGamificationState() {
  return fetch('/api/gamification/leaderboard?period=all')
    .then(function (r) { return r.ok ? r.json() : null; })
    .then(function (data) {
      if (!data || !Array.isArray(data.users)) return;
      // Default user = José (id 1). Later: look up from UserContext cookie.
      var me = data.users.find(function (u) { return u.userId === 1; }) || data.users[0];
      if (me) {
        VAULT_GAMIFICATION.userState.totalXp = me.totalXp || 0;
      }
      return fetch('/api/gamification/xp-history?days=30');
    })
    .then(function (r) { return r && r.ok ? r.json() : null; })
    .then(function (hist) {
      if (Array.isArray(hist)) VAULT_GAMIFICATION.userState.xpHistory = hist;
    })
    .catch(function () { /* keep defaults on error */ });
}

/* Auto-init sidebar widget on DOMContentLoaded */
(function () {
  function initGamificationWidget() {
    var container = document.getElementById('gm-sidebar-container');
    if (container) {
      // Render immediately with zeros to avoid flash-of-hardcoded content,
      // then re-render after the API populates the user state.
      VAULT_GAMIFICATION.renderSidebarWidget(container);
      loadGamificationState().then(function () {
        VAULT_GAMIFICATION.renderSidebarWidget(container);
      });
    }
  }
  if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', initGamificationWidget);
  } else {
    initGamificationWidget();
  }
})();
