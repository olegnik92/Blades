import { Promise } from 'es6-promise';

const noop = () => { };
export default noop;

export const echo = (data: any) => data;

export const echoPromise = (data: any) => {
    return new Promise((res, rej) => {
        res(data);
    });
};
