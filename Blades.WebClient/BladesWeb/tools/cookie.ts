

export class CookieOptions {
    constructor(expires?: any) {
        this.expires = expires;
        this.path = '/';
    }

    public expires: any;
    public path: string;
    public domain: string;
    public secure: boolean;
}


export class CookieApi {

    getCookie(name: string): string {
        let matches = document.cookie.match(new RegExp(
            "(?:^|; )" + name.replace(/([\.$?*|{}\(\)\[\]\\\/\+^])/g, '\\$1') + "=([^;]*)"
        ));
        return matches ? decodeURIComponent(matches[1]) : undefined;
    }


    setCookie(name: string, value: string, options: CookieOptions): void {
        options = options || new CookieOptions();

        let expires = options.expires;

        if (typeof expires == "number" && expires) {
            let d = new Date();
            d.setTime(d.getTime() + expires * 1000);
            expires = options.expires = d;
        }
        if (expires && expires.toUTCString) {
            options.expires = expires.toUTCString();
        }

        value = encodeURIComponent(value);

        var updatedCookie = name + "=" + value;

        for (var propName in options) {
            updatedCookie += "; " + propName;
            var propValue = options[propName];
            if (propValue !== true) {
                updatedCookie += "=" + propValue;
            }
        }

        document.cookie = updatedCookie;
    }


    deleteCookie(name: string):void {
        this.setCookie(name, '', new CookieOptions(-1));
    }
}

export const cookie = new CookieApi();