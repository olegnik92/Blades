"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var browserStorage_1 = require("../browserStorage");
var cookieStorage_1 = require("../cookieStorage");
require("../../jasmine");
describe('Blades Temp storage tests', function () {
    describe('Local storage test', function () {
        testScript(browserStorage_1.localStorage, false);
    });
    describe('Session storage test', function () {
        testScript(browserStorage_1.sessionStorage, false);
    });
    describe('Cookie storage test', function () {
        testScript(cookieStorage_1.default, true);
    });
});
function testScript(storage, longInterval) {
    var expireInterval = longInterval ? 2000 : 150;
    var test1Interval = longInterval ? 100 : 50;
    var test2Interval = longInterval ? 3000 : 250;
    it('Add item', function () {
        var item = { a: 5 };
        storage.set('item', item);
        var savedItem = storage.get('item');
        expect(savedItem['a']).toBe(5);
    });
    it('Add item with expired', function (done) {
        var item = { a: 5 };
        storage.set('item', item, expireInterval);
        setTimeout(function () {
            var savedItem = storage.get('item');
            expect(savedItem['a']).toBe(5);
        }, test1Interval);
        setTimeout(function () {
            var savedItem = storage.get('item');
            expect(savedItem).toBeNull();
            done();
        }, test2Interval);
    });
    it('Add str value and expire in Date format', function (done) {
        var item = 'Test String';
        var expireDate = new Date();
        expireDate.setTime(Date.now() + expireInterval);
        storage.setStr('item', item, expireDate);
        setTimeout(function () {
            var savedStr = storage.getStr('item');
            expect(savedStr).toBe(item);
        }, test1Interval);
        setTimeout(function () {
            var savedStr = storage.getStr('item');
            expect(savedStr).toBeNull();
            done();
        }, test2Interval);
    });
    it('Remove item', function () {
        var item = { a: 6 };
        storage.set('item', item);
        var savedItem = storage.get('item');
        expect(savedItem['a']).toBe(6);
        storage.remove('item');
        var savedItem2 = storage.get('item');
        expect(savedItem2).toBeNull();
    });
}
;
