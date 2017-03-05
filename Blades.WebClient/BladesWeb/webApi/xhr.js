"use strict";
var es6_promise_1 = require("es6-promise");
var json_1 = require("../tools/json");
var RequestExecutionError_1 = require("./RequestExecutionError");
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
        return new es6_promise_1.Promise(function (res, rej) {
            _this.xhr.send(_this.reqBody);
            _this.xhr.onreadystatechange = (function () {
                if (_this.xhr.readyState !== XMLHttpRequest.DONE) {
                    return;
                }
                res(_this);
            });
        });
    };
    Xhr.beforeExecution = function (hook) {
        var next = Xhr.beforeExecutionChain;
        Xhr.beforeExecutionChain = (function (xhr) { return hook(xhr).then(next); });
    };
    Xhr.afterExecution = function (hook) {
        var next = Xhr.afterExecutionChain;
        Xhr.afterExecutionChain = (function (xhr) { return hook(xhr).then(next); });
    };
    Xhr.createResData = function (xhr) {
        if (xhr.readyState !== XMLHttpRequest.DONE) {
            return null;
        }
        var data = null;
        try {
            data = json_1.json.parse(xhr.responseText);
        }
        catch (e) {
            console.error(e);
        }
        return data;
    };
    Xhr.prototype.execute = function () {
        var _this = this;
        return new es6_promise_1.Promise(function (res, rej) {
            Xhr.beforeExecutionChain(_this)
                .then(function (xhr) { return xhr.rawExecute(); })
                .then(function (xhr) { return Xhr.afterExecutionChain(xhr); })
                .then(function (xhr) {
                if (xhr.status !== 200) {
                    rej(new RequestExecutionError_1.RequestExecutionError(xhr));
                    return;
                }
                var data = Xhr.createResData(xhr);
                res(data);
            })
                .catch(function (err) { return rej(new RequestExecutionError_1.RequestExecutionError(err)); });
        });
    };
    return Xhr;
}());
Xhr.beforeExecutionChain = function (xhr) { return new es6_promise_1.Promise(function (res) { return res(xhr); }); };
Xhr.afterExecutionChain = function (xhr) { return new es6_promise_1.Promise(function (res) { return res(xhr); }); };
exports.Xhr = Xhr;
//# sourceMappingURL=xhr.js.map