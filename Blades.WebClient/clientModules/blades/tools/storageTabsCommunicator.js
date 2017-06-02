"use strict";
var StorageTabsCommunicator = (function () {
    function StorageTabsCommunicator(id) {
        var _this = this;
        this.communicationItemKey = 'StorageTabsCommunicator.communicationItemKey';
        this.tabFocusEventKey = 'StorageTabsCommunicator.tabFocusEventKey';
        this.receiveDataHandlers = [];
        this.communicationItemKey += id;
        this.tabFocusEventKey += id;
        window.addEventListener('focus', function (e) {
            localStorage.setItem(_this.tabFocusEventKey, _this.seed().toString());
            _this.isActive = true;
        });
        window.addEventListener('storage', function (e) {
            if (e.key === _this.communicationItemKey) {
                var data = JSON.parse(e.newValue).data;
                _this.fireReceiveDataHandlers(data);
            }
            else if (e.key === _this.tabFocusEventKey) {
                _this.isActive = false;
            }
        });
    }
    StorageTabsCommunicator.prototype.seed = function () {
        return Date.now() + Math.random();
    };
    StorageTabsCommunicator.prototype.isActiveTab = function () {
        return this.isActive;
    };
    StorageTabsCommunicator.prototype.sendDataToOtherTabs = function (data) {
        var sendItem = {
            data: data, seed: this.seed()
        };
        localStorage.setItem(this.communicationItemKey, JSON.stringify(sendItem));
    };
    StorageTabsCommunicator.prototype.receiveData = function (handler) {
        var _this = this;
        this.receiveDataHandlers.push(handler);
        return function () {
            var index = _this.receiveDataHandlers.indexOf(handler);
            if (index > -1) {
                _this.receiveDataHandlers.splice(index, 1);
            }
        };
    };
    StorageTabsCommunicator.prototype.fireReceiveDataHandlers = function (data) {
        this.receiveDataHandlers.forEach(function (handler) { return handler(data); });
    };
    Object.defineProperty(StorageTabsCommunicator, "instance", {
        get: function () {
            if (StorageTabsCommunicator.inst == null) {
                StorageTabsCommunicator.inst = new StorageTabsCommunicator('');
            }
            return StorageTabsCommunicator.inst;
        },
        enumerable: true,
        configurable: true
    });
    return StorageTabsCommunicator;
}());
StorageTabsCommunicator.inst = null;
exports.StorageTabsCommunicator = StorageTabsCommunicator;
var storageTabsCommunicator = StorageTabsCommunicator.instance;
Object.defineProperty(exports, "__esModule", { value: true });
exports.default = storageTabsCommunicator;
