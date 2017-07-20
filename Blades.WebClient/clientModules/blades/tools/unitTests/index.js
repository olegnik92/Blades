"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var barrier_1 = require("../barrier");
var syncActionsChain_1 = require("../syncActionsChain");
var asyncActionsChain_1 = require("../asyncActionsChain");
require("../../jasmine");
describe('Blades Tools tests', function () {
    describe('Barrier tests', function () {
        barrierTests();
    });
    describe('Sync actions chain tests', function () {
        syncActionsChainTests();
    });
    describe('Async actions chain tests', function () {
        asyncActionsChainTests();
    });
});
function barrierTests() {
    it('Only last call Test', function (done) {
        var barrier = new barrier_1.default(100);
        var callsCounter = 0;
        var inc = function () {
            callsCounter++;
        };
        barrier.exec(inc);
        barrier.exec(inc);
        setTimeout(function () {
            barrier.exec(inc);
        }, 30);
        setTimeout(function () {
            barrier.exec(inc);
        }, 50);
        setTimeout(function () {
            expect(callsCounter).toBe(1);
            done();
        }, 200);
    });
    it('Exec after Interval', function (done) {
        var barrier = new barrier_1.default(100);
        var callsCounter = 0;
        var inc = function () {
            callsCounter++;
        };
        setTimeout(function () {
            barrier.exec(inc);
        }, 30);
        setTimeout(function () {
            barrier.exec(inc);
        }, 50);
        setTimeout(function () {
            barrier.exec(inc);
        }, 200);
        setTimeout(function () {
            expect(callsCounter).toBe(2);
            done();
        }, 350);
    });
    it('Exec one by one', function (done) {
        var barrier = new barrier_1.default(-1); //without wait
        var callsCounter = 0;
        barrier.execAsync(function () {
            return new Promise(function (res, rej) {
                setTimeout(function () {
                    expect(callsCounter).toBe(0);
                    callsCounter++;
                    res();
                }, 50);
            });
        });
        barrier.execAsync(function () {
            return new Promise(function (res, rej) {
                setTimeout(function () {
                    expect(callsCounter).toBe(1);
                    callsCounter++;
                    res();
                }, 50);
            });
        });
        barrier.execAsync(function () {
            return new Promise(function (res, rej) {
                setTimeout(function () {
                    expect(callsCounter).toBe(2);
                    callsCounter++;
                    res();
                }, 50);
            });
        });
        setTimeout(function () {
            done();
        }, 200);
    });
    it('Only last call Test Async', function (done) {
        var barrier = new barrier_1.default(50);
        var callsCounter = 0;
        barrier.execAsync(function () {
            return new Promise(function (res, rej) {
                setTimeout(function () {
                    expect(false).toBeTruthy(); //should not fired
                    callsCounter++;
                    res();
                }, 50);
            });
        });
        setTimeout(function () {
            barrier.execAsync(function () {
                return new Promise(function (res, rej) {
                    setTimeout(function () {
                        expect(callsCounter).toBe(0); //should  fired
                        callsCounter++;
                        res();
                    }, 50);
                });
            });
        }, 10);
        setTimeout(function () {
            barrier.execAsync(function () {
                return new Promise(function (res, rej) {
                    setTimeout(function () {
                        expect(callsCounter).toBe(1); //should  fired
                        callsCounter++;
                        res();
                    }, 50);
                });
            });
        }, 70);
        setTimeout(function () {
            expect(callsCounter).toBe(2);
            done();
        }, 200);
    });
}
;
var A = (function () {
    function A() {
    }
    return A;
}());
function syncActionsChainTests() {
    it('Chain', function () {
        var chain = new syncActionsChain_1.default();
        var test = new A();
        test.a = 0;
        chain.addLast(function (d) {
            expect(d.a).toBe(1);
            d.a++;
            return d;
        });
        chain.addFirst(function (d) {
            expect(d.a).toBe(0);
            d.a++;
            return d;
        });
        chain.addLast(function (d) {
            expect(d.a).toBe(2);
            d.a++;
            return d;
        });
        expect(test.a).toBe(0);
        test = chain.run(test);
        expect(test.a).toBe(3);
    });
    it('Empty Chain', function () {
        var chain = new syncActionsChain_1.default();
        var test = new A();
        test.a = 0;
        expect(test.a).toBe(0);
        test = chain.run(test);
        expect(test.a).toBe(0);
    });
    it('Remove from Chain', function () {
        var chain = new syncActionsChain_1.default();
        var test = new A();
        test.a = 0;
        var remove = chain.addLast(function (d) {
            d.a++;
            return d;
        });
        chain.addFirst(function (d) {
            d.a++;
            return d;
        });
        chain.addLast(function (d) {
            d.a++;
            return d;
        });
        expect(test.a).toBe(0);
        test = chain.run(test);
        expect(test.a).toBe(3);
        test.a = 0;
        remove();
        test = chain.run(test);
        expect(test.a).toBe(2);
    });
}
;
function asyncActionsChainTests() {
    it('Chain', function (done) {
        var chain = new asyncActionsChain_1.default();
        var test = new A();
        test.a = 0;
        chain.addLast(function (d) {
            return new Promise(function (res) {
                setTimeout(function () {
                    expect(d.a).toBe(1);
                    d.a++;
                    res(d);
                }, 10);
            });
        });
        chain.addFirst(function (d) {
            return new Promise(function (res) {
                setTimeout(function () {
                    expect(d.a).toBe(0);
                    d.a++;
                    res(d);
                }, 10);
            });
        });
        chain.addLast(function (d) {
            return new Promise(function (res) {
                setTimeout(function () {
                    expect(d.a).toBe(2);
                    d.a++;
                    res(d);
                }, 10);
            });
        });
        expect(test.a).toBe(0);
        chain.run(test).then(function (d) {
            expect(d.a).toBe(3);
            done();
        });
    });
    it('Empty Chain', function (done) {
        var chain = new asyncActionsChain_1.default();
        var test = new A();
        test.a = 0;
        expect(test.a).toBe(0);
        chain.run(test).then(function (d) {
            expect(d.a).toBe(0);
            done();
        });
    });
    it('Remove from Chain', function (done) {
        var chain = new asyncActionsChain_1.default();
        var test = new A();
        function getIncFunc() {
            return function (d) {
                return new Promise(function (res) {
                    setTimeout(function () {
                        d.a++;
                        res(d);
                    }, 10);
                });
            };
        }
        ;
        chain.addLast(getIncFunc());
        var remove = chain.addLast(getIncFunc());
        chain.addLast(getIncFunc());
        test.a = 0;
        chain.run(test).then(function (d) {
            expect(d.a).toBe(3);
        }).then(function () {
            test.a = 0;
            remove();
            return chain.run(test);
        }).then(function (d) {
            expect(d.a).toBe(2);
            done();
        });
    });
}
;
