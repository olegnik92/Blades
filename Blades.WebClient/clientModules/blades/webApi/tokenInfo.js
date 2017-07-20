"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var json_1 = require("../tools/json");
var TokenInfo = (function () {
    function TokenInfo() {
    }
    TokenInfo.fromJson = function (jsonStr) {
        var obj = json_1.default.parse(jsonStr);
        if (!obj) {
            return null;
        }
        var result = new TokenInfo();
        result.login = obj.login;
        result.token = obj.token;
        result.expireDate = new Date(obj.expireDate);
        return result;
    };
    return TokenInfo;
}());
exports.default = TokenInfo;
