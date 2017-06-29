import json from '../tools/json';

export default class TokenInfo {
    public login: string;
    public token: string;
    public expireDate: Date;

    public static fromJson(jsonStr: string): TokenInfo {
        let obj = json.parse(jsonStr);
        if (!obj) {
            return null;
        }

        let result = new TokenInfo();
        result.login = obj.login;
        result.token = obj.token;
        result.expireDate = new Date(obj.expireDate);
        return result;
    }
}