"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var noop = function () { };
exports.default = noop;
function echo(data) {
    return data;
}
exports.echo = echo;
;
function echoPromise(data) {
    return new Promise(function (res, rej) {
        res(data);
    });
}
exports.echoPromise = echoPromise;
;
