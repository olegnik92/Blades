"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var cookie_1 = require("../tools/cookie");
var json_1 = require("../tools/json");
var CookieStorage = (function () {
    function CookieStorage() {
    }
    CookieStorage.prototype.set = function (key, value, expire) {
        if (typeof value !== 'object') {
            return;
        }
        var options = new cookie_1.CookieOptions(expire);
        cookie_1.default.setCookie(key, json_1.default.stringify(value), options);
    };
    CookieStorage.prototype.get = function (key) {
        var itemStr = cookie_1.default.getCookie(key);
        if (!itemStr) {
            return null;
        }
        var storeItem = json_1.default.parse(itemStr);
        if (typeof storeItem !== 'object') {
            return null;
        }
        return storeItem;
    };
    CookieStorage.prototype.setStr = function (key, value, expire) {
        if (typeof value !== 'string') {
            return;
        }
        var options = new cookie_1.CookieOptions(expire);
        cookie_1.default.setCookie(key, value, options);
    };
    CookieStorage.prototype.getStr = function (key) {
        var itemStr = cookie_1.default.getCookie(key);
        if (typeof itemStr !== 'string') {
            return null;
        }
        return itemStr;
    };
    CookieStorage.prototype.remove = function (key) {
        cookie_1.default.deleteCookie(key);
    };
    return CookieStorage;
}());
exports.CookieStorage = CookieStorage;
;
var cookieStorage = new CookieStorage();
exports.default = cookieStorage;
//# sourceMappingURL=cookieStorage.js.map