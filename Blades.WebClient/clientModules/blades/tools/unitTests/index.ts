import Barrier from '../barrier';
import SyncActionsChain from '../syncActionsChain';
import AsyncActionsChain from '../asyncActionsChain';
import '../../jasmine';

describe('Blades Tools tests', () => {

    describe('Barrier tests', () => {
        barrierTests();
    });

    describe('Sync actions chain tests', () => {
        syncActionsChainTests();
    });

    describe('Async actions chain tests', () => {
        asyncActionsChainTests();
    });
});




function barrierTests() {
    it('Only last call Test', (done) => {
        const barrier = new Barrier(100);
        let callsCounter = 0;
        const inc  = () => {
            callsCounter++;
        };

        barrier.exec(inc);
        barrier.exec(inc);
        setTimeout(() => {
            barrier.exec(inc);
        }, 30);

        setTimeout(() => {
            barrier.exec(inc);
        }, 50);

        setTimeout(() => {
            expect(callsCounter).toBe(1);
            done();
        }, 200);
    });


    it('Exec after Interval', (done) => {
        const barrier = new Barrier(100);
        let callsCounter = 0;
        const inc = () => {
            callsCounter++;
        };

        setTimeout(() => {
            barrier.exec(inc);
        }, 30);

        setTimeout(() => {
            barrier.exec(inc);
        }, 50);


        setTimeout(() => {
            barrier.exec(inc);
        }, 200);


        setTimeout(() => {
            expect(callsCounter).toBe(2);
            done();
        }, 350);
    });


    it('Exec one by one', (done) => {
        const barrier = new Barrier(-1); //without wait
        let callsCounter = 0;
        barrier.execAsync(() => {
            return new Promise((res, rej) => {
                setTimeout(() => {
                    expect(callsCounter).toBe(0);
                    callsCounter++;
                    res();
                }, 50);
            });
        });

        barrier.execAsync(() => {
            return new Promise((res, rej) => {
                setTimeout(() => {
                    expect(callsCounter).toBe(1);
                    callsCounter++;
                    res();
                }, 50);
            });
        });

        barrier.execAsync(() => {
            return new Promise((res, rej) => {
                setTimeout(() => {
                    expect(callsCounter).toBe(2);
                    callsCounter++;
                    res();
                }, 50);
            });
        });


        setTimeout(() => {
            done();
        }, 200);
    });


    it('Only last call Test Async', (done) => {
        const barrier = new Barrier(50);
        let callsCounter = 0;
        barrier.execAsync(() => {
            return new Promise((res, rej) => {
                setTimeout(() => {
                    expect(false).toBeTruthy(); //should not fired
                    callsCounter++;
                    res();
                }, 50);
            });
        });

        setTimeout(() => {
            barrier.execAsync(() => {
                return new Promise((res, rej) => {
                    setTimeout(() => {
                        expect(callsCounter).toBe(0); //should  fired
                        callsCounter++;
                        res();
                    }, 50);
                });
            });
        }, 10);

        setTimeout(() => {
            barrier.execAsync(() => {
                return new Promise((res, rej) => {
                    setTimeout(() => {
                        expect(callsCounter).toBe(1); //should  fired
                        callsCounter++;
                        res();
                    }, 50);
                });
            });
        }, 70);

        setTimeout(() => {
            expect(callsCounter).toBe(2); 
            done();
        }, 200);
    });
};




class A {
    a: number;
}

function syncActionsChainTests() {
    it('Chain', () => {
        var chain = new SyncActionsChain<A>();
        let test = new A();
        test.a = 0;
        chain.addLast((d) => {
            expect(d.a).toBe(1);
            d.a++;
            return d;
        });

        chain.addFirst((d) => {
            expect(d.a).toBe(0);
            d.a++;
            return d;
        });

        chain.addLast((d) => {
            expect(d.a).toBe(2);
            d.a++;
            return d;
        });

        expect(test.a).toBe(0);
        test = chain.run(test);
        expect(test.a).toBe(3);
    });

    it('Empty Chain', () => {
        var chain = new SyncActionsChain<A>();
        let test = new A();
        test.a = 0;
        expect(test.a).toBe(0);
        test = chain.run(test);
        expect(test.a).toBe(0);
    });


    it('Remove from Chain', () => {
        var chain = new SyncActionsChain<A>();
        let test = new A();
        test.a = 0;
        const remove = chain.addLast((d) => {
            d.a++;
            return d;
        });

        chain.addFirst((d) => {
            d.a++;
            return d;
        });

        chain.addLast((d) => {
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
};


function asyncActionsChainTests() {
    it('Chain', (done) => {
        const chain = new AsyncActionsChain<A>();
        let test = new A();
        test.a = 0;
        chain.addLast((d) => {
            return new Promise((res) => {
                setTimeout(() => {
                    expect(d.a).toBe(1);
                    d.a++;
                    res(d);
                }, 10);
            });
        });

        chain.addFirst((d) => {
            return new Promise((res) => {
                setTimeout(() => {
                    expect(d.a).toBe(0);
                    d.a++;
                    res(d);
                }, 10);
            });
        });

        chain.addLast((d) => {
            return new Promise((res) => {
                setTimeout(() => {
                    expect(d.a).toBe(2);
                    d.a++;
                    res(d);
                }, 10);
            });
        });

        expect(test.a).toBe(0)
        chain.run(test).then(d => {
            expect(d.a).toBe(3);
            done();
        });
    });


    it('Empty Chain', (done) => {
        var chain = new AsyncActionsChain<A>();
        let test = new A();
        test.a = 0;
        expect(test.a).toBe(0);
        chain.run(test).then((d) => {
            expect(d.a).toBe(0);
            done();
        });
    });


    it('Remove from Chain', (done) => {
        const chain = new AsyncActionsChain<A>();
        let test = new A();

        function getIncFunc(): (d: A) => Promise<A> {
            return (d: A) => {
                return new Promise((res) => {
                    setTimeout(() => {
                        d.a++;
                        res(d);
                    }, 10);
                });
            };
        };

        chain.addLast(getIncFunc());
        const remove = chain.addLast(getIncFunc());
        chain.addLast(getIncFunc());

        test.a = 0;
        chain.run(test).then((d) => {
            expect(d.a).toBe(3);
        }).then(() => {
            test.a = 0;
            remove();
            return chain.run(test);
        }).then((d) => {
            expect(d.a).toBe(2);
            done();
        });
    });
};