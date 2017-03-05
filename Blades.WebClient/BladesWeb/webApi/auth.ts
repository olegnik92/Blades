import { Promise } from 'es6-promise';
import { TokenInfo } from './tokenInfo';
import { Xhr } from './xhr';


export class Auth {

    private accessTokenInfoItemKey: string;

    public accessTokenPath: string = '/token';


    constructor(accessTokenInfoItemKey: string) {
        this.accessTokenInfoItemKey = accessTokenInfoItemKey;
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

    public addAccessTokenToRequest(xhr: Xhr): Xhr{
        let tokenInfoJson = localStorage.getItem(this.accessTokenInfoItemKey);
        if (!tokenInfoJson) {
            return xhr;
        }

        let tokenInfo = TokenInfo.fromJson(tokenInfoJson);
        xhr.setRequestHeader('Authorization', `Bearer ${tokenInfo.token}`);
        return xhr;
    }

    public authorize(login: string, password: string): Promise<TokenInfo> {
        return this.requestNewAccessToken(login, password).then((tokenInfo: TokenInfo) => {
            localStorage.setItem(this.accessTokenInfoItemKey, tokenInfo.toJson());
            return tokenInfo;
        });
    }

    public clearTokenInfo(): void {
        localStorage.removeItem(this.accessTokenInfoItemKey);
    }
}

export const auth = new Auth('accessTokenInfo');