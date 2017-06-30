

abstract class Guid {

    public static empty(): string {
        return '00000000-0000-0000-0000-000000000000';
    } 

    //from https://stackoverflow.com/questions/105034/create-guid-uuid-in-javascript#answer-2117523
    public static new(): string {
        return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
            var r = Math.random() * 16 | 0, v = c == 'x' ? r : (r & 0x3 | 0x8);
            return v.toString(16);
        });
    }

    public static isEquals(guid1: string, guid2: string): boolean{
        return (typeof guid1 === 'string') && (typeof guid2 === 'string') && (guid1.toLowerCase() === guid2.toLowerCase());
    }
}

export default Guid;