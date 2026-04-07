/* ============================================
   VAULT WIDGET REGISTRY
   Register widget types, manage instances,
   persist configuration via VaultStore
   ============================================ */
var VaultWidgetRegistry = (function () {
  'use strict';

  /* Widget type definitions: key -> { label, icon, category, defaultConfig, render, configSchema } */
  var definitions = {};

  /* Active widget instances on dashboard (ordered array)
     Each: { id, type, config, visible } */
  var instances = [];

  /* ---------- Type registration ---------- */

  function registerType(key, definition) {
    definitions[key] = {
      key: key,
      label: definition.label || key,
      icon: definition.icon || '',
      category: definition.category || 'general',
      defaultConfig: definition.defaultConfig || {},
      render: definition.render || null,
      configSchema: definition.configSchema || null
    };
  }

  function getType(key) {
    return definitions[key] || null;
  }

  function getAllTypes() {
    return Object.keys(definitions).map(function (k) { return definitions[k]; });
  }

  /* ---------- Instance management ---------- */

  function addInstance(type, config) {
    var def = definitions[type];
    if (!def) return null;
    var id = type + '-' + Date.now();
    var instance = {
      id: id,
      type: type,
      config: Object.assign({}, def.defaultConfig, config || {}),
      visible: true
    };
    instances.push(instance);
    persist();
    return instance;
  }

  function removeInstance(id) {
    instances = instances.filter(function (inst) { return inst.id !== id; });
    persist();
  }

  function reorderInstances(orderedIds) {
    var map = {};
    instances.forEach(function (inst) { map[inst.id] = inst; });
    instances = orderedIds.map(function (id) { return map[id]; }).filter(Boolean);
    persist();
  }

  function toggleInstance(id, visible) {
    instances = instances.map(function (inst) {
      if (inst.id !== id) return inst;
      return Object.assign({}, inst, { visible: visible });
    });
    persist();
  }

  function updateConfig(id, newConfig) {
    instances = instances.map(function (inst) {
      if (inst.id !== id) return inst;
      return Object.assign({}, inst, { config: Object.assign({}, inst.config, newConfig) });
    });
    persist();
  }

  function getInstance(id) {
    for (var i = 0; i < instances.length; i++) {
      if (instances[i].id === id) return instances[i];
    }
    return null;
  }

  function getInstances() {
    return instances.slice();
  }

  function getVisibleInstances() {
    return instances.filter(function (inst) { return inst.visible; });
  }

  /* ---------- Rendering ---------- */

  function renderInstance(id, container) {
    var inst = getInstance(id);
    if (!inst) return;
    var def = definitions[inst.type];
    if (!def || typeof def.render !== 'function') return;
    def.render(container, inst.config, inst.id);
  }

  function renderAll(parentContainer) {
    instances.forEach(function (inst) {
      var def = definitions[inst.type];
      if (!def || typeof def.render !== 'function') return;

      var sec = parentContainer.querySelector('.sec[data-widget-id="' + inst.id + '"]');
      if (!sec) {
        sec = parentContainer.querySelector('.sec[data-widget="' + inst.type + '"]:not([data-widget-id])');
        if (sec) {
          sec.setAttribute('data-widget-id', inst.id);
        }
      }
      if (!sec) {
        sec = document.createElement('div');
        sec.className = 'sec';
        sec.setAttribute('data-widget-id', inst.id);
        sec.setAttribute('data-widget', inst.type);
        parentContainer.appendChild(sec);
      }

      if (!inst.visible) {
        sec.classList.add('vg-hidden');
      } else {
        sec.classList.remove('vg-hidden');
      }

      def.render(sec, inst.config, inst.id);
    });
  }

  /* ---------- Persistence ---------- */

  function persist() {
    if (typeof VaultStore !== 'undefined') {
      VaultStore.set('widget_instances', instances);
    }
  }

  function load() {
    if (typeof VaultStore === 'undefined') return;
    var saved = VaultStore.get('widget_instances', null);
    if (saved && Array.isArray(saved) && saved.length > 0) {
      instances = saved;
    }
  }

  /* ---------- Seeding defaults ---------- */

  function seedDefaults(defaultTypes) {
    if (instances.length > 0) return; // Already have instances
    defaultTypes.forEach(function (type) {
      var def = definitions[type];
      if (!def) return;
      instances.push({
        id: type + '-default',
        type: type,
        config: Object.assign({}, def.defaultConfig),
        visible: true
      });
    });
    persist();
  }

  /* ---------- Migration helper ---------- */
  /* Converts legacy data-widget="key" sections to registry instances */
  function migrateFromDOM(container) {
    if (instances.length > 0) return; // Already migrated
    var sections = container.querySelectorAll('.sec[data-widget]');
    var migrated = [];
    sections.forEach(function (sec) {
      var type = sec.getAttribute('data-widget');
      if (!type) return;
      var id = type + '-default';
      sec.setAttribute('data-widget-id', id);
      migrated.push({
        id: id,
        type: type,
        config: {},
        visible: !sec.classList.contains('vg-hidden')
      });
    });
    if (migrated.length > 0) {
      instances = migrated;
      persist();
    }
  }

  return {
    registerType: registerType,
    getType: getType,
    getAllTypes: getAllTypes,
    addInstance: addInstance,
    removeInstance: removeInstance,
    reorderInstances: reorderInstances,
    toggleInstance: toggleInstance,
    updateConfig: updateConfig,
    getInstance: getInstance,
    getInstances: getInstances,
    getVisibleInstances: getVisibleInstances,
    renderInstance: renderInstance,
    renderAll: renderAll,
    load: load,
    persist: persist,
    seedDefaults: seedDefaults,
    migrateFromDOM: migrateFromDOM
  };
})();
