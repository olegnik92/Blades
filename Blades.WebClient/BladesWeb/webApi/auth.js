"use strict";
var tokenInfo_1 = require("./tokenInfo");
var xhr_1 = require("./xhr");
var Auth = (function () {
    function Auth(accessTokenInfoItemKey) {
        this.accessTokenPath = '/token';
        this.accessTokenInfoItemKey = accessTokenInfoItemKey;
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
    Auth.prototype.addAccessTokenToRequest = function (xhr) {
        var tokenInfoJson = localStorage.getItem(this.accessTokenInfoItemKey);
        if (!tokenInfoJson) {
            return xhr;
        }
        var tokenInfo = tokenInfo_1.TokenInfo.fromJson(tokenInfoJson);
        xhr.setRequestHeader('Authorization', "Bearer " + tokenInfo.token);
        return xhr;
    };
    Auth.prototype.authorize = function (login, password) {
        var _this = this;
        return this.requestNewAccessToken(login, password).then(function (tokenInfo) {
            localStorage.setItem(_this.accessTokenInfoItemKey, tokenInfo.toJson());
            return tokenInfo;
        });
    };
    Auth.prototype.clearTokenInfo = function () {
        localStorage.removeItem(this.accessTokenInfoItemKey);
    };
    return Auth;
}());
exports.Auth = Auth;
exports.auth = new Auth('accessTokenInfo');
//# sourceMappingURL=auth.js.map