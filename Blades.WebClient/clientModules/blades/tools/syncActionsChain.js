"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var noop_1 = require("./noop");
var SyncActionsChain = (function () {
    function SyncActionsChain() {
        this.chain = [];
    }
    SyncActionsChain.prototype.addLast = function (item) {
        return this.add(item, true);
    };
    SyncActionsChain.prototype.addFirst = function (item) {
        return this.add(item, false);
    };
    SyncActionsChain.prototype.add = function (item, toEnd) {
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
        return function () {
            var index = _this.chain.indexOf(item);
            if (index > -1) {
                _this.chain.splice(index, 1);
            }
        };
    };
    SyncActionsChain.prototype.run = function (data) {
        for (var i = 0; i < this.chain.length; i++) {
            data = this.chain[i](data);
        }
        return data;
    };
    return SyncActionsChain;
}());
;
exports.default = SyncActionsChain;
//# sourceMappingURL=syncActionsChain.js.map