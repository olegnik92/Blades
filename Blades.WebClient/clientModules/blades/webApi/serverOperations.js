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
var xhr_1 = require("./xhr");
var serverConnection_1 = require("./serverConnection");
var json_1 = require("../tools/json");
var BaseOperation = (function () {
    function BaseOperation(name, data, requestType, requestMethod) {
        this.requestUrl = '/api/operation';
        this.name = name;
        this.data = data;
        this.requestMethod = requestMethod;
        this.requestType = requestType;
    }
    BaseOperation.prototype.createXhr = function () {
        var xhr = new xhr_1.default(this.requestUrl, this.requestMethod, this.data);
        xhr.setRequestHeader('x-blades-operation-name', this.name);
        xhr.setRequestHeader('x-blades-operation-request-type', this.requestType);
        return xhr;
    };
    return BaseOperation;
}());
exports.default = BaseOperation;
var JsonOperation = (function (_super) {
    __extends(JsonOperation, _super);
    function JsonOperation(name, data) {
        var _this = this;
        var strData = json_1.default.stringify(data);
        _this = _super.call(this, name, strData, 'json', 'POST') || this;
        return _this;
    }
    JsonOperation.prototype.execute = function () {
        var xhr = this.createXhr();
        xhr.setRequestHeader('Content-Type', 'application/json; charset=utf-8');
        return xhr.execute();
    };
    JsonOperation.prototype.executeViaConnection = function () {
        var message = {
            name: this.name,
            requestType: this.requestType,
            data: this.data
        };
        var messageStr = this.requestType + "@@@@@" + this.name + "@@@@@" + this.data;
        serverConnection_1.default.send(messageStr);
    };
    return JsonOperation;
}(BaseOperation));
exports.JsonOperation = JsonOperation;
var FormDataOperation = (function (_super) {
    __extends(FormDataOperation, _super);
    function FormDataOperation(name, data) {
        return _super.call(this, name, data, 'FormDataOperation', 'POST') || this;
    }
    return FormDataOperation;
}(BaseOperation));
exports.FormDataOperation = FormDataOperation;
//# sourceMappingURL=serverOperations.js.map