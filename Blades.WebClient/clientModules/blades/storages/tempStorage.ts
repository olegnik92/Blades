

interface ITempStorage {

    get(key: string): Object | null;

    set(key: string, value: Object, expire?: number | Date): void; //number - число милисекунд  до истичения срока, Date - дата истичения срока

    //getStr и setStr используются для объектов, которых не желательно применяь сериализацию
    getStr(key: string): string | null;

    setStr(key: string, value: string, expire?: number | Date): void; //number - число милисекунд  до истичения срока, Date - дата истичения срока


    remove(key: string): void;
};

export default ITempStorage;