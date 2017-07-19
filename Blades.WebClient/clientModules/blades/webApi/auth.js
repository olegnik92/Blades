"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var tokenInfo_1 = require("./tokenInfo");
var xhr_1 = require("./xhr");
var syncActionsChain_1 = require("../tools/syncActionsChain");
var cookieStorage_1 = require("../storages/cookieStorage");
var Auth = (function () {
    function Auth(accessTokenKey, storage) {
        this.accessTokenKey = accessTokenKey;
        this.storage = storage;
        this.accessTokenApiPath = '/token';
        this.tokenInfoChanged = new syncActionsChain_1.default();
    }
    Auth.prototype.requestNewAccessToken = function (login, password) {
        var data = "grant_type=password&username=" + login + "&password=" + password;
        var date = new Date();
        var xhr = new xhr_1.default(this.accessTokenApiPath, 'POST', data);
        xhr.setRequestHeader('Content-Type', 'application/x-www-form-urlencoded; charset=utf-8');
        return xhr.execute().then(function (result) {
            var info = new tokenInfo_1.default();
            info.token = result.access_token;
            info.login = login;
            date.setSeconds(date.getSeconds() + result.expires_in);
            info.expireDate = date;
            return info;
        });
    };
    Object.defineProperty(Auth.prototype, "accessTokenInfoKey", {
        get: function () {
            return this.accessTokenKey + "__tokenInfo";
        },
        enumerable: true,
        configurable: true
    });
    Auth.prototype.getTokenInfo = function () {
        var tokenInfo = this.storage.get(this.accessTokenInfoKey);
        if (!tokenInfo) {
            return null;
        }
        return tokenInfo;
    };
    Auth.prototype.addAccessTokenToRequestHeader = function (xhr) {
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
            _this.storage.set(_this.accessTokenInfoKey, tokenInfo, tokenInfo.expireDate);
            _this.storage.setStr(_this.accessTokenKey, tokenInfo.token, tokenInfo.expireDate);
            _this.tokenInfoChanged.run(tokenInfo);
            return tokenInfo;
        });
    };
    Auth.prototype.clearTokenInfo = function () {
        this.storage.remove(this.accessTokenInfoKey);
        this.storage.remove(this.accessTokenKey);
        this.tokenInfoChanged.run(null);
    };
    Auth.prototype.setStorage = function (storage) {
        this.storage = storage;
    };
    return Auth;
}());
exports.Auth = Auth;
;
var auth = new Auth('accessToken', cookieStorage_1.default);
exports.default = auth;
//# sourceMappingURL=auth.js.map