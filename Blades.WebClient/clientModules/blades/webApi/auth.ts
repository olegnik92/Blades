import { Promise } from 'es6-promise';
import TokenInfo from './tokenInfo';
import Xhr from './xhr';
import noop from '../tools/noop';
import ITempStorage from '../storages/tempStorage';
import cookieStorage from '../storages/cookieStorage';

export class Auth {

    public readonly accessTokenApiPath: string = '/token';

    constructor(private accessTokenKey: string, private storage: ITempStorage) {
    }

    public requestNewAccessToken(login: string, password: string): Promise<TokenInfo> {
        let data = `grant_type=password&username=${login}&password=${password}`;
        let date = new Date();
        let xhr = new Xhr(this.accessTokenApiPath, 'POST', data);
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

    private get accessTokenInfoKey() {
        return `${this.accessTokenKey}__tokenInfo`;
    }

    public getTokenInfo(): TokenInfo {
        const tokenInfo = <TokenInfo>this.storage.get(this.accessTokenInfoKey);
        if (!tokenInfo) {
            return null;
        }

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
            this.storage.set(this.accessTokenInfoKey, tokenInfo, tokenInfo.expireDate);
            this.storage.setStr(this.accessTokenKey, tokenInfo.token, tokenInfo.expireDate);
            this.tokenInfoChanged(tokenInfo);
            return tokenInfo;
        });
    }

    public clearTokenInfo(): void {
        this.storage.remove(this.accessTokenInfoKey);
        this.storage.remove(this.accessTokenKey);
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


    public setStorage(storage: ITempStorage): void {
        this.storage = storage;
    }
};

const auth = new Auth('accessToken', cookieStorage);
export default auth;