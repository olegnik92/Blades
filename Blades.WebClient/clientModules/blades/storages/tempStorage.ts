

interface ITempStorage {

    get(key: string): Object;

    set(key: string, value: Object, timeToLive?: number): void;

    remove(key: string): void;
};

export default ITempStorage;