
import { JsonOperation } from '../serverOperations';
import RequestExecutionError from '../requestExecutionError';
import { Promise } from 'es6-promise';
import connection from '../serverConnection';
import auth from '../auth';


class Tuple<T1, T2, T3> {
    public item1: T1;
    public item2: T2;
    public item3: T3;
}

class ComplexDataType {
    public someInt: number;

    public someString: string;

    public someBool: boolean;

    public time: Date;

    public listData: Array<Tuple<number, string, boolean>>;
}


describe('Xhr tests', () => {

    beforeEach(() => {
        auth.clearTokenInfo();
    });

    it('TestOperation.EchoOperation', (done) => {
        let operation = new JsonOperation('TestOperation.EchoOperation', 5);
        operation.execute().then(result => {
            expect(result).toBe(5);
            done();
        }).catch(err => {
            console.error(err);
            expect(false).toBeTruthy();
            done();
        });
    });


    it('TestOperation.ComplexDataOperation', (done) => {
        let data = new ComplexDataType();
        data.someInt = -3;
        data.someString = 'Тестовая строка';
        data.someBool = false;
        data.time = new Date();
        data.listData = [{ item1: 5, item2: 'Abc', item3: true }, { item1: -5, item2: 'abC', item3: false }];

        new JsonOperation('TestOperation.ComplexDataOperation', data).execute()
            .then((result: ComplexDataType) => {
                expect(typeof (result.someInt)).toBe('number');
                expect(result.someInt).toBe(-6);

                expect(result.someString).toBe('ТЕСТОВАЯ СТРОКА');

                expect(typeof (result.someBool)).toBe('boolean');
                expect(result.someBool).toBe(true);

                expect(typeof (result.time)).toBe('object');
                expect(result.time.getHours()).toBe(data.time.getHours());
                expect(result.time.getFullYear()).toBe(data.time.getFullYear() + 1);

                expect(result.listData.length).toBe(2);
                expect(result.listData[0].item1).toBe(4);
                expect(result.listData[1].item2).toBe('abc');
                done();
            })
            .catch(err => {
                console.error(err);
                expect(false).toBeTruthy();
                done();
            });
    });


    it('TestOperation.FailedOperation 0', (done) => {
        let operation = new JsonOperation('TestOperation.FailedOperation', 0);
        operation.execute().then(result => {
            expect(result).toBe(0);
            done();
        }).catch(err => {
            console.error(err);
            expect(false).toBeTruthy();
            done();
        });
    });

    it('TestOperation.FailedOperation > 0', (done) => {
        let operation = new JsonOperation('TestOperation.FailedOperation', 5);
        operation.execute().then(result => {
            expect(result).toBe(0);
            done();
        }).catch((err: RequestExecutionError) => {
            expect(err.status).toBe(500);
            expect(err.message).toBe('Data > 0');
            done();
        });
    });

    it('TestOperation.FailedOperation < 0', (done) => {
        let operation = new JsonOperation('TestOperation.FailedOperation', -5);
        operation.execute().then(result => {
            expect(result).toBe(0);
            done();
        }).catch((err: RequestExecutionError) => {
            expect(err.status).toBe(500);
            expect(err.message).toBe('Data < 0');
            done();
        });
    });


    it('Login password good info test', (done) => {
        auth.authorize('admin', 'w')
            .then(data => {
                expect(data.login).toBe('admin');
                done();
            })
            .catch(err => {
                expect(false).toBeTruthy();
                done();
            });
    });

    it('Login password bad info test', (done) => {
        auth.authorize('admin', 'p')
            .then(data => {
                expect('').toBe('Попытка должна провалиться');
                done();
            })
            .catch(err => {
                expect(err.status).toBe(400);
                expect(err.message).toBe("Неверный логин или пароль");
                done();
            });
    });

    it('TestOperation.AuthFailedOperation Bad scenario', (done) => {
        let operation = new JsonOperation('TestOperation.AuthFailedOperation', 5);
        operation.execute().then(result => {
            expect(result).toBe('Попытка должна провалиться');
            done();
        }).catch((err: RequestExecutionError) => {
            expect(err.status).toBe(401);
            expect(err.message).toBe('Пользователь не авторизован');
            done();
        });
    });


    it('TestOperation.AuthFailedOperation Good scenario', (done) => {
        let operation = new JsonOperation('TestOperation.AuthFailedOperation', 7);
        auth.authorize('admin', 'w').then(() => {
            operation.execute().then(result => {
                expect(result).toBe(7);
                done();
            }).catch((err: RequestExecutionError) => {
                expect(false).toBeTruthy();
                done();
            });
        });

    });


    it('TestOperation.PermissionedFailedOperation Bad scenario', (done) => {
        let operation = new JsonOperation('TestOperation.PermissionedFailedOperation', 7);
        auth.authorize('someUser', 'w').then(() => {
            operation.execute().then(result => {
                expect(result).toBe('Попытка должна провалиться');
                done();
            }).catch((err: RequestExecutionError) => {
                expect(err.status).toBe(403);
                expect(err.message).toBe("Пользователь someuser не имеет прав: Update, Delete, на ресурс: Тестовый тип --- Тестовый объект");
                done();
            });
        });

    });


    it('TestOperation.PermissionedFailedOperation Good scenario', (done) => {
        let operation = new JsonOperation('TestOperation.PermissionedFailedOperation', 7);
        auth.authorize('admin', 'w').then(() => {
            operation.execute().then(result => {
                expect(result).toBe(7);
                done();
            }).catch((err: RequestExecutionError) => {
                expect(false).toBeTruthy();
                done();
            });
        });

    });
});


describe('Web socket connection Tests', () => {
    it('TestOperation.WebSocketOperation; test 1', (done) => {
        let data = new Tuple<number, string, Date>();
        data.item1 = 4
        data.item2 = 'TesT';
        data.item3 = new Date();

        let operation = new JsonOperation('TestOperation.WebSocketOperation', data);
        auth.authorize('admin', 'w').then(() => {
            connection.open();
            operation.executeViaConnection();
        });

        connection.onNotifyMessage('TestWsOp', (newData: Tuple<number, string, Date>) => {
            expect(newData.item1).toBe(8);
            expect(newData.item2).toBe('test');
            expect(newData.item3.getFullYear()).toBe(data.item3.getFullYear() + 5);
            done();
        });
    });

});