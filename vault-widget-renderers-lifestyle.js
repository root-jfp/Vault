/* ============================================
   VAULT WIDGET RENDERERS — LIFESTYLE
   Water/Health, Fitness/Weight, Birthdays,
   Budget, Notes widgets.
   ============================================ */
(function () {
  'use strict';

  /* ===== SAMPLE DATA ===== */

  var waterData = {
    current: 1250,
    goal: 2000,
    glasses: 5,
    glassSize: 250,
    history: [
      { day: 'Wed', amount: 2000 },
      { day: 'Thu', amount: 1500 },
      { day: 'Fri', amount: 2000 },
      { day: 'Sat', amount: 750 },
      { day: 'Sun', amount: 1750 }
    ]
  };

  var fitnessData = {
    current: 78.5,
    goal: 75,
    unit: 'kg',
    entries: [
      { date: 'Apr 2', weight: 79.2 },
      { date: 'Apr 3', weight: 79.0 },
      { date: 'Apr 4', weight: 78.8 },
      { date: 'Apr 5', weight: 78.9 },
      { date: 'Apr 6', weight: 78.6 },
      { date: 'Apr 7', weight: 78.5 }
    ]
  };

  var birthdayData = [
    { name: 'Maria', date: 'Apr 13', daysUntil: 6, color: '#E91E63' },
    { name: 'Mom', date: 'Apr 29', daysUntil: 22, color: '#9C27B0' },
    { name: 'Pedro', date: 'May 15', daysUntil: 38, color: '#2196F3' },
    { name: 'Ana', date: 'Jun 2', daysUntil: 56, color: '#FF9800' },
    { name: 'Carlos', date: 'Jun 20', daysUntil: 74, color: '#4CAF50' }
  ];

  var budgetData = {
    monthly: 2500,
    spent: 1680,
    categories: [
      { name: 'Groceries', spent: 420, budget: 500, color: 'var(--ac)' },
      { name: 'Transport', spent: 180, budget: 200, color: 'var(--inf)' },
      { name: 'Dining out', spent: 280, budget: 200, color: 'var(--dn)' },
      { name: 'Utilities', spent: 310, budget: 350, color: 'var(--pu)' },
      { name: 'Shopping', spent: 290, budget: 400, color: 'var(--wn)' },
      { name: 'Other', spent: 200, budget: 300, color: 'var(--tx3)' }
    ]
  };

  var notesData = [
    { title: 'Shopping reminders', preview: 'Check prices for new router. Compare Asus vs TP-Link models for mesh WiFi.', pinned: true, starred: false, date: 'Apr 6' },
    { title: 'Recipe: Bacalhau', preview: 'Soak cod 48h, change water 3x. Potatoes, onion, garlic, olive oil, eggs.', pinned: false, starred: true, date: 'Apr 5' },
    { title: 'Car service notes', preview: 'Next oil change at 85,000km. Current: 83,200km. Check brake pads too.', pinned: false, starred: false, date: 'Apr 4' },
    { title: 'Gift ideas - Maria', preview: 'Perfume (Jo Malone), book about gardening, cooking class voucher.', pinned: true, starred: true, date: 'Apr 3' },
    { title: 'Home improvement', preview: 'Paint bedroom walls (light grey). Fix bathroom tile grout. Replace kitchen tap.', pinned: false, starred: false, date: 'Apr 1' }
  ];

  /* ===== HELPERS ===== */


  /* ===== RENDER: WATER/HEALTH ===== */

  function renderWaterHealth(container) {
    var w = waterData;
    var pct = Math.min(Math.round(w.current / w.goal * 100), 100);
    var totalGlasses = Math.ceil(w.goal / w.glassSize);
    var filledGlasses = Math.floor(w.current / w.glassSize);

    var html = '<div class="sec-hdr"><svg viewBox="0 0 24 24" fill="none" stroke="var(--inf)" stroke-width="2"><path d="M12 2.69l5.66 5.66a8 8 0 1 1-11.31 0z"/></svg> Water &amp; health</div>';

    html += '<div class="water-main">';
    html += '<div class="water-ring"><svg viewBox="0 0 80 80">';
    html += '<circle class="water-ring-bg" cx="40" cy="40" r="34"/>';
    var circ = 2 * Math.PI * 34;
    var offset = circ - (pct / 100) * circ;
    html += '<circle class="water-ring-fill" cx="40" cy="40" r="34" stroke-dasharray="' + circ.toFixed(1) + '" stroke-dashoffset="' + offset.toFixed(1) + '"/>';
    html += '</svg>';
    html += '<div class="water-ring-text"><span class="water-ring-pct">' + pct + '%</span><span class="water-ring-label">of goal</span></div>';
    html += '</div>';

    html += '<div class="water-info">';
    html += '<div class="water-amount">' + w.current + ' ml</div>';
    html += '<div class="water-goal">Goal: ' + w.goal + ' ml</div>';
    html += '<div class="water-glasses">';
    for (var i = 0; i < Math.min(totalGlasses, 8); i++) {
      var filled = i < filledGlasses ? ' filled' : '';
      html += '<div class="water-glass' + filled + '"><svg viewBox="0 0 24 24" fill="' + (i < filledGlasses ? 'currentColor' : 'none') + '" stroke="currentColor" stroke-width="2"><path d="M12 2.69l5.66 5.66a8 8 0 1 1-11.31 0z"/></svg></div>';
    }
    html += '</div></div></div>';

    html += '<div class="water-history">';
    w.history.forEach(function (d) {
      var h = Math.round((d.amount / w.goal) * 40);
      html += '<div class="water-bar-wrap">';
      html += '<div class="water-bar"><div class="water-bar-fill" style="height:' + Math.min(h, 40) + 'px"></div></div>';
      html += '<div class="water-bar-label">' + d.day + '</div>';
      html += '</div>';
    });
    html += '</div>';

    container.innerHTML = html;
  }

  /* ===== RENDER: FITNESS/WEIGHT ===== */

  function renderFitnessWeight(container) {
    var f = fitnessData;
    var delta = f.current - f.entries[0].weight;
    var deltaStr = (delta >= 0 ? '+' : '') + delta.toFixed(1);
    var toGoal = f.current - f.goal;
    var toGoalStr = toGoal.toFixed(1);

    var html = '<div class="sec-hdr"><svg viewBox="0 0 24 24" fill="none" stroke="var(--pu)" stroke-width="2"><path d="M18 20V10M12 20V4M6 20v-6"/></svg> Fitness &amp; weight</div>';

    html += '<div class="fitness-main">';
    html += '<div class="fitness-current">';
    html += '<div class="fitness-weight">' + f.current + '</div>';
    html += '<div class="fitness-unit">' + f.unit + '</div>';
    html += '<div class="fitness-goal">Goal: ' + f.goal + ' ' + f.unit + '</div>';
    html += '</div>';

    /* SVG sparkline */
    var weights = f.entries.map(function (e) { return e.weight; });
    var minW = Math.min.apply(null, weights) - 0.5;
    var maxW = Math.max.apply(null, weights) + 0.5;
    var range = maxW - minW || 1;
    var denominator = f.entries.length > 1 ? f.entries.length - 1 : 1;
    var points = f.entries.map(function (e, i) {
      var x = (i / denominator) * 200;
      var y = 55 - ((e.weight - minW) / range) * 50;
      return x.toFixed(1) + ',' + y.toFixed(1);
    }).join(' ');

    html += '<div class="fitness-chart"><svg viewBox="0 0 200 60" preserveAspectRatio="none">';
    html += '<polyline points="' + points + '" fill="none" stroke="var(--pu)" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"/>';
    /* goal line */
    var goalY = 55 - ((f.goal - minW) / range) * 50;
    html += '<line x1="0" y1="' + goalY.toFixed(1) + '" x2="200" y2="' + goalY.toFixed(1) + '" stroke="var(--ac)" stroke-width="1" stroke-dasharray="4 3" opacity="0.5"/>';
    html += '</svg></div></div>';

    html += '<div class="fitness-delta">';
    html += '<div class="fitness-delta-card"><div class="fitness-delta-val ' + (delta <= 0 ? 'negative' : 'positive') + '">' + deltaStr + ' ' + f.unit + '</div><div class="fitness-delta-label">This week</div></div>';
    html += '<div class="fitness-delta-card"><div class="fitness-delta-val ' + (toGoal <= 0 ? 'negative' : 'positive') + '">' + toGoalStr + ' ' + f.unit + '</div><div class="fitness-delta-label">To goal</div></div>';
    html += '</div>';

    html += '<div class="fitness-entries">';
    f.entries.forEach(function (e) {
      html += '<div class="fitness-entry"><span class="fitness-entry-date">' + e.date + '</span> ' + e.weight + ' ' + f.unit + '</div>';
    });
    html += '</div>';

    container.innerHTML = html;
  }

  /* ===== RENDER: BIRTHDAYS ===== */

  function renderBirthdays(container) {
    var html = '<div class="sec-hdr"><svg viewBox="0 0 24 24" fill="none" stroke="var(--wn)" stroke-width="2"><path d="M20 21v-2a4 4 0 0 0-4-4H8a4 4 0 0 0-4 4v2"/><circle cx="12" cy="7" r="4"/></svg> Birthdays &amp; celebrations</div>';

    html += '<div class="birthday-list">';
    birthdayData.forEach(function (b) {
      var cls = b.daysUntil <= 7 ? 'soon' : 'upcoming';
      html += '<div class="birthday-card">';
      html += '<div class="birthday-avatar" style="background:' + b.color + '">' + b.name.charAt(0) + '</div>';
      html += '<div class="birthday-info">';
      html += '<div class="birthday-name">' + b.name + '</div>';
      html += '<div class="birthday-date">' + b.date + '</div>';
      html += '</div>';
      html += '<div class="birthday-days ' + cls + '">' + b.daysUntil + 'd</div>';
      html += '</div>';
    });
    html += '</div>';

    container.innerHTML = html;
  }

  /* ===== RENDER: BUDGET ===== */

  function renderBudget(container) {
    var b = budgetData;
    var pct = Math.min(Math.round(b.spent / b.monthly * 100), 100);
    var remaining = b.monthly - b.spent;
    var remClass = remaining > 500 ? 'ok' : remaining > 0 ? 'warning' : 'over';

    var html = '<div class="sec-hdr"><svg viewBox="0 0 24 24" fill="none" stroke="var(--ac)" stroke-width="2"><line x1="12" y1="1" x2="12" y2="23"/><path d="M17 5H9.5a3.5 3.5 0 0 0 0 7h5a3.5 3.5 0 0 1 0 7H6"/></svg> Monthly budget</div>';

    html += '<div class="budget-main">';
    html += '<div class="budget-ring"><svg viewBox="0 0 80 80">';
    html += '<circle class="budget-ring-bg" cx="40" cy="40" r="34"/>';
    var circ = 2 * Math.PI * 34;
    var offset = circ - (pct / 100) * circ;
    html += '<circle class="budget-ring-fill" cx="40" cy="40" r="34" stroke-dasharray="' + circ.toFixed(1) + '" stroke-dashoffset="' + offset.toFixed(1) + '"/>';
    html += '</svg>';
    html += '<div class="budget-ring-text"><span class="budget-ring-pct">' + pct + '%</span><span class="budget-ring-label">spent</span></div>';
    html += '</div>';

    html += '<div class="budget-summary">';
    html += '<div class="budget-spent">\u20ac' + b.spent.toLocaleString() + '</div>';
    html += '<div class="budget-total">of \u20ac' + b.monthly.toLocaleString() + ' monthly</div>';
    html += '<div class="budget-remaining ' + remClass + '">\u20ac' + Math.abs(remaining) + ' ' + (remaining >= 0 ? 'remaining' : 'over budget') + '</div>';
    html += '</div></div>';

    html += '<div class="budget-cats">';
    b.categories.forEach(function (cat) {
      var catPct = Math.min(Math.round(cat.spent / cat.budget * 100), 100);
      var over = cat.spent > cat.budget;
      html += '<div class="budget-cat' + (over ? ' overspent' : '') + '">';
      html += '<div class="budget-cat-name">' + cat.name + '</div>';
      html += '<div class="budget-cat-bar"><div class="budget-cat-fill" style="width:' + catPct + '%;background:' + cat.color + '"></div></div>';
      html += '<div class="budget-cat-amt">\u20ac' + cat.spent + '</div>';
      html += '</div>';
    });
    html += '</div>';

    container.innerHTML = html;
  }

  /* ===== RENDER: NOTES ===== */

  function renderNotes(container) {
    var pinSvg = '<svg class="note-icon pin" viewBox="0 0 24 24" fill="currentColor" stroke="currentColor" stroke-width="1"><path d="M21.44 11.05l-9.19 9.19a6 6 0 0 1-8.49-8.49l9.19-9.19a4 4 0 0 1 5.66 5.66l-9.2 9.19a2 2 0 0 1-2.83-2.83l8.49-8.48"/></svg>';
    var starSvg = '<svg class="note-icon star" viewBox="0 0 24 24" fill="currentColor" stroke="currentColor" stroke-width="1"><polygon points="12 2 15.09 8.26 22 9.27 17 14.14 18.18 21.02 12 17.77 5.82 21.02 7 14.14 2 9.27 8.91 8.26 12 2"/></svg>';

    var html = '<div class="sec-hdr"><svg viewBox="0 0 24 24" fill="none" stroke="var(--wn)" stroke-width="2"><path d="M11 4H4a2 2 0 0 0-2 2v14a2 2 0 0 0 2 2h14a2 2 0 0 0 2-2v-7"/><path d="M18.5 2.5a2.121 2.121 0 0 1 3 3L12 15l-4 1 1-4 9.5-9.5z"/></svg> Quick notes</div>';

    html += '<div class="notes-list">';
    notesData.forEach(function (n) {
      var cls = 'note-card';
      if (n.pinned) cls += ' pinned';
      if (n.starred) cls += ' starred';
      html += '<div class="' + cls + '">';
      if (n.pinned || n.starred) {
        html += '<div class="note-icons">';
        if (n.pinned) html += pinSvg;
        if (n.starred) html += starSvg;
        html += '</div>';
      }
      html += '<div class="note-title">' + n.title + '</div>';
      html += '<div class="note-preview">' + n.preview + '</div>';
      html += '<div class="note-date">' + n.date + '</div>';
      html += '</div>';
    });
    html += '</div>';

    container.innerHTML = html;
  }

  /* ===== REGISTER ===== */

  if (typeof VaultWidgetRegistry !== 'undefined') {
    VaultWidgetRegistry.registerType('water-health', {
      label: 'Water & health',
      icon: '<svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><path d="M12 2.69l5.66 5.66a8 8 0 1 1-11.31 0z"/></svg>',
      category: 'lifestyle',
      defaultConfig: {},
      render: renderWaterHealth
    });

    VaultWidgetRegistry.registerType('fitness-weight', {
      label: 'Fitness & weight',
      icon: '<svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><path d="M18 20V10M12 20V4M6 20v-6"/></svg>',
      category: 'lifestyle',
      defaultConfig: {},
      render: renderFitnessWeight
    });

    VaultWidgetRegistry.registerType('birthdays', {
      label: 'Birthdays & celebrations',
      icon: '<svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><path d="M20 21v-2a4 4 0 0 0-4-4H8a4 4 0 0 0-4 4v2"/><circle cx="12" cy="7" r="4"/></svg>',
      category: 'lifestyle',
      defaultConfig: {},
      render: renderBirthdays
    });

    VaultWidgetRegistry.registerType('budget', {
      label: 'Monthly budget',
      icon: '<svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><line x1="12" y1="1" x2="12" y2="23"/><path d="M17 5H9.5a3.5 3.5 0 0 0 0 7h5a3.5 3.5 0 0 1 0 7H6"/></svg>',
      category: 'lifestyle',
      defaultConfig: {},
      render: renderBudget
    });

    VaultWidgetRegistry.registerType('notes', {
      label: 'Quick notes',
      icon: '<svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><path d="M11 4H4a2 2 0 0 0-2 2v14a2 2 0 0 0 2 2h14a2 2 0 0 0 2-2v-7"/><path d="M18.5 2.5a2.121 2.121 0 0 1 3 3L12 15l-4 1 1-4 9.5-9.5z"/></svg>',
      category: 'lifestyle',
      defaultConfig: {},
      render: renderNotes
    });
  }

})();
