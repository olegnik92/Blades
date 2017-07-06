
describe('Jasmine own tests', () => {
    it('True to be true', () => {
        let a = true;
        expect(a).toBe(true);
    });

    it('1 to be 1 and 2 to be 2', () => {
        expect(1).toBe(1);
        expect(2).toBe(2);
    });
});


