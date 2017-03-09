"use strict";
var tokenInfo_1 = require("./tokenInfo");
var xhr_1 = require("./xhr");
var noop_1 = require("../tools/noop");
var cookie_1 = require("../tools/cookie");
var Auth = (function () {
    function Auth(accessTokenInfoStorageKey, accessTokenCookieName) {
        this.accessTokenPath = '/token';
        this.tokenInfoChangedHandlers = [];
        this.accessTokenInfoStorageKey = accessTokenInfoStorageKey;
        this.accessTokenCookieName = accessTokenCookieName;
    }
    Auth.prototype.requestNewAccessToken = function (login, password) {
        var data = "grant_type=password&username=" + login + "&password=" + password;
        var date = new Date();
        var xhr = new xhr_1.Xhr(this.accessTokenPath, 'POST', data);
        xhr.setRequestHeader('Content-Type', 'application/x-www-form-urlencoded; charset=utf-8');
        return xhr.execute().then(function (result) {
            var info = new tokenInfo_1.TokenInfo();
            info.token = result.access_token;
            info.login = login;
            date.setSeconds(date.getSeconds() + result.expires_in);
            info.expireDate = date;
            return info;
        });
    };
    Auth.prototype.getTokenInfo = function () {
        var tokenInfoJson = localStorage.getItem(this.accessTokenInfoStorageKey);
        if (!tokenInfoJson) {
            return null;
        }
        var tokenInfo = tokenInfo_1.TokenInfo.fromJson(tokenInfoJson);
        return tokenInfo;
    };
    Auth.prototype.addAccessTokenToRequest = function (xhr) {
        var tokenInfo = this.getTokenInfo();
        if (!tokenInfo) {
            return xhr;
        }
        xhr.setRequestHeader('Authorization', "Bearer " + tokenInfo.token);
        return xhr;
    };
    Auth.prototype.authorize = function (login, password) {
        var _this = this;
        return this.requestNewAccessToken(login, password).then(function (tokenInfo) {
            localStorage.setItem(_this.accessTokenInfoStorageKey, tokenInfo.toJson());
            cookie_1.cookie.setCookie(_this.accessTokenCookieName, tokenInfo.token, new cookie_1.CookieOptions(tokenInfo.expireDate));
            _this.tokenInfoChanged(tokenInfo);
            return tokenInfo;
        });
    };
    Auth.prototype.clearTokenInfo = function () {
        localStorage.removeItem(this.accessTokenInfoStorageKey);
        this.tokenInfoChanged(null);
    };
    Auth.prototype.onTokenInfoChanged = function (handler) {
        var _this = this;
        if (!handler) {
            return noop_1.noop;
        }
        this.tokenInfoChangedHandlers.push(handler);
        return function () {
            var index = _this.tokenInfoChangedHandlers.indexOf(handler);
            if (index > -1) {
                _this.tokenInfoChangedHandlers.splice(index, 1);
            }
        };
    };
    Auth.prototype.tokenInfoChanged = function (newInfo) {
        this.tokenInfoChangedHandlers.forEach(function (handler) { return handler(newInfo); });
    };
    return Auth;
}());
exports.Auth = Auth;
exports.auth = new Auth('accessTokenInfo', 'accessToken');
//# sourceMappingURL=auth.js.map