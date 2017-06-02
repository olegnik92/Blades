import { Promise } from 'es6-promise';
import json from '../tools/json';
import RequestExecutionError from './RequestExecutionError';


export default class Xhr {
    public reqUrl: string;
    public reqMethod: string;
    public reqBody: any;

    //http://stackoverflow.com/questions/40794571/failed-to-construct-xmlhttprequest-please-use-the-new-operator-this-dom-ob
    //attempt to extend failed with es5 target, so only wrapp(
    private xhr: XMLHttpRequest;

    public get innerXhr(): XMLHttpRequest {
        return this.xhr;
    }

    public get status(): number {
        return this.xhr.status;
    }

    public get statusText(): string {
        return this.xhr.statusText;
    }

    public get response(): any {
        return this.xhr.response;
    }

    public get responseText(): string {
        return this.xhr.responseText;
    } 

    public get readyState(): number {
        return this.xhr.readyState;
    }

    public setRequestHeader(header: string, value: string): void {
        this.xhr.setRequestHeader(header, value);
    }

    constructor(url: string, method: string = 'GET', body?: any) {
        this.xhr = new XMLHttpRequest();

        this.reqUrl = url;
        this.reqMethod = method;
        this.reqBody = body;

        this.xhr.open(this.reqMethod, this.reqUrl, true);
    }

    public rawExecute(): Promise<Xhr> {
        return new Promise<Xhr>((res, rej) => {
            this.xhr.send(this.reqBody);
            this.xhr.onreadystatechange = (() => {
                if (this.xhr.readyState !== XMLHttpRequest.DONE) {
                    return;
                }

                res(this);
            });

            this.xhr.onerror = ((ev: ErrorEvent) => {
                rej(this);
            });
        });
    }


    private static beforeExecutionChain: (xhr: Xhr) => Promise<Xhr> =
    (xhr) => new Promise<Xhr>(res => res(xhr));

    private static afterExecutionChain: (xhr: Xhr) => Promise<Xhr> =
    (xhr) => new Promise<Xhr>(res => res(xhr));


    public static beforeExecution(hook: (xhr: Xhr) => Promise<Xhr>): void {
        const next = Xhr.beforeExecutionChain;
        Xhr.beforeExecutionChain = ((xhr) => hook(xhr).then(next));
    }

    public static afterExecution(hook: (xhr: Xhr) => Promise<Xhr>): void {
        const next = Xhr.afterExecutionChain;
        Xhr.afterExecutionChain = ((xhr) => hook(xhr).then(next));
    }

    private static createResData(xhr: Xhr): any {
        if (xhr.readyState !== XMLHttpRequest.DONE) {
            throw new Error('Try to parse XHR in incorrect state');
        }

        const data = json.parse(xhr.responseText);
        return data;
    }

    public execute(): Promise<any> {
        return new Promise<any>((res, rej) => {
            Xhr.beforeExecutionChain(this)
                .then(xhr => xhr.rawExecute())
                .then(xhr => Xhr.afterExecutionChain(xhr))
                .then(xhr => {
                    if (xhr.status !== 200) {
                        rej(new RequestExecutionError(xhr));
                        return; 
                    }

                    let data = Xhr.createResData(xhr);
                    res(data);
                })
                .catch(err => rej(new RequestExecutionError(err)));
        });
    }
}