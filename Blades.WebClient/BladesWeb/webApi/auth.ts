import { Promise } from 'es6-promise';
import { TokenInfo } from './tokenInfo';
import { Xhr } from './xhr';
import { noop } from '../tools/noop';
import { cookie, CookieOptions } from '../tools/cookie';

export class Auth {

    private accessTokenInfoStorageKey: string;
    private accessTokenCookieName: string

    public accessTokenPath: string = '/token';


    constructor(accessTokenInfoStorageKey: string, accessTokenCookieName: string) {
        this.accessTokenInfoStorageKey = accessTokenInfoStorageKey;
        this.accessTokenCookieName = accessTokenCookieName;
    }

    public requestNewAccessToken(login: string, password: string): Promise<TokenInfo> {
        let data = `grant_type=password&username=${login}&password=${password}`;
        let date = new Date();
        let xhr = new Xhr(this.accessTokenPath, 'POST', data);
        xhr.setRequestHeader('Content-Type', 'application/x-www-form-urlencoded; charset=utf-8');
        return xhr.execute().then(result => {
            let info = new TokenInfo();
            info.token = result.access_token;
            info.login = login;
            date.setSeconds(date.getSeconds() + result.expires_in);
            info.expireDate = date;
            return info;
        });

    }

    public getTokenInfo(): TokenInfo {
        let tokenInfoJson = localStorage.getItem(this.accessTokenInfoStorageKey);
        if (!tokenInfoJson) {
            return null;
        }

        let tokenInfo = TokenInfo.fromJson(tokenInfoJson);
        return tokenInfo;
    }

    public addAccessTokenToRequestHeader(xhr: Xhr): Xhr{
        let tokenInfo = this.getTokenInfo();
        if (!tokenInfo) {
            return xhr;
        }
        xhr.setRequestHeader('Authorization', `Bearer ${tokenInfo.token}`);
        return xhr;
    }

    public authorize(login: string, password: string): Promise<TokenInfo> {
        return this.requestNewAccessToken(login, password).then((tokenInfo: TokenInfo) => {
            localStorage.setItem(this.accessTokenInfoStorageKey, tokenInfo.toJson());
            cookie.setCookie(this.accessTokenCookieName, tokenInfo.token, new CookieOptions(tokenInfo.expireDate));
            this.tokenInfoChanged(tokenInfo);
            return tokenInfo;
        });
    }

    public clearTokenInfo(): void {
        localStorage.removeItem(this.accessTokenInfoStorageKey);
        cookie.deleteCookie(this.accessTokenCookieName);
        this.tokenInfoChanged(null);
    }


    private tokenInfoChangedHandlers: Array<(info: TokenInfo) => void> = [];
    public onTokenInfoChanged(handler: (info: TokenInfo) => void): () => void {
        if (!handler) {
            return noop;
        }

        this.tokenInfoChangedHandlers.push(handler);
        return () => {
            let index = this.tokenInfoChangedHandlers.indexOf(handler);
            if (index > -1) {
                this.tokenInfoChangedHandlers.splice(index, 1);
            }
        };
    }

    private tokenInfoChanged(newInfo: TokenInfo): void {
        this.tokenInfoChangedHandlers.forEach(handler => handler(newInfo));
    }

}

export const auth = new Auth('accessTokenInfo', 'accessToken');