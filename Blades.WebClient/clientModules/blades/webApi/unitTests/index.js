"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var serverOperations_1 = require("../serverOperations");
var serverConnection_1 = require("../serverConnection");
var auth_1 = require("../auth");
var Tuple = (function () {
    function Tuple() {
    }
    return Tuple;
}());
var ComplexDataType = (function () {
    function ComplexDataType() {
    }
    return ComplexDataType;
}());
describe('Xhr tests', function () {
    beforeEach(function () {
        auth_1.default.clearTokenInfo();
    });
    it('TestOperation.EchoOperation', function (done) {
        var operation = new serverOperations_1.JsonOperation('TestOperation.EchoOperation', 5);
        operation.execute().then(function (result) {
            expect(result).toBe(5);
            done();
        }).catch(function (err) {
            console.error(err);
            expect(false).toBeTruthy();
            done();
        });
    });
    it('TestOperation.ComplexDataOperation', function (done) {
        var data = new ComplexDataType();
        data.someInt = -3;
        data.someString = 'Тестовая строка';
        data.someBool = false;
        data.time = new Date();
        data.listData = [{ item1: 5, item2: 'Abc', item3: true }, { item1: -5, item2: 'abC', item3: false }];
        new serverOperations_1.JsonOperation('TestOperation.ComplexDataOperation', data).execute()
            .then(function (result) {
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
            .catch(function (err) {
            console.error(err);
            expect(false).toBeTruthy();
            done();
        });
    });
    it('TestOperation.FailedOperation 0', function (done) {
        var operation = new serverOperations_1.JsonOperation('TestOperation.FailedOperation', 0);
        operation.execute().then(function (result) {
            expect(result).toBe(0);
            done();
        }).catch(function (err) {
            console.error(err);
            expect(false).toBeTruthy();
            done();
        });
    });
    it('TestOperation.FailedOperation > 0', function (done) {
        var operation = new serverOperations_1.JsonOperation('TestOperation.FailedOperation', 5);
        operation.execute().then(function (result) {
            expect(result).toBe(0);
            done();
        }).catch(function (err) {
            expect(err.status).toBe(500);
            expect(err.message).toBe('Data > 0');
            done();
        });
    });
    it('TestOperation.FailedOperation < 0', function (done) {
        var operation = new serverOperations_1.JsonOperation('TestOperation.FailedOperation', -5);
        operation.execute().then(function (result) {
            expect(result).toBe(0);
            done();
        }).catch(function (err) {
            expect(err.status).toBe(500);
            expect(err.message).toBe('Data < 0');
            done();
        });
    });
    it('Login password good info test', function (done) {
        auth_1.default.authorize('admin', 'w')
            .then(function (data) {
            expect(data.login).toBe('admin');
            done();
        })
            .catch(function (err) {
            expect(false).toBeTruthy();
            done();
        });
    });
    it('Login password bad info test', function (done) {
        auth_1.default.authorize('admin', 'p')
            .then(function (data) {
            expect('').toBe('Попытка должна провалиться');
            done();
        })
            .catch(function (err) {
            expect(err.status).toBe(400);
            expect(err.message).toBe("Неверный логин или пароль");
            done();
        });
    });
    it('TestOperation.AuthFailedOperation Bad scenario', function (done) {
        var operation = new serverOperations_1.JsonOperation('TestOperation.AuthFailedOperation', 5);
        operation.execute().then(function (result) {
            expect(result).toBe('Попытка должна провалиться');
            done();
        }).catch(function (err) {
            expect(err.status).toBe(401);
            expect(err.message).toBe('Пользователь не авторизован');
            done();
        });
    });
    it('TestOperation.AuthFailedOperation Good scenario', function (done) {
        var operation = new serverOperations_1.JsonOperation('TestOperation.AuthFailedOperation', 7);
        auth_1.default.authorize('admin', 'w').then(function () {
            operation.execute().then(function (result) {
                expect(result).toBe(7);
                done();
            }).catch(function (err) {
                expect(false).toBeTruthy();
                done();
            });
        });
    });
    it('TestOperation.PermissionedFailedOperation Bad scenario', function (done) {
        var operation = new serverOperations_1.JsonOperation('TestOperation.PermissionedFailedOperation', 7);
        auth_1.default.authorize('someUser', 'w').then(function () {
            operation.execute().then(function (result) {
                expect(result).toBe('Попытка должна провалиться');
                done();
            }).catch(function (err) {
                expect(err.status).toBe(403);
                expect(err.message).toBe("Пользователь someuser не имеет прав: Update, Delete, на ресурс: Тестовый тип --- Тестовый объект");
                done();
            });
        });
    });
    it('TestOperation.PermissionedFailedOperation Good scenario', function (done) {
        var operation = new serverOperations_1.JsonOperation('TestOperation.PermissionedFailedOperation', 7);
        auth_1.default.authorize('admin', 'w').then(function () {
            operation.execute().then(function (result) {
                expect(result).toBe(7);
                done();
            }).catch(function (err) {
                expect(false).toBeTruthy();
                done();
            });
        });
    });
    it('TestOperation.ChildrenTypesData', function (done) {
        var baseType = { field1: 10 };
        var childType1 = { $type: '38ADA025-A94F-486E-AEEF-55E61F951CF8, guid', field1: 10, field2: 5 };
        var childType2 = { $type: 'A670EBC8-29D8-44DD-8D23-C08BAE68F218, guid', field1: 10, field2: 5 };
        var operation0 = new serverOperations_1.JsonOperation('TestOperation.ChildrenTypesData', baseType);
        var operation1 = new serverOperations_1.JsonOperation('TestOperation.ChildrenTypesData', childType1);
        var operation2 = new serverOperations_1.JsonOperation('TestOperation.ChildrenTypesData', childType2);
        operation0.execute().then(function (result) {
            expect(result).toBe(baseType.field1);
            return operation1.execute();
        }).then(function (result) {
            expect(result).toBe(childType1.field1 + childType1.field2);
            return operation2.execute();
        }).then(function (result) {
            expect(result).toBe(childType2.field1 - childType2.field2);
            done();
        });
    });
    it('TestOperation.ListOfChildrenTypes', function (done) {
        var baseType = { field1: 10 };
        var childType1 = { $type: '38ADA025-A94F-486E-AEEF-55E61F951CF8, guid', field1: 10, field2: 5 };
        var childType2 = { $type: 'A670EBC8-29D8-44DD-8D23-C08BAE68F218, guid', field1: 10, field2: 5 };
        var data = [baseType, childType1, childType2];
        var operation = new serverOperations_1.JsonOperation('TestOperation.ListOfChildrenTypes', data);
        operation.execute().then(function (results) {
            expect(results.length).toBe(3);
            expect(results[0]).toBe(baseType.field1);
            expect(results[1]).toBe(childType1.field1 + childType1.field2);
            expect(results[2]).toBe(childType2.field1 - childType2.field2);
            done();
        });
    });
});
describe('Web socket connection Tests', function () {
    it('TestOperation.WebSocketOperation; test 1', function (done) {
        var data = new Tuple();
        data.item1 = 4;
        data.item2 = 'TesT';
        data.item3 = new Date();
        var operation = new serverOperations_1.JsonOperation('TestOperation.WebSocketOperation', data);
        auth_1.default.authorize('admin', 'w').then(function () {
            serverConnection_1.default.open();
            operation.executeViaConnection();
        });
        serverConnection_1.default.onNotifyMessage('TestWsOp', function (newData) {
            expect(newData.item1).toBe(8);
            expect(newData.item2).toBe('test');
            expect(newData.item3.getFullYear()).toBe(data.item3.getFullYear() + 5);
            done();
        });
    });
});
