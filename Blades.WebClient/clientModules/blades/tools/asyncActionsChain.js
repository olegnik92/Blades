"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var noop_1 = require("./noop");
var AsyncActionsChain = (function () {
    function AsyncActionsChain() {
        this.chain = [];
        this.asyncAction = this.buildChain();
    }
    AsyncActionsChain.prototype.addLast = function (item) {
        return this.add(item, true);
    };
    AsyncActionsChain.prototype.addFirst = function (item) {
        return this.add(item, false);
    };
    AsyncActionsChain.prototype.add = function (item, toEnd) {
        var _this = this;
        if (typeof item !== 'function') {
            return noop_1.default;
        }
        if (toEnd) {
            this.chain.push(item);
        }
        else {
            this.chain.unshift(item);
        }
        this.asyncAction = this.buildChain();
        return function () {
            var index = _this.chain.indexOf(item);
            if (index > -1) {
                _this.chain.splice(index, 1);
                _this.asyncAction = _this.buildChain();
            }
        };
    };
    AsyncActionsChain.prototype.run = function (data) {
        return this.asyncAction(data);
    };
    AsyncActionsChain.prototype.buildChain = function () {
        var _this = this;
        var action = noop_1.echoPromise;
        if (this.chain.length === 0) {
            return action;
        }
        var _loop_1 = function (i) {
            var oldAction = action;
            action = (function (data) { return oldAction(data).then(_this.chain[i]); });
        };
        for (var i = 0; i < this.chain.length; i++) {
            _loop_1(i);
        }
        return action;
    };
    return AsyncActionsChain;
}());
;
exports.default = AsyncActionsChain;
