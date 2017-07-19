"use strict";
var __extends = (this && this.__extends) || (function () {
    var extendStatics = Object.setPrototypeOf ||
        ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
        function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
Object.defineProperty(exports, "__esModule", { value: true });
var json_1 = require("../tools/json");
var StoreItem = (function () {
    function StoreItem(data, expire) {
        this.data = data;
        this.expire = expire;
    }
    return StoreItem;
}());
;
function expireValueKey(key) {
    return key + "__expire";
}
;
function getExpireDateNumber(expire) {
    if (typeof expire === 'number') {
        return Date.now() + expire;
    }
    else if (expire.getTime) {
        return expire.getTime();
    }
    return 0;
}
;
function isShouldRemove(expire) {
    return expire !== undefined && expire !== 0 && expire < Date.now();
}
;
var BrowserStorage = (function () {
    function BrowserStorage(storage) {
        this.storage = storage;
    }
    BrowserStorage.prototype.set = function (key, value, expire) {
        if (typeof value !== 'object') {
            return;
        }
        var storeItem = new StoreItem(value);
        if (expire) {
            storeItem.expire = getExpireDateNumber(expire);
        }
        this.storage.setItem(key, json_1.default.stringify(storeItem));
    };
    BrowserStorage.prototype.get = function (key) {
        var itemStr = this.storage.getItem(key);
        if (!itemStr) {
            return null;
        }
        var storeItem = json_1.default.parse(itemStr);
        if (typeof storeItem !== 'object') {
            return null;
        }
        if (isShouldRemove(storeItem.expire)) {
            this.storage.removeItem(key);
            return null;
        }
        return storeItem.data;
    };
    BrowserStorage.prototype.setStr = function (key, value, expire) {
        if (typeof value !== 'string') {
            return;
        }
        this.storage.setItem(key, value);
        if (expire) {
            this.storage.setItem(expireValueKey(key), getExpireDateNumber(expire).toString());
        }
    };
    BrowserStorage.prototype.getStr = function (key) {
        var expire = parseInt(this.storage.getItem(expireValueKey(key)) || '');
        if (isShouldRemove(expire)) {
            this.storage.removeItem(expireValueKey(key));
            this.storage.removeItem(key);
            return null;
        }
        return this.storage.getItem(key);
    };
    BrowserStorage.prototype.remove = function (key) {
        this.storage.removeItem(key);
        this.storage.removeItem(expireValueKey(key));
    };
    return BrowserStorage;
}());
;
exports.default = BrowserStorage;
var SessionStorage = (function (_super) {
    __extends(SessionStorage, _super);
    function SessionStorage() {
        return _super.call(this, window.sessionStorage) || this;
    }
    return SessionStorage;
}(BrowserStorage));
exports.SessionStorage = SessionStorage;
exports.sessionStorage = new SessionStorage();
var LocalStorage = (function (_super) {
    __extends(LocalStorage, _super);
    function LocalStorage() {
        return _super.call(this, window.localStorage) || this;
    }
    return LocalStorage;
}(BrowserStorage));
exports.LocalStorage = LocalStorage;
exports.localStorage = new LocalStorage();
//# sourceMappingURL=browserStorage.js.map