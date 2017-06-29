import ITempStorage from '../tempStorage';
import { localStorage, sessionStorage } from '../browserStorage';
import cookieStorage from '../cookieStorage';


describe('Blades Temp storage tests', () => {

    describe('Local storage test', () => {
        testScript(localStorage);
    });

    describe('Session storage test', () => {
        testScript(sessionStorage);
    });

    describe('Cookie storage test', () => {
        testScript(cookieStorage);
    });
});


function testScript(storage: ITempStorage): void  {
    it('Add item', () => {
        const item = { a: 5 };
        storage.set('item', item);
        const savedItem = storage.get('item');
        expect(savedItem['a']).toBe(5);
    });

    it('Add item with expired', (done) => {
        const item = { a: 5 };
        storage.set('item', item, 150);
        setTimeout(() => {
            const savedItem = storage.get('item');
            expect(savedItem['a']).toBe(5);
        }, 50);

        setTimeout(() => {
            const savedItem = storage.get('item');
            expect(savedItem).toBeNull();
            done();
        }, 250);
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