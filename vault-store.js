/* ============================================
   VAULT STORE — localStorage persistence layer
   Prefixed key/value with JSON serialization
   ============================================ */
var VaultStore = (function () {
  'use strict';

  var PREFIX = 'vault_';

  function get(key, defaultValue) {
    try {
      var raw = localStorage.getItem(PREFIX + key);
      return raw !== null ? JSON.parse(raw) : defaultValue;
    } catch (e) {
      return defaultValue;
    }
  }

  function set(key, value) {
    try {
      localStorage.setItem(PREFIX + key, JSON.stringify(value));
    } catch (e) {
      /* Storage full or unavailable */
    }
  }

  function remove(key) {
    localStorage.removeItem(PREFIX + key);
  }

  function keys() {
    var result = [];
    for (var i = 0; i < localStorage.length; i++) {
      var k = localStorage.key(i);
      if (k && k.indexOf(PREFIX) === 0) {
        result.push(k.substring(PREFIX.length));
      }
    }
    return result;
  }

  return { get: get, set: set, remove: remove, keys: keys };
})();
