import ITempStorage from './tempStorage';
import json from '../tools/json';

const deathTimeField = '__deathTime';

abstract class BrowserStorage implements ITempStorage {

    public constructor(private storage: Storage) {}

    public set(key: string, value: Object, timeToLive?: number): void {
        if (typeof value !== 'object') {
            return;
        }

        const storeItem = { ...value };
        if (timeToLive) {
            const deathTime = Date.now() + timeToLive;
            storeItem[deathTimeField] = deathTime;
        }

        this.storage.setItem(key, json.stringify(storeItem));
    }

    public get(key: string): Object {
        const itemStr = this.storage.getItem(key);
        if (!itemStr) {
            return null;
        }

        const storeItem = <Object>json.parse(itemStr);
        if (typeof storeItem !== 'object') {
            return null;
        }

        if (storeItem[deathTimeField] < Date.now()) {
            this.storage.removeItem(key);
            return null;
        }

        return storeItem;
    }

    public remove(key: string): void {
        this.storage.removeItem(key);
    }
};

export default BrowserStorage;



export class SessionStorage extends BrowserStorage {
    public constructor() {
        super(window.sessionStorage);
    }
}

export const sessionStorage = new SessionStorage();

export class LocalStorage extends BrowserStorage {
    public constructor() {
        super(window.localStorage);
    }
}

export const localStorage = new LocalStorage();