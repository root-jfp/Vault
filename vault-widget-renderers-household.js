/* ============================================
   VAULT WIDGET RENDERERS — HOUSEHOLD
   Shopping, Meal Planner, Inventory/Pantry,
   Chore Rotation, Utility Meters widgets.
   ============================================ */
(function () {
  'use strict';

  /* ===== SAMPLE DATA ===== */

  var shoppingData = {
    categories: [
      {
        name: 'Produce',
        icon: '<svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><path d="M12 2.69l5.66 5.66a8 8 0 1 1-11.31 0z"/></svg>',
        items: [
          { name: 'Bananas', qty: '1 bunch', checked: false, recurring: true },
          { name: 'Tomatoes', qty: '500g', checked: true, recurring: false }
        ]
      },
      {
        name: 'Dairy',
        icon: '<svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><path d="M18 8h1a4 4 0 0 1 0 8h-1"/><path d="M2 8h16v9a4 4 0 0 1-4 4H6a4 4 0 0 1-4-4V8z"/></svg>',
        items: [
          { name: 'Milk', qty: '1L', checked: false, recurring: true },
          { name: 'Greek yogurt', qty: '4 pack', checked: false, recurring: false }
        ]
      },
      {
        name: 'Bakery',
        icon: '<svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><path d="M17 8h1a4 4 0 1 1 0 8h-1"/><path d="M3 8h14v9a4 4 0 0 1-4 4H7a4 4 0 0 1-4-4V8z"/></svg>',
        items: [
          { name: 'Sourdough bread', qty: '1 loaf', checked: true, recurring: false }
        ]
      },
      {
        name: 'Cleaning',
        icon: '<svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><path d="M12 2.69l5.66 5.66a8 8 0 1 1-11.31 0z"/></svg>',
        items: [
          { name: 'Dish soap', qty: '1', checked: false, recurring: true },
          { name: 'Paper towels', qty: '2 rolls', checked: false, recurring: false }
        ]
      },
      {
        name: 'Other',
        icon: '<svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><circle cx="12" cy="12" r="10"/><line x1="12" y1="8" x2="12" y2="12"/><line x1="12" y1="16" x2="12.01" y2="16"/></svg>',
        items: [
          { name: 'Batteries AA', qty: '4 pack', checked: false, recurring: false }
        ]
      }
    ]
  };

  var mealData = {
    days: ['Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat', 'Sun'],
    todayIdx: 0,
    meals: {
      lunch: ['Grilled chicken salad', 'Pasta carbonara', 'Fish tacos', 'Veggie stir fry', 'Bacalhau', '', 'BBQ'],
      dinner: ['Soup & bread', 'Salmon & rice', 'Pizza night', 'Lentil curry', 'Leftovers', 'Dining out', '']
    }
  };

  var inventoryData = [
    { name: 'Rice', stock: 'high', detail: '2kg bag, 80% full', alert: false },
    { name: 'Pasta', stock: 'medium', detail: '3 packs remaining', alert: false },
    { name: 'Olive oil', stock: 'low', detail: '~100ml left', alert: true },
    { name: 'Canned tuna', stock: 'high', detail: '6 cans', alert: false },
    { name: 'Coffee beans', stock: 'low', detail: '~50g left', alert: true },
    { name: 'Flour', stock: 'medium', detail: '1kg bag, half full', alert: false },
    { name: 'Sugar', stock: 'high', detail: '1kg bag, 90% full', alert: false },
    { name: 'Toilet paper', stock: 'critical', detail: '1 roll left!', alert: true }
  ];

  var choreData = {
    fairness: { jose: 52, wife: 48 },
    chores: [
      { name: 'Vacuum living room', assignee: 'jose', status: 'done', frequency: 'Weekly' },
      { name: 'Clean bathroom', assignee: 'wife', status: 'done', frequency: 'Weekly' },
      { name: 'Cook dinner', assignee: 'jose', status: 'pending', frequency: 'Daily rotation' },
      { name: 'Wash dishes', assignee: 'wife', status: 'pending', frequency: 'Daily rotation' },
      { name: 'Take out trash', assignee: 'jose', status: 'done', frequency: '2x/week' },
      { name: 'Laundry', assignee: 'wife', status: 'pending', frequency: '2x/week' }
    ]
  };

  var utilityData = [
    {
      name: 'Electricity',
      unit: 'kWh',
      current: 285,
      trend: 'up',
      trendPct: 8,
      months: [
        { label: 'Nov', value: 310 },
        { label: 'Dec', value: 340 },
        { label: 'Jan', value: 360 },
        { label: 'Feb', value: 320 },
        { label: 'Mar', value: 295 },
        { label: 'Apr', value: 285 }
      ]
    },
    {
      name: 'Water',
      unit: 'm\u00b3',
      current: 8.2,
      trend: 'down',
      trendPct: 5,
      months: [
        { label: 'Nov', value: 9.1 },
        { label: 'Dec', value: 8.8 },
        { label: 'Jan', value: 9.5 },
        { label: 'Feb', value: 8.9 },
        { label: 'Mar', value: 8.6 },
        { label: 'Apr', value: 8.2 }
      ]
    },
    {
      name: 'Gas',
      unit: 'm\u00b3',
      current: 12.4,
      trend: 'down',
      trendPct: 15,
      months: [
        { label: 'Nov', value: 18.2 },
        { label: 'Dec', value: 22.1 },
        { label: 'Jan', value: 24.5 },
        { label: 'Feb', value: 20.3 },
        { label: 'Mar', value: 14.6 },
        { label: 'Apr', value: 12.4 }
      ]
    }
  ];

  /* ===== RENDER: SHOPPING LIST ===== */

  function renderShopping(container) {
    var checkSvg = '<svg viewBox="0 0 24 24" fill="none" stroke="#fff" stroke-width="3"><polyline points="20 6 9 17 4 12"/></svg>';
    var html = '<div class="sec-hdr"><svg viewBox="0 0 24 24" fill="none" stroke="var(--ac)" stroke-width="2"><circle cx="9" cy="21" r="1"/><circle cx="20" cy="21" r="1"/><path d="M1 1h4l2.68 13.39a2 2 0 0 0 2 1.61h9.72a2 2 0 0 0 2-1.61L23 6H6"/></svg> Shopping list<span class="sec-new">+ Add</span></div>';

    var totalItems = 0;
    var checkedItems = 0;
    shoppingData.categories.forEach(function (cat) {
      cat.items.forEach(function (item) {
        totalItems++;
        if (item.checked) checkedItems++;
      });
    });
    html += '<div style="font-size:10px;color:var(--tx3);margin-bottom:8px">' + checkedItems + '/' + totalItems + ' items checked</div>';

    html += '<div class="shopping-cats">';
    shoppingData.categories.forEach(function (cat) {
      html += '<div>';
      html += '<div class="shopping-cat-title">' + cat.icon + ' ' + cat.name + '</div>';
      html += '<div class="shopping-items">';
      cat.items.forEach(function (item) {
        var cls = 'shopping-item' + (item.checked ? ' checked' : '');
        html += '<div class="' + cls + '">';
        html += '<div class="shopping-check' + (item.checked ? ' done' : '') + '">' + checkSvg + '</div>';
        html += '<span>' + item.name + '</span>';
        if (item.recurring) html += ' <span class="shopping-recurring">recurring</span>';
        html += '<span class="shopping-qty">' + item.qty + '</span>';
        html += '</div>';
      });
      html += '</div></div>';
    });
    html += '</div>';

    container.innerHTML = html;
  }

  /* ===== RENDER: MEAL PLANNER ===== */

  function renderMealPlanner(container) {
    var m = mealData;
    var html = '<div class="sec-hdr"><svg viewBox="0 0 24 24" fill="none" stroke="var(--wn)" stroke-width="2"><path d="M18 8h1a4 4 0 0 1 0 8h-1"/><path d="M2 8h16v9a4 4 0 0 1-4 4H6a4 4 0 0 1-4-4V8z"/><line x1="6" y1="1" x2="6" y2="4"/><line x1="10" y1="1" x2="10" y2="4"/><line x1="14" y1="1" x2="14" y2="4"/></svg> Meal planner<span class="sec-new">This week</span></div>';

    html += '<div class="meal-grid">';
    /* header row */
    html += '<div class="meal-header"></div>';
    m.days.forEach(function (day, i) {
      html += '<div class="meal-header">' + day + '</div>';
    });

    /* lunch row */
    html += '<div class="meal-label">Lunch</div>';
    m.meals.lunch.forEach(function (meal, i) {
      var cls = 'meal-cell' + (i === m.todayIdx ? ' today' : '') + (!meal ? ' empty' : '');
      html += '<div class="' + cls + '">' + (meal || '\u2014') + '</div>';
    });

    /* dinner row */
    html += '<div class="meal-label">Dinner</div>';
    m.meals.dinner.forEach(function (meal, i) {
      var cls = 'meal-cell' + (i === m.todayIdx ? ' today' : '') + (!meal ? ' empty' : '');
      html += '<div class="' + cls + '">' + (meal || '\u2014') + '</div>';
    });

    html += '</div>';

    container.innerHTML = html;
  }

  /* ===== RENDER: INVENTORY/PANTRY ===== */

  function renderInventory(container) {
    var alertSvg = '<svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><path d="M10.29 3.86L1.82 18a2 2 0 0 0 1.71 3h16.94a2 2 0 0 0 1.71-3L13.71 3.86a2 2 0 0 0-3.42 0z"/><line x1="12" y1="9" x2="12" y2="13"/><line x1="12" y1="17" x2="12.01" y2="17"/></svg>';

    var lowCount = inventoryData.filter(function (i) { return i.stock === 'low' || i.stock === 'critical'; }).length;

    var html = '<div class="sec-hdr"><svg viewBox="0 0 24 24" fill="none" stroke="var(--inf)" stroke-width="2"><path d="M21 16V8a2 2 0 0 0-1-1.73l-7-4a2 2 0 0 0-2 0l-7 4A2 2 0 0 0 3 8v8a2 2 0 0 0 1 1.73l7 4a2 2 0 0 0 2 0l7-4A2 2 0 0 0 21 16z"/></svg> Inventory &amp; pantry';
    if (lowCount > 0) html += '<span style="margin-left:8px;font-size:9px;padding:2px 6px;border-radius:3px;background:var(--wnbg);color:var(--wntx)">' + lowCount + ' low</span>';
    html += '</div>';

    html += '<div class="inventory-list">';
    inventoryData.forEach(function (item) {
      html += '<div class="inventory-card">';
      html += '<div class="inventory-name">' + item.name + ' <span class="inventory-stock ' + item.stock + '">' + item.stock + '</span></div>';
      html += '<div class="inventory-detail">' + item.detail + '</div>';
      if (item.alert) html += '<div class="inventory-alert">' + alertSvg + ' Restock soon</div>';
      html += '</div>';
    });
    html += '</div>';

    container.innerHTML = html;
  }

  /* ===== RENDER: CHORE ROTATION ===== */

  function renderChoreRotation(container) {
    var c = choreData;
    var html = '<div class="sec-hdr"><svg viewBox="0 0 24 24" fill="none" stroke="var(--pu)" stroke-width="2"><path d="M17 21v-2a4 4 0 0 0-4-4H5a4 4 0 0 0-4 4v2"/><circle cx="9" cy="7" r="4"/><path d="M23 21v-2a4 4 0 0 0-3-3.87"/><path d="M16 3.13a4 4 0 0 1 0 7.75"/></svg> Chore rotation</div>';

    html += '<div class="chore-list">';
    c.chores.forEach(function (chore) {
      html += '<div class="chore-row">';
      html += '<div class="chore-name">' + chore.name + '</div>';
      html += '<span class="chore-assignee ' + chore.assignee + '">' + (chore.assignee === 'jose' ? 'Jose' : 'Maria') + '</span>';
      html += '<span class="chore-status ' + chore.status + '">' + (chore.status === 'done' ? 'Done' : 'Pending') + '</span>';
      html += '<span style="font-size:8px;color:var(--tx3)">' + chore.frequency + '</span>';
      html += '</div>';
    });
    html += '</div>';

    html += '<div class="chore-fairness">';
    html += '<div class="chore-fairness-label"><span>Jose ' + c.fairness.jose + '%</span><span>Maria ' + c.fairness.wife + '%</span></div>';
    html += '<div class="chore-fairness-bar">';
    html += '<div class="chore-fairness-fill jose" style="width:' + c.fairness.jose + '%"></div>';
    html += '<div class="chore-fairness-fill wife" style="width:' + c.fairness.wife + '%"></div>';
    html += '</div></div>';

    container.innerHTML = html;
  }

  /* ===== RENDER: UTILITY METERS ===== */

  function renderUtilityMeters(container) {
    var trendUpSvg = '<svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><line x1="12" y1="19" x2="12" y2="5"/><polyline points="5 12 12 5 19 12"/></svg>';
    var trendDownSvg = '<svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><line x1="12" y1="5" x2="12" y2="19"/><polyline points="19 12 12 19 5 12"/></svg>';

    var html = '<div class="sec-hdr"><svg viewBox="0 0 24 24" fill="none" stroke="var(--inf)" stroke-width="2"><path d="M18 20V10M12 20V4M6 20v-6"/></svg> Utility meters</div>';

    html += '<div class="utility-list">';
    utilityData.forEach(function (u) {
      var maxVal = Math.max.apply(null, u.months.map(function (m) { return m.value; }));

      html += '<div class="utility-card">';
      html += '<div class="utility-header">';
      html += '<div class="utility-name">' + u.name + '</div>';
      html += '<div class="utility-trend ' + u.trend + '">' + (u.trend === 'up' ? trendUpSvg : trendDownSvg) + ' ' + u.trendPct + '%</div>';
      html += '</div>';
      html += '<div class="utility-value">' + u.current + ' <span class="utility-unit">' + u.unit + '</span></div>';

      /* Mini bar chart */
      html += '<div class="utility-chart"><svg viewBox="0 0 180 40" preserveAspectRatio="none">';
      var barW = 22;
      var gap = 8;
      u.months.forEach(function (m, i) {
        var h = maxVal > 0 ? (m.value / maxVal) * 36 : 0;
        var x = i * (barW + gap);
        var isLast = i === u.months.length - 1;
        html += '<rect x="' + x + '" y="' + (40 - h) + '" width="' + barW + '" height="' + h + '" fill="' + (isLast ? 'var(--inf)' : 'var(--bd)') + '" rx="2"/>';
      });
      html += '</svg></div>';

      html += '</div>';
    });
    html += '</div>';

    container.innerHTML = html;
  }

  /* ===== REGISTER ===== */

  if (typeof VaultWidgetRegistry !== 'undefined') {
    VaultWidgetRegistry.registerType('shopping', {
      label: 'Shopping list',
      icon: '<svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><circle cx="9" cy="21" r="1"/><circle cx="20" cy="21" r="1"/><path d="M1 1h4l2.68 13.39a2 2 0 0 0 2 1.61h9.72a2 2 0 0 0 2-1.61L23 6H6"/></svg>',
      category: 'household',
      defaultConfig: {},
      render: renderShopping
    });

    VaultWidgetRegistry.registerType('meal-planner', {
      label: 'Meal planner',
      icon: '<svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><path d="M18 8h1a4 4 0 0 1 0 8h-1"/><path d="M2 8h16v9a4 4 0 0 1-4 4H6a4 4 0 0 1-4-4V8z"/><line x1="6" y1="1" x2="6" y2="4"/><line x1="10" y1="1" x2="10" y2="4"/><line x1="14" y1="1" x2="14" y2="4"/></svg>',
      category: 'household',
      defaultConfig: {},
      render: renderMealPlanner
    });

    VaultWidgetRegistry.registerType('inventory-pantry', {
      label: 'Inventory & pantry',
      icon: '<svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><path d="M21 16V8a2 2 0 0 0-1-1.73l-7-4a2 2 0 0 0-2 0l-7 4A2 2 0 0 0 3 8v8a2 2 0 0 0 1 1.73l7 4a2 2 0 0 0 2 0l7-4A2 2 0 0 0 21 16z"/></svg>',
      category: 'household',
      defaultConfig: {},
      render: renderInventory
    });

    VaultWidgetRegistry.registerType('chore-rotation', {
      label: 'Chore rotation',
      icon: '<svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><path d="M17 21v-2a4 4 0 0 0-4-4H5a4 4 0 0 0-4 4v2"/><circle cx="9" cy="7" r="4"/><path d="M23 21v-2a4 4 0 0 0-3-3.87"/><path d="M16 3.13a4 4 0 0 1 0 7.75"/></svg>',
      category: 'household',
      defaultConfig: {},
      render: renderChoreRotation
    });

    VaultWidgetRegistry.registerType('utility-meters', {
      label: 'Utility meters',
      icon: '<svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><path d="M18 20V10M12 20V4M6 20v-6"/></svg>',
      category: 'household',
      defaultConfig: {},
      render: renderUtilityMeters
    });
  }

})();
