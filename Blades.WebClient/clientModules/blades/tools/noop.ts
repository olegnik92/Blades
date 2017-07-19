
const noop = () => { };
export default noop;

export function echo(data: any): any {
    return data;
};

export function echoPromise(data: any): any {
    return new Promise((res, rej) => {
        res(data);
    });
};
