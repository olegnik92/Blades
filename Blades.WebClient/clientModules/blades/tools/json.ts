
//date parse method from https://weblog.west-wind.com/posts/2014/jan/06/javascript-json-date-parsing-and-real-dates
const reISO = /^(\d{4})-(\d{2})-(\d{2})T(\d{2}):(\d{2}):(\d{2}(?:\.\d*))(?:Z|(\+|-)([\d|:]*))?$/;
const reMsAjax = /^\/Date\((d|-|.*)\)[\/|\\]$/;

const parseMsAjaxDate = false;

const dateParser = function (key: string, value: any) {
    /// <summary>
    /// Globally enables JSON date parsing for JSON.parse().
    /// Replaces the default JSON.parse() method and adds
    /// the datePaser() extension to the processing chain.
    /// </summary>    
    /// <param name="key" type="string">property name that is parsed</param>
    /// <param name="value" type="any">property value</param>
    /// <returns type="date">returns date or the original value if not a date string</returns>
    if (typeof value === 'string') {
        var a = reISO.exec(value);
        if (a)
            return new Date(value);

        if (!parseMsAjaxDate)
            return value;

        a = reMsAjax.exec(value);
        if (a) {
            var b = a[1].split(/[-+,.]/);
            return new Date(b[0] ? +b[0] : 0 - +b[1]);
        }
    }
    return value;
};


const json = { 

    parse(str: string): any {
        try {
            let res = JSON.parse(str, dateParser);
            return res;
        } catch (e) {
            // orignal error thrown has no error message so rethrow with message
            throw new Error("JSON content could not be parsed");
        }
    },

    stringify(data: any): string {
        return JSON.stringify(data);
    }
};

export default json;