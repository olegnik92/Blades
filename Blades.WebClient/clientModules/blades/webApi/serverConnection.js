"use strict";
var json_1 = require("../tools/json");
var noop_1 = require("../tools/noop");
var ServerConnection = (function () {
    function ServerConnection() {
        this.connectionProtocol = 'ws';
        this.connectionRoute = 'ws';
        this.reconnectTimeout = 15000;
        this.reconnectIntervalRef = 0;
        this.sendMessagesTimeout = 100;
        this.sendMessagesIntervalRef = 0;
        this.messagesQueue = [];
        this.rawMessageHandlers = [];
        this.notifyMessageHandlers = {};
    }
    ServerConnection.prototype.open = function () {
        var _this = this;
        if (this.isOpen) {
            return;
        }
        this.connect();
        clearInterval(this.reconnectIntervalRef);
        this.reconnectIntervalRef = setInterval(function () {
            if (!_this.ws || _this.ws.readyState !== WebSocket.OPEN) {
                _this.connect();
            }
        }, this.reconnectTimeout);
    };
    ServerConnection.prototype.close = function () {
        if (this.ws) {
            this.ws.close();
        }
        clearInterval(this.reconnectIntervalRef);
    };
    Object.defineProperty(ServerConnection.prototype, "isOpen", {
        get: function () {
            if (!this.ws) {
                return false;
            }
            return this.ws.readyState === WebSocket.OPEN;
        },
        enumerable: true,
        configurable: true
    });
    ServerConnection.prototype.connect = function () {
        var _this = this;
        this.ws = new WebSocket(this.connectionProtocol + "://" + document.location.host + "/" + this.connectionRoute);
        this.bindEvents();
        clearInterval(this.sendMessagesIntervalRef);
        this.sendMessagesIntervalRef = setInterval(function () {
            _this.sendMessagesFromQueue();
        }, this.sendMessagesTimeout);
    };
    ServerConnection.prototype.bindEvents = function () {
        var _this = this;
        this.ws.onclose = function () {
            clearInterval(_this.sendMessagesIntervalRef);
        };
        this.ws.onmessage = function (ev) {
            _this.rawMessage(ev);
            _this.processNotifyMessage(ev);
        };
    };
    ServerConnection.prototype.sendMessagesFromQueue = function () {
        while (this.messagesQueue.length > 0) {
            if (this.ws.readyState !== WebSocket.OPEN) {
                break;
            }
            var data = this.messagesQueue.shift();
            this.ws.send(data);
        }
    };
    ServerConnection.prototype.send = function (data) {
        this.messagesQueue.push(data);
    };
    ServerConnection.prototype.rawMessage = function (e) {
        this.rawMessageHandlers.forEach(function (handler) { return handler(e); });
    };
    ServerConnection.prototype.onRawMessage = function (handler) {
        var _this = this;
        if (!handler) {
            return noop_1.default;
        }
        this.rawMessageHandlers.push(handler);
        return function () {
            var index = _this.rawMessageHandlers.indexOf(handler);
            if (index > -1) {
                _this.rawMessageHandlers.splice(index, 1);
            }
        };
    };
    ServerConnection.prototype.processNotifyMessage = function (e) {
        try {
            var message_1 = json_1.default.parse(e.data);
            if (message_1.messageRole === 'NotifyMessage'
                && message_1.name
                && this.notifyMessageHandlers[message_1.name]) {
                this.notifyMessageHandlers[message_1.name]
                    .forEach(function (handler) { return handler(message_1.data, message_1.attributes); });
            }
        }
        catch (e) {
        }
    };
    ServerConnection.prototype.onNotifyMessage = function (name, handler) {
        var _this = this;
        if (!(name && handler)) {
            return noop_1.default;
        }
        if (!this.notifyMessageHandlers[name]) {
            this.notifyMessageHandlers[name] = [];
        }
        this.notifyMessageHandlers[name].push(handler);
        return function () {
            var index = _this.notifyMessageHandlers[name].indexOf(handler);
            if (index > -1) {
                _this.notifyMessageHandlers[name].splice(index, 1);
            }
        };
    };
    return ServerConnection;
}());
exports.ServerConnection = ServerConnection;
var connection = new ServerConnection();
Object.defineProperty(exports, "__esModule", { value: true });
exports.default = connection;
