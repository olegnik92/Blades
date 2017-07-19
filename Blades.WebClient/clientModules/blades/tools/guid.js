"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var Guid = (function () {
    function Guid() {
    }
    Guid.empty = function () {
        return '00000000-0000-0000-0000-000000000000';
    };
    //from https://stackoverflow.com/questions/105034/create-guid-uuid-in-javascript#answer-2117523
    Guid.new = function () {
        return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
            var r = Math.random() * 16 | 0, v = c == 'x' ? r : (r & 0x3 | 0x8);
            return v.toString(16);
        });
    };
    Guid.isEquals = function (guid1, guid2) {
        return (typeof guid1 === 'string') && (typeof guid2 === 'string') && (guid1.toLowerCase() === guid2.toLowerCase());
    };
    return Guid;
}());
exports.default = Guid;
//# sourceMappingURL=guid.js.map