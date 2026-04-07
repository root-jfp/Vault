/* ============================================
   VAULT MOBILE
   Bottom nav active state, more sheet toggle
   ============================================ */
(function () {
  'use strict';

  /* ---------- Auto-detect current page ---------- */
  var path = window.location.pathname.split('/').pop() || '';
  var PAGE_MAP = {
    'vault_dashboard_v5_wide_scroll.html': 'home',
    'vault_habits.html': 'habits',
    'vault_habit_new.html': 'habits',
    'vault_habit_edit.html': 'habits',
    'vault_tasks.html': 'tasks',
    'vault_task_new.html': 'tasks',
    'vault_maintenance.html': 'maint',
    'vault_projects.html': 'more',
    'vault_calendar.html': 'more',
    'vault_performance.html': 'more',
    'vault_templates.html': 'more',
    'vault_achievements.html': 'more'
  };
  var currentNav = PAGE_MAP[path] || 'home';

  /* ---------- Set active bottom nav item ---------- */
  function setActiveNav() {
    var items = document.querySelectorAll('.bnav-item');
    items.forEach(function (item) {
      var nav = item.getAttribute('data-nav');
      if (nav === currentNav) {
        item.classList.add('active');
      } else {
        item.classList.remove('active');
      }
    });
  }

  /* ---------- More sheet toggle ---------- */
  function initMoreSheet() {
    var moreBtn = document.querySelector('.bnav-item[data-nav="more"]');
    var sheet = document.getElementById('mobile-sheet');
    var backdrop = document.getElementById('mobile-sheet-backdrop');

    if (!moreBtn || !sheet || !backdrop) return;

    function openSheet() {
      sheet.classList.add('open');
      backdrop.classList.add('open');
      document.body.style.overflow = 'hidden';
    }

    function closeSheet() {
      sheet.classList.remove('open');
      backdrop.classList.remove('open');
      document.body.style.overflow = '';
    }

    moreBtn.addEventListener('click', function (e) {
      e.preventDefault();
      if (sheet.classList.contains('open')) {
        closeSheet();
      } else {
        openSheet();
      }
    });

    backdrop.addEventListener('click', closeSheet);

    /* Close on Escape */
    document.addEventListener('keydown', function (e) {
      if (e.key === 'Escape' && sheet.classList.contains('open')) {
        closeSheet();
      }
    });
  }

  /* ---------- Init ---------- */
  function init() {
    setActiveNav();
    initMoreSheet();
  }

  if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', init);
  } else {
    init();
  }
})();
