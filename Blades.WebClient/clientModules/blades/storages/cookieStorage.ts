import cookie, { CookieOptions } from '../tools/cookie';
import json from '../tools/json';
import ITempStorage from './tempStorage';


export class CookieStorage implements ITempStorage {

    public set(key: string, value: Object, expire?: number | Date): void {
        if (typeof value !== 'object') {
            return;
        }

        const options = new CookieOptions(expire);
        cookie.setCookie(key, json.stringify(value), options); 
    }

    public get(key: string): Object | null {
        const itemStr = cookie.getCookie(key);
        if (!itemStr) {
            return null;
        }

        const storeItem = <Object>json.parse(itemStr);
        if (typeof storeItem !== 'object') {
            return null;
        }

        return storeItem;
    }

    public setStr(key: string, value: string, expire?: number | Date): void {
        if (typeof value !== 'string') {
            return;
        }

        const options = new CookieOptions(expire);
        cookie.setCookie(key, value, options);
    }

    public getStr(key: string): string | null {
        const itemStr = cookie.getCookie(key);
        if (typeof itemStr !== 'string') {
            return null;
        }

        return itemStr;
    }

    public remove(key: string): void {
        cookie.deleteCookie(key);
    }
};


const cookieStorage = new CookieStorage();
export default cookieStorage;