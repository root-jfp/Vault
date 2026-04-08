/* ============================================
   VAULT GRID CONFIGURATOR
   Layout switching, widget visibility/reorder,
   sidebar position
   ============================================ */
(function () {
  'use strict';

  /* ---------- Widget definitions ---------- */
  var WIDGETS = [
    { key: 'streak', label: 'Habit streak & goals', icon: '<svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><path d="M22 11.08V12a10 10 0 1 1-5.93-9.14"/><polyline points="22 4 12 14.01 9 11.01"/></svg>' },
    { key: 'daily', label: 'Daily habit tracking', icon: '<svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><path d="M22 11.08V12a10 10 0 1 1-5.93-9.14"/><polyline points="22 4 12 14.01 9 11.01"/></svg>' },
    { key: 'tasks', label: 'Tasks', icon: '<svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><rect x="3" y="3" width="18" height="18" rx="2"/><path d="M3 9h18M9 21V9"/></svg>' },
    { key: 'projects', label: 'Project manager', icon: '<svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><path d="M2 3h6a4 4 0 0 1 4 4v14a3 3 0 0 0-3-3H2z"/><path d="M22 3h-6a4 4 0 0 0-4 4v14a3 3 0 0 1 3-3h7z"/></svg>' },
    { key: 'calendar', label: 'Calendar', icon: '<svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><rect x="3" y="4" width="18" height="18" rx="2"/><line x1="16" y1="2" x2="16" y2="6"/><line x1="8" y1="2" x2="8" y2="6"/><line x1="3" y1="10" x2="21" y2="10"/></svg>' },
    { key: 'weather', label: 'Weather', icon: '<svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><path d="M18 10h-1.26A8 8 0 1 0 9 20h9a5 5 0 0 0 0-10z"/></svg>' },
    { key: 'quick-actions', label: 'Quick actions', icon: '<svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><polygon points="13 2 3 14 12 14 11 22 21 10 12 10 13 2"/></svg>' },
    { key: 'daily-agenda', label: 'Daily agenda', icon: '<svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><rect x="3" y="4" width="18" height="18" rx="2"/><line x1="16" y1="2" x2="16" y2="6"/><line x1="8" y1="2" x2="8" y2="6"/><line x1="3" y1="10" x2="21" y2="10"/></svg>' },
    { key: 'streak-gamification', label: 'Streak & gamification', icon: '<svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><path d="M12 15l-3 3h6l-3-3z"/><circle cx="12" cy="8" r="4"/><path d="M5.52 19h12.96"/></svg>' },
    { key: 'water-health', label: 'Water & health', icon: '<svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><path d="M12 2.69l5.66 5.66a8 8 0 1 1-11.31 0z"/></svg>' },
    { key: 'fitness-weight', label: 'Fitness & weight', icon: '<svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><path d="M18 20V10M12 20V4M6 20v-6"/></svg>' },
    { key: 'birthdays', label: 'Birthdays & celebrations', icon: '<svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><path d="M20 21v-2a4 4 0 0 0-4-4H8a4 4 0 0 0-4 4v2"/><circle cx="12" cy="7" r="4"/></svg>' },
    { key: 'budget', label: 'Monthly budget', icon: '<svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><line x1="12" y1="1" x2="12" y2="23"/><path d="M17 5H9.5a3.5 3.5 0 0 0 0 7h5a3.5 3.5 0 0 1 0 7H6"/></svg>' },
    { key: 'notes', label: 'Quick notes', icon: '<svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><path d="M11 4H4a2 2 0 0 0-2 2v14a2 2 0 0 0 2 2h14a2 2 0 0 0 2-2v-7"/><path d="M18.5 2.5a2.121 2.121 0 0 1 3 3L12 15l-4 1 1-4 9.5-9.5z"/></svg>' },
    { key: 'shopping', label: 'Shopping list', icon: '<svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><circle cx="9" cy="21" r="1"/><circle cx="20" cy="21" r="1"/><path d="M1 1h4l2.68 13.39a2 2 0 0 0 2 1.61h9.72a2 2 0 0 0 2-1.61L23 6H6"/></svg>' },
    { key: 'meal-planner', label: 'Meal planner', icon: '<svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><path d="M18 8h1a4 4 0 0 1 0 8h-1"/><path d="M2 8h16v9a4 4 0 0 1-4 4H6a4 4 0 0 1-4-4V8z"/><line x1="6" y1="1" x2="6" y2="4"/><line x1="10" y1="1" x2="10" y2="4"/><line x1="14" y1="1" x2="14" y2="4"/></svg>' },
    { key: 'inventory-pantry', label: 'Inventory & pantry', icon: '<svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><path d="M21 16V8a2 2 0 0 0-1-1.73l-7-4a2 2 0 0 0-2 0l-7 4A2 2 0 0 0 3 8v8a2 2 0 0 0 1 1.73l7 4a2 2 0 0 0 2 0l7-4A2 2 0 0 0 21 16z"/></svg>' },
    { key: 'chore-rotation', label: 'Chore rotation', icon: '<svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><path d="M17 21v-2a4 4 0 0 0-4-4H5a4 4 0 0 0-4 4v2"/><circle cx="9" cy="7" r="4"/><path d="M23 21v-2a4 4 0 0 0-3-3.87"/><path d="M16 3.13a4 4 0 0 1 0 7.75"/></svg>' },
    { key: 'utility-meters', label: 'Utility meters', icon: '<svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><path d="M18 20V10M12 20V4M6 20v-6"/></svg>' }
  ];

  /* ---------- Preset definitions ---------- */
  var PRESETS = {
    stack: {
      label: 'Stack',
      thumb: '<div class="vg-thumb"><div class="vg-thumb-row"></div><div class="vg-thumb-row"></div><div class="vg-thumb-row"></div><div class="vg-thumb-row"></div><div class="vg-thumb-row"></div></div>'
    },
    'two-col': {
      label: 'Two Column',
      thumb: '<div class="vg-thumb"><div class="vg-thumb-cols"><div class="vg-thumb-col"></div><div class="vg-thumb-col"></div></div><div class="vg-thumb-cols"><div class="vg-thumb-col"></div><div class="vg-thumb-col"></div></div><div class="vg-thumb-cols"><div class="vg-thumb-col"></div><div class="vg-thumb-col"></div></div></div>'
    },
    dashboard: {
      label: 'Dashboard',
      thumb: '<div class="vg-thumb"><div class="vg-thumb-row" style="flex:2"></div><div class="vg-thumb-cols"><div class="vg-thumb-col"></div><div class="vg-thumb-col"></div></div><div class="vg-thumb-cols"><div class="vg-thumb-col"></div><div class="vg-thumb-col"></div></div></div>'
    },
    focus: {
      label: 'Focus',
      thumb: '<div class="vg-thumb"><div class="vg-thumb-row" style="flex:3"></div><div class="vg-thumb-cols"><div class="vg-thumb-col"></div><div class="vg-thumb-col"></div><div class="vg-thumb-col"></div><div class="vg-thumb-col"></div></div></div>'
    },
    compact: {
      label: 'Compact',
      thumb: '<div class="vg-thumb"><div class="vg-thumb-cols"><div class="vg-thumb-col"></div><div class="vg-thumb-col"></div><div class="vg-thumb-col"></div></div><div class="vg-thumb-cols"><div class="vg-thumb-col"></div><div class="vg-thumb-col"></div><div class="vg-thumb-col"></div></div></div>'
    }
  };

  /* ---------- Default state ---------- */
  var ORIGINAL_KEYS = { streak: true, daily: true, tasks: true, projects: true, calendar: true };
  var defaultVisibility = {};
  WIDGETS.forEach(function (w) { defaultVisibility[w.key] = !!ORIGINAL_KEYS[w.key]; });
  var defaultOrder = WIDGETS.map(function (w) { return w.key; });

  /* ---------- State (loaded from VaultStore if available) ---------- */
  var hasStore = typeof VaultStore !== 'undefined';
  var state = {
    activePreset: hasStore ? VaultStore.get('grid_preset', 'stack') : 'stack',
    widgetVisibility: hasStore ? VaultStore.get('grid_widget_visibility', defaultVisibility) : defaultVisibility,
    widgetOrder: hasStore ? VaultStore.get('grid_widget_order', defaultOrder) : defaultOrder,
    sidebarPosition: hasStore ? VaultStore.get('grid_sidebar_position', 'left') : 'left'
  };

  function persistState() {
    if (!hasStore) return;
    VaultStore.set('grid_preset', state.activePreset);
    VaultStore.set('grid_widget_visibility', state.widgetVisibility);
    VaultStore.set('grid_widget_order', state.widgetOrder);
    VaultStore.set('grid_sidebar_position', state.sidebarPosition);
  }

  /* ---------- DOM refs ---------- */
  var panel, backdrop, content, layout;
  var presetsContainer, widgetsContainer, orderContainer, sidebarContainer;

  /* ---------- Registry bridge ---------- */
  var hasRegistry = typeof VaultWidgetRegistry !== 'undefined';

  function getWidgetList() {
    if (hasRegistry) {
      return VaultWidgetRegistry.getAllTypes().map(function (def) {
        return { key: def.key, label: def.label, icon: def.icon };
      });
    }
    return WIDGETS;
  }

  /* ---------- Helpers ---------- */
  function $(sel, ctx) { return (ctx || document).querySelector(sel); }

  function widgetByKey(key) {
    if (hasRegistry) {
      var def = VaultWidgetRegistry.getType(key);
      if (def) return { key: def.key, label: def.label, icon: def.icon };
    }
    for (var i = 0; i < WIDGETS.length; i++) {
      if (WIDGETS[i].key === key) return WIDGETS[i];
    }
    return null;
  }

  /* ---------- Init ---------- */
  function init() {
    panel = $('#vg-panel');
    backdrop = $('#vg-backdrop');
    content = $('#vg-content');
    layout = $('#vg-layout');
    if (!panel || !backdrop || !content || !layout) return;

    presetsContainer = $('#vg-presets');
    widgetsContainer = $('#vg-widgets');
    orderContainer = $('#vg-order');
    sidebarContainer = $('#vg-sidebar');

    // Trigger button
    var trigger = $('#vg-trigger');
    if (trigger) trigger.addEventListener('click', openPanel);

    // Close
    var closeBtn = $('#vg-panel-close');
    if (closeBtn) closeBtn.addEventListener('click', closePanel);
    backdrop.addEventListener('click', closePanel);
    document.addEventListener('keydown', function (e) {
      if (e.key === 'Escape' && panel.classList.contains('open')) closePanel();
    });

    renderPresets();
    renderWidgets();
    renderOrder();
    renderSidebar();

    // Restore saved layout state
    applyLayout(state.activePreset);
    applyWidgetOrder();
    // Apply visibility
    getWidgetList().forEach(function (w) {
      if (state.widgetVisibility[w.key] === false) {
        var sec = content.querySelector('.sec[data-widget="' + w.key + '"]');
        if (sec) sec.classList.add('vg-hidden');
      }
    });
    if (state.sidebarPosition !== 'left') setSidebar(state.sidebarPosition);
  }

  /* ---------- Panel open/close ---------- */
  function openPanel() {
    panel.classList.add('open');
    backdrop.classList.add('open');
  }

  function closePanel() {
    panel.classList.remove('open');
    backdrop.classList.remove('open');
  }

  /* ==============================
     PRESETS
     ============================== */
  function renderPresets() {
    if (!presetsContainer) return;
    var html = '';
    var keys = Object.keys(PRESETS);
    keys.forEach(function (key) {
      var p = PRESETS[key];
      var cls = key === state.activePreset ? 'vg-preset active' : 'vg-preset';
      html += '<div class="' + cls + '" data-preset="' + key + '">';
      html += p.thumb;
      html += '<div class="vg-preset-label">' + p.label + '</div>';
      html += '</div>';
    });
    presetsContainer.innerHTML = html;

    // Click delegation
    presetsContainer.addEventListener('click', function (e) {
      var el = e.target.closest('.vg-preset');
      if (!el) return;
      selectPreset(el.getAttribute('data-preset'));
    });
  }

  function selectPreset(key) {
    if (!PRESETS[key]) return;
    state.activePreset = key;
    applyLayout(key);
    persistState();

    // Update active class
    var items = presetsContainer.querySelectorAll('.vg-preset');
    items.forEach(function (el) {
      el.classList.toggle('active', el.getAttribute('data-preset') === key);
    });
  }

  function applyLayout(key) {
    // Remove focus wrapper if it exists
    unwrapFocusRow();

    // Remove all layout classes
    var classes = ['vg-layout-stack', 'vg-layout-two-col', 'vg-layout-dashboard', 'vg-layout-focus', 'vg-layout-compact'];
    classes.forEach(function (c) { content.classList.remove(c); });

    // Add selected
    content.classList.add('vg-layout-' + key);

    // Focus layout: wrap non-first sections in focus row
    if (key === 'focus') {
      wrapFocusRow();
    }
  }

  function wrapFocusRow() {
    var sections = content.querySelectorAll('.sec:not(.vg-hidden)');
    if (sections.length <= 1) return;
    var row = document.createElement('div');
    row.className = 'vg-focus-row';
    // Insert row after first section
    sections[0].after(row);
    for (var i = 1; i < sections.length; i++) {
      row.appendChild(sections[i]);
    }
  }

  function unwrapFocusRow() {
    var row = content.querySelector('.vg-focus-row');
    if (!row) return;
    var children = Array.prototype.slice.call(row.children);
    children.forEach(function (child) {
      row.before(child);
    });
    row.remove();
  }

  /* ==============================
     WIDGET VISIBILITY
     ============================== */
  function renderWidgets() {
    if (!widgetsContainer) return;
    var html = '';
    var list = getWidgetList();
    list.forEach(function (w) {
      var checked = state.widgetVisibility[w.key] !== false ? 'checked' : '';
      html += '<div class="vg-widget-row">';
      html += '<div class="vg-widget-name">' + w.icon + w.label + '</div>';
      html += '<label class="vg-toggle"><input type="checkbox" ' + checked + ' data-widget-key="' + w.key + '"><span class="vg-toggle-track"></span></label>';
      html += '</div>';
    });
    widgetsContainer.innerHTML = html;

    widgetsContainer.addEventListener('change', function (e) {
      var input = e.target;
      if (!input.hasAttribute('data-widget-key')) return;
      toggleWidget(input.getAttribute('data-widget-key'), input.checked);
    });
  }

  function toggleWidget(key, visible) {
    state.widgetVisibility[key] = visible;
    persistState();
    var sec = content.querySelector('.sec[data-widget="' + key + '"]');
    if (!sec) return;

    // If in focus layout, unwrap first, toggle, re-wrap
    var wasFocus = state.activePreset === 'focus';
    if (wasFocus) unwrapFocusRow();

    if (visible) {
      sec.classList.remove('vg-hidden');
    } else {
      sec.classList.add('vg-hidden');
    }

    if (wasFocus) wrapFocusRow();
  }

  /* ==============================
     WIDGET REORDER
     ============================== */
  var dragSrcKey = null;

  function renderOrder() {
    if (!orderContainer) return;
    buildOrderHTML();
  }

  function buildOrderHTML() {
    var html = '';
    state.widgetOrder.forEach(function (key, idx) {
      var w = widgetByKey(key);
      if (!w) return;
      html += '<div class="vg-order-row" draggable="true" data-order-key="' + key + '">';
      html += '<div class="vg-grip"><svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><circle cx="9" cy="6" r="1"/><circle cx="15" cy="6" r="1"/><circle cx="9" cy="12" r="1"/><circle cx="15" cy="12" r="1"/><circle cx="9" cy="18" r="1"/><circle cx="15" cy="18" r="1"/></svg></div>';
      html += '<div class="vg-order-name">' + w.label + '</div>';
      html += '<div class="vg-order-arrows">';
      html += '<button class="vg-arrow-btn" data-move="up" ' + (idx === 0 ? 'disabled' : '') + '><svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5"><polyline points="18 15 12 9 6 15"/></svg></button>';
      html += '<button class="vg-arrow-btn" data-move="down" ' + (idx === state.widgetOrder.length - 1 ? 'disabled' : '') + '><svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5"><polyline points="6 9 12 15 18 9"/></svg></button>';
      html += '</div>';
      html += '</div>';
    });
    orderContainer.innerHTML = html;

    // Arrow buttons
    orderContainer.querySelectorAll('.vg-arrow-btn').forEach(function (btn) {
      btn.addEventListener('click', function (e) {
        e.stopPropagation();
        var row = btn.closest('.vg-order-row');
        var key = row.getAttribute('data-order-key');
        var dir = btn.getAttribute('data-move') === 'up' ? -1 : 1;
        moveWidget(key, dir);
      });
    });

    // Drag events
    var rows = orderContainer.querySelectorAll('.vg-order-row');
    rows.forEach(function (row) {
      row.addEventListener('dragstart', function (e) {
        dragSrcKey = row.getAttribute('data-order-key');
        row.classList.add('dragging');
        e.dataTransfer.effectAllowed = 'move';
      });
      row.addEventListener('dragend', function () {
        row.classList.remove('dragging');
        dragSrcKey = null;
        rows.forEach(function (r) { r.classList.remove('drag-over'); });
      });
      row.addEventListener('dragover', function (e) {
        e.preventDefault();
        e.dataTransfer.dropEffect = 'move';
        row.classList.add('drag-over');
      });
      row.addEventListener('dragleave', function () {
        row.classList.remove('drag-over');
      });
      row.addEventListener('drop', function (e) {
        e.preventDefault();
        row.classList.remove('drag-over');
        var targetKey = row.getAttribute('data-order-key');
        if (dragSrcKey && dragSrcKey !== targetKey) {
          reorderByDrag(dragSrcKey, targetKey);
        }
      });
    });
  }

  function moveWidget(key, dir) {
    var idx = state.widgetOrder.indexOf(key);
    if (idx < 0) return;
    var newIdx = idx + dir;
    if (newIdx < 0 || newIdx >= state.widgetOrder.length) return;

    // Swap in state
    var temp = state.widgetOrder[newIdx];
    state.widgetOrder[newIdx] = key;
    state.widgetOrder[idx] = temp;

    persistState();
    buildOrderHTML();
    applyWidgetOrder();
  }

  function reorderByDrag(srcKey, targetKey) {
    var srcIdx = state.widgetOrder.indexOf(srcKey);
    var targetIdx = state.widgetOrder.indexOf(targetKey);
    if (srcIdx < 0 || targetIdx < 0) return;

    // Remove src and insert at target position
    state.widgetOrder.splice(srcIdx, 1);
    var insertIdx = state.widgetOrder.indexOf(targetKey);
    state.widgetOrder.splice(insertIdx, 0, srcKey);

    persistState();
    buildOrderHTML();
    applyWidgetOrder();
  }

  function applyWidgetOrder() {
    var wasFocus = state.activePreset === 'focus';
    if (wasFocus) unwrapFocusRow();

    state.widgetOrder.forEach(function (key) {
      var sec = content.querySelector('.sec[data-widget="' + key + '"]');
      if (sec) content.appendChild(sec);
    });

    if (wasFocus) wrapFocusRow();
  }

  /* ==============================
     SIDEBAR POSITION
     ============================== */
  function renderSidebar() {
    if (!sidebarContainer) return;
    var opts = [
      { key: 'left', label: 'Left' },
      { key: 'right', label: 'Right' },
      { key: 'hidden', label: 'Hidden' }
    ];
    var html = '';
    opts.forEach(function (o) {
      var cls = o.key === state.sidebarPosition ? 'vg-sidebar-opt active' : 'vg-sidebar-opt';
      html += '<button class="' + cls + '" data-sidebar="' + o.key + '">' + o.label + '</button>';
    });
    sidebarContainer.innerHTML = html;

    sidebarContainer.addEventListener('click', function (e) {
      var btn = e.target.closest('.vg-sidebar-opt');
      if (!btn) return;
      setSidebar(btn.getAttribute('data-sidebar'));
    });
  }

  function setSidebar(key) {
    state.sidebarPosition = key;
    persistState();

    // Update button states
    sidebarContainer.querySelectorAll('.vg-sidebar-opt').forEach(function (btn) {
      btn.classList.toggle('active', btn.getAttribute('data-sidebar') === key);
    });

    // Remove all sidebar classes
    layout.classList.remove('vg-sidebar-right', 'vg-sidebar-hidden');

    if (key === 'right') {
      layout.classList.add('vg-sidebar-right');
    } else if (key === 'hidden') {
      layout.classList.add('vg-sidebar-hidden');
    }
    // 'left' is the default — no class needed
  }

  /* ---------- Boot ---------- */
  if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', init);
  } else {
    init();
  }

})();
