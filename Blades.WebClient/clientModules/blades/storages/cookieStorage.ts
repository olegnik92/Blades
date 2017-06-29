import cookie, { CookieOptions } from '../tools/cookie';
import json from '../tools/json';
import ITempStorage from './tempStorage';


export class CookieStorage implements ITempStorage {

    public set(key: string, value: Object, timeToLive?: number): void {
        if (typeof value !== 'object') {
            return;
        }

        const options = new CookieOptions(timeToLive || undefined);
        cookie.setCookie(key, json.stringify(value), options); 
    }

    public get(key: string): Object {
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

    public remove(key: string): void {
        cookie.deleteCookie(key);
    }
};


const cookieStorage = new CookieStorage();
export default cookieStorage;