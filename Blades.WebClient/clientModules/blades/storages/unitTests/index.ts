import ITempStorage from '../tempStorage';
import { localStorage, sessionStorage } from '../browserStorage';
import cookieStorage from '../cookieStorage';


describe('Blades Temp storage tests', () => {

    describe('Local storage test', () => {
        testScript(localStorage, false);
    });

    describe('Session storage test', () => {
        testScript(sessionStorage, false);
    });

    describe('Cookie storage test', () => {
        testScript(cookieStorage, true);
    });
});


function testScript(storage: ITempStorage, longInterval: boolean): void  {

    const expireInterval = longInterval ? 2000 : 150;
    const test1Interval = longInterval ? 100 : 50;
    const test2Interval = longInterval ? 3000 : 250;

    it('Add item', () => {
        const item = { a: 5 };
        storage.set('item', item);
        const savedItem = storage.get('item');
        expect(savedItem['a']).toBe(5);
    });

    it('Add item with expired', (done) => {
        const item = { a: 5 };
        storage.set('item', item, expireInterval);
        setTimeout(() => {
            const savedItem = storage.get('item');
            expect(savedItem['a']).toBe(5);
        }, test1Interval);

        setTimeout(() => {
            const savedItem = storage.get('item');
            expect(savedItem).toBeNull();
            done();
        }, test2Interval);
    });


    it('Add str value and expire in Date format', (done) => {
        const item = 'Test String';
        const expireDate = new Date();
        expireDate.setTime(Date.now() + expireInterval);
        storage.setStr('item', item, expireDate);

        setTimeout(() => {
            const savedStr = storage.getStr('item');
            expect(savedStr).toBe(item);
        }, test1Interval);

        setTimeout(() => {
            const savedStr = storage.getStr('item');
            expect(savedStr).toBeNull();
            done();
        }, test2Interval);
    });

    it('Remove item', () => {
        const item = { a: 6 };
        storage.set('item', item);
        const savedItem = storage.get('item');
        expect(savedItem['a']).toBe(6);

        storage.remove('item');
        const savedItem2 = storage.get('item');
        expect(savedItem2).toBeNull();
    });
};