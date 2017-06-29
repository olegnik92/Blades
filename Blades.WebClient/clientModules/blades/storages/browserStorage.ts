import ITempStorage from './tempStorage';
import json from '../tools/json';


class StoreItem {
    public constructor(public data: Object, public expire?: number) {
    }
};

function expireValueKey(key: string): string {
    return `${key}__expire`;
};


function getExpireDateNumber(expire: number | Date): number {

    if (typeof expire === 'number') {
        return Date.now() + expire;
    } else if (expire.getTime) {
        return expire.getTime();
    }

    return 0;
};

function isShouldRemove(expire: number): boolean {
    return expire && expire < Date.now();
};

abstract class BrowserStorage implements ITempStorage {

    public constructor(private storage: Storage) {}

    public set(key: string, value: Object, expire?: number | Date): void {
        if (typeof value !== 'object') {
            return;
        }

        const storeItem = new StoreItem(value);
        if (expire) {
            storeItem.expire = getExpireDateNumber(expire);
        }

        this.storage.setItem(key, json.stringify(storeItem));
    }


    public get(key: string): Object {
        const itemStr = this.storage.getItem(key);
        if (!itemStr) {
            return null;
        }

        const storeItem = <StoreItem>json.parse(itemStr);
        if (typeof storeItem !== 'object') {
            return null;
        }

        if (isShouldRemove(storeItem.expire)) {
            this.storage.removeItem(key);
            return null;
        }

        return storeItem.data;
    }


    public setStr(key: string, value: string, expire?: number | Date): void {
        if (typeof value !== 'string') {
            return;
        }


        this.storage.setItem(key, value);
        if (expire) {
            this.storage.setItem(expireValueKey(key), getExpireDateNumber(expire).toString());
        }
    }


    public getStr(key: string): string {
        const expire = parseInt(this.storage.getItem(expireValueKey(key)));
        if (isShouldRemove(expire)) {
            this.storage.removeItem(expireValueKey(key));
            this.storage.removeItem(key);
            return null;
        }

        return this.storage.getItem(key);
    }

    public remove(key: string): void {
        this.storage.removeItem(key);
        this.storage.removeItem(expireValueKey(key));
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