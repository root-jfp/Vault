/* ============================================
   VAULT TASKS — TaskStore
   CRUD + filter helpers on top of VaultStore.
   Shared by all pages that work with tasks.
   ============================================ */
var TaskStore = (function () {
  'use strict';

  var TASKS_KEY  = 'tasks';
  var COLS_KEY   = 'task_columns';
  var USER_KEY   = 'current_user';
  var ID_KEY     = 'task_next_id';

  var DEFAULT_COLUMNS = [
    {id:1, name:'To Do',       color:'#E24B4A', position:0},
    {id:2, name:'In Progress', color:'#EF9F27', position:1},
    {id:3, name:'Review',      color:'#378ADD', position:2},
    {id:4, name:'Done',        color:'#1D9E75', position:3}
  ];

  /* Seed tasks use relative dates so overdue/today/upcoming always make sense */
  function _makeSeed() {
    return [
      {id:1,  title:'Notion template tutorial',       description:'Record first Notion template video and publish to YouTube',                  columnId:1, position:0, priority:'high',   label:'Content creation', labelColor:'#378ADD', dueDate:_off(0),  projectId:1, projectName:'Notion template',   userId:1},
      {id:2,  title:'Email newsletter draft',         description:'Write weekly email content for subscribers about productivity tips',        columnId:1, position:1, priority:'medium', label:'Content creation', labelColor:'#378ADD', dueDate:_off(2),  projectId:2, projectName:'Content creation',  userId:1},
      {id:3,  title:'Design landing page mockup',     description:'Create a Figma mockup for the Notion template sales page',                  columnId:1, position:2, priority:'low',    label:'Design',           labelColor:'#7F77DD', dueDate:_off(6),  projectId:1, projectName:'Notion template',   userId:1},
      {id:4,  title:'Research competitor templates',  description:'Analyze top 10 Notion templates on Gumroad for pricing and features',       columnId:1, position:3, priority:'medium', label:'Research',         labelColor:'#EF9F27', dueDate:_off(3),  projectId:1, projectName:'Notion template',   userId:1},
      {id:5,  title:'Learn Framer basics',            description:'Complete Framer crash course chapters 1–4',                                 columnId:2, position:0, priority:'medium', label:'Learning',         labelColor:'#1D9E75', dueDate:_off(4),  projectId:4, projectName:'Learn Framer',      userId:1},
      {id:6,  title:'Edit CapCut intro video',        description:'Add transitions, music and captions to the intro clip',                     columnId:2, position:1, priority:'high',   label:'Content creation', labelColor:'#378ADD', dueDate:_off(-1), projectId:3, projectName:'CapCut + Notion',   userId:1},
      {id:7,  title:'Fix IFS posting control',        description:'Debug the posting control module validation errors',                        columnId:2, position:2, priority:'urgent', label:'Bug',              labelColor:'#E24B4A', dueDate:_off(-2), projectId:null, projectName:null,             userId:1},
      {id:8,  title:'Study ASP.NET middleware',       description:'Read through middleware pipeline docs and create sample project',            columnId:3, position:0, priority:'medium', label:'Learning',         labelColor:'#1D9E75', dueDate:_off(1),  projectId:null, projectName:null,             userId:1},
      {id:9,  title:'Review invoice module',          description:'Code review the invoice generation module for edge cases',                   columnId:3, position:1, priority:'high',   label:'Review',           labelColor:'#378ADD', dueDate:_off(2),  projectId:null, projectName:null,             userId:1},
      {id:10, title:'Framer 2nd video script',        description:'Write script for the second Framer tutorial covering animations',            columnId:1, position:4, priority:'low',    label:'Content creation', labelColor:'#378ADD', dueDate:_off(8),  projectId:4, projectName:'Learn Framer',      userId:1},
      {id:11, title:'Set up email automation',        description:'Configure Mailchimp drip campaign for new subscribers',                     columnId:4, position:0, priority:'medium', label:'Content creation', labelColor:'#378ADD', dueDate:_off(-5), projectId:2, projectName:'Content creation',  userId:1},
      {id:12, title:'FERRO workout plan',             description:'Create weekly workout schedule and track sets/reps',                         columnId:4, position:1, priority:'low',    label:'Personal',         labelColor:'#7F77DD', dueDate:_off(-7), projectId:null, projectName:null,             userId:1},
      {id:13, title:'Content calendar April',         description:'Plan all content for April across platforms',                               columnId:4, position:2, priority:'high',   label:'Content creation', labelColor:'#378ADD', dueDate:_off(-9), projectId:2, projectName:'Content creation',  userId:1},
      {id:14, title:'CapCut advanced editing',        description:'Learn keyframing and speed ramp techniques in CapCut',                       columnId:2, position:3, priority:'low',    label:'Learning',         labelColor:'#1D9E75', dueDate:_off(5),  projectId:3, projectName:'CapCut + Notion',   userId:1}
    ];
  }

  /* ── date helpers ── */
  function _now()  { return new Date().toISOString(); }
  function _dateStr(d) {
    return d.getFullYear() + '-' +
      String(d.getMonth()+1).padStart(2,'0') + '-' +
      String(d.getDate()).padStart(2,'0');
  }
  function _off(n) {
    var d = new Date(); d.setDate(d.getDate() + n); return _dateStr(d);
  }
  function _today() { return _dateStr(new Date()); }

  /* ── ID generator ── */
  function _nextId() {
    var id = VaultStore.get(ID_KEY, 1);
    VaultStore.set(ID_KEY, id + 1);
    return id;
  }

  /* ── Auto-seed on first ever load (key missing = null) ── */
  function _init() {
    if (VaultStore.get(TASKS_KEY, null) === null) {
      var seed = _makeSeed();
      VaultStore.set(TASKS_KEY, seed);
      VaultStore.set(ID_KEY, seed.length + 1);
    }
    if (VaultStore.get(COLS_KEY, null) === null) {
      VaultStore.set(COLS_KEY, DEFAULT_COLUMNS);
    }
  }

  /* ══════════════════════════════════════════
     CRUD
     ══════════════════════════════════════════ */

  function getAll(userId) {
    _init();
    var tasks = VaultStore.get(TASKS_KEY, []);
    if (userId !== undefined && userId !== null) {
      tasks = tasks.filter(function(t) { return t.userId === userId; });
    }
    return tasks;
  }

  function get(id) {
    _init();
    var tasks = VaultStore.get(TASKS_KEY, []);
    for (var i = 0; i < tasks.length; i++) {
      if (tasks[i].id === id) return tasks[i];
    }
    return null;
  }

  function create(data) {
    _init();
    var tasks = VaultStore.get(TASKS_KEY, []);
    var task = Object.assign({}, data, {
      id:         _nextId(),
      created_at: _now(),
      updated_at: _now()
    });
    tasks.push(task);
    VaultStore.set(TASKS_KEY, tasks);
    return task;
  }

  function update(id, changes) {
    _init();
    var tasks   = VaultStore.get(TASKS_KEY, []);
    var updated = null;
    tasks = tasks.map(function(t) {
      if (t.id === id) {
        updated = Object.assign({}, t, changes, {updated_at: _now()});
        return updated;
      }
      return t;
    });
    if (!updated) return null;
    VaultStore.set(TASKS_KEY, tasks);
    return updated;
  }

  function remove(id) {
    _init();
    var tasks = VaultStore.get(TASKS_KEY, []);
    var before = tasks.length;
    tasks = tasks.filter(function(t) { return t.id !== id; });
    if (tasks.length === before) return false;
    VaultStore.set(TASKS_KEY, tasks);
    return true;
  }

  /* Move task to a new column at a given position, then reindex that column */
  function move(taskId, columnId, position) {
    var result = update(taskId, {columnId: columnId, position: position});
    if (!result) return false;
    _reindex(columnId);
    return true;
  }

  function _reindex(columnId) {
    var tasks = VaultStore.get(TASKS_KEY, []);
    var col = tasks
      .filter(function(t) { return t.columnId === columnId; })
      .sort(function(a, b) { return a.position - b.position; });
    var posMap = {};
    col.forEach(function(t, i) { posMap[t.id] = i; });
    tasks = tasks.map(function(t) {
      if (t.columnId === columnId) {
        return Object.assign({}, t, {position: posMap[t.id]});
      }
      return t;
    });
    VaultStore.set(TASKS_KEY, tasks);
  }

  /* ══════════════════════════════════════════
     FILTER HELPERS
     ══════════════════════════════════════════ */

  function getOverdue(userId) {
    var today = _today();
    return getAll(userId).filter(function(t) {
      return t.columnId !== 4 && t.dueDate && t.dueDate < today;
    });
  }

  function getDueToday(userId) {
    var today = _today();
    return getAll(userId).filter(function(t) {
      return t.columnId !== 4 && t.dueDate === today;
    });
  }

  function getUpcoming(userId, days) {
    var today = _today();
    var limit = _off(days !== undefined ? days : 7);
    return getAll(userId).filter(function(t) {
      return t.columnId !== 4 && t.dueDate && t.dueDate > today && t.dueDate <= limit;
    });
  }

  function getCompleted(userId) {
    return getAll(userId).filter(function(t) { return t.columnId === 4; });
  }

  /* ══════════════════════════════════════════
     COLUMNS
     ══════════════════════════════════════════ */

  function getColumns() {
    _init();
    return VaultStore.get(COLS_KEY, DEFAULT_COLUMNS);
  }

  function createColumn(name, color) {
    var cols  = getColumns();
    var maxId = cols.reduce(function(m, c) { return Math.max(m, c.id); }, 0);
    var col   = {id: maxId + 1, name: name, color: color || '#6e6d68', position: cols.length};
    cols.push(col);
    VaultStore.set(COLS_KEY, cols);
    return col;
  }

  /* ══════════════════════════════════════════
     USER
     ══════════════════════════════════════════ */

  function getCurrentUser() { return VaultStore.get(USER_KEY, 1); }
  function setCurrentUser(id) { VaultStore.set(USER_KEY, id); }

  /* ══════════════════════════════════════════
     RESET (testing / fresh start)
     Sets tasks to [] — _init() won't reseed because key now exists.
     ══════════════════════════════════════════ */
  function reset() {
    VaultStore.set(TASKS_KEY, []);
    VaultStore.set(COLS_KEY, DEFAULT_COLUMNS);
    VaultStore.set(ID_KEY, 1);
  }

  return {
    getAll:         getAll,
    get:            get,
    create:         create,
    update:         update,
    'delete':       remove,
    move:           move,
    getOverdue:     getOverdue,
    getDueToday:    getDueToday,
    getUpcoming:    getUpcoming,
    getCompleted:   getCompleted,
    getColumns:     getColumns,
    createColumn:   createColumn,
    getCurrentUser: getCurrentUser,
    setCurrentUser: setCurrentUser,
    reset:          reset
  };

})();
