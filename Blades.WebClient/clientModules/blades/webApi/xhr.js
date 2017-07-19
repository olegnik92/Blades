"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var json_1 = require("../tools/json");
var RequestExecutionError_1 = require("./RequestExecutionError");
var asyncActionsChain_1 = require("../tools/asyncActionsChain");
var Xhr = (function () {
    function Xhr(url, method, body) {
        if (method === void 0) { method = 'GET'; }
        this.xhr = new XMLHttpRequest();
        this.reqUrl = url;
        this.reqMethod = method;
        this.reqBody = body;
        this.xhr.open(this.reqMethod, this.reqUrl, true);
    }
    Object.defineProperty(Xhr.prototype, "innerXhr", {
        get: function () {
            return this.xhr;
        },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(Xhr.prototype, "status", {
        get: function () {
            return this.xhr.status;
        },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(Xhr.prototype, "statusText", {
        get: function () {
            return this.xhr.statusText;
        },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(Xhr.prototype, "response", {
        get: function () {
            return this.xhr.response;
        },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(Xhr.prototype, "responseText", {
        get: function () {
            return this.xhr.responseText;
        },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(Xhr.prototype, "readyState", {
        get: function () {
            return this.xhr.readyState;
        },
        enumerable: true,
        configurable: true
    });
    Xhr.prototype.setRequestHeader = function (header, value) {
        this.xhr.setRequestHeader(header, value);
    };
    Xhr.prototype.rawExecute = function () {
        var _this = this;
        return new Promise(function (res, rej) {
            _this.xhr.send(_this.reqBody);
            _this.xhr.onreadystatechange = (function () {
                if (_this.xhr.readyState !== XMLHttpRequest.DONE) {
                    return;
                }
                res(_this);
            });
            _this.xhr.onerror = (function (ev) {
                rej(_this);
            });
        });
    };
    Object.defineProperty(Xhr, "beforeExecChain", {
        get: function () {
            return Xhr.beforeExecutionChain;
        },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(Xhr, "afterExecChain", {
        get: function () {
            return Xhr.afterExecutionChain;
        },
        enumerable: true,
        configurable: true
    });
    Xhr.createResData = function (xhr) {
        if (xhr.readyState !== XMLHttpRequest.DONE) {
            throw new Error('Try to parse XHR in incorrect state');
        }
        var data = json_1.default.parse(xhr.responseText);
        return data;
    };
    Xhr.prototype.execute = function () {
        var _this = this;
        return new Promise(function (res, rej) {
            Xhr.beforeExecutionChain.run(_this)
                .then(function (xhr) { return xhr.rawExecute(); })
                .then(function (xhr) { return Xhr.afterExecutionChain.run(xhr); })
                .then(function (xhr) {
                if (xhr.status !== 200) {
                    rej(new RequestExecutionError_1.default(xhr));
                    return;
                }
                var data = Xhr.createResData(xhr);
                res(data);
            })
                .catch(function (err) { return rej(new RequestExecutionError_1.default(err)); });
        });
    };
    Xhr.beforeExecutionChain = new asyncActionsChain_1.default();
    Xhr.afterExecutionChain = new asyncActionsChain_1.default();
    return Xhr;
}());
exports.default = Xhr;
//# sourceMappingURL=xhr.js.map