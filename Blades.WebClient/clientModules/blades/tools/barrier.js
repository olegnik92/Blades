"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var Barrier = (function () {
    function Barrier(waitTime) {
        this.waitTime = waitTime;
        this.timeoutId = 0;
        this.deferred = null;
    }
    Barrier.prototype.exec = function (action) {
        if (this.timeoutId) {
            this.cancelWaitAction();
        }
        if (this.isWaitMode()) {
            this.executeWaitAction(action);
        }
        else {
            action();
        }
    };
    Barrier.prototype.isWaitMode = function () {
        return this.waitTime >= 0;
    };
    Barrier.prototype.cancelWaitAction = function () {
        clearTimeout(this.timeoutId);
        this.timeoutId = 0;
    };
    Barrier.prototype.executeWaitAction = function (action) {
        var _this = this;
        this.timeoutId = setTimeout(function () {
            action();
            _this.timeoutId = 0;
        }, this.waitTime);
    };
    ;
    Barrier.prototype.execAsync = function (action) {
        if (this.timeoutId) {
            this.cancelWaitAction();
        }
        if (this.isWaitMode()) {
            this.addToDeferredWithWait(action);
        }
        else {
            this.addToDeferred(action);
        }
    };
    ;
    Barrier.prototype.addToDeferred = function (action) {
        if (this.deferred == null) {
            this.deferred = new Promise(function (res, rej) {
                action().then(res).catch(rej);
            });
        }
        else {
            this.deferred = this.deferred.then(function () {
                return action();
            });
        }
    };
    ;
    Barrier.prototype.addToDeferredWithWait = function (action) {
        var _this = this;
        this.timeoutId = setTimeout(function () {
            _this.addToDeferred(action);
            _this.timeoutId = 0;
        }, this.waitTime);
    };
    ;
    return Barrier;
}());
;
exports.default = Barrier;
