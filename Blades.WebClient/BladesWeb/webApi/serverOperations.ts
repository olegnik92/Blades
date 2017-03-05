import { Xhr } from './xhr';
import { Promise } from 'es6-promise';
import { json } from '../tools/json';

export abstract class BaseOperation {

    protected name: string;

    protected data: any;

    protected requestMethod: string;

    protected requestType: string;

    protected requestUrl: string = '/api/operation';

    public constructor(name: string, data: any, requestType: string, requestMethod: string) {
        this.name = name;
        this.data = data;
        this.requestMethod = requestMethod;
        this.requestType = requestType;
    }


    protected createXhr(): Xhr {
        let xhr = new Xhr(this.requestUrl, this.requestMethod, this.data);
        xhr.setRequestHeader('x-blades-operation-name', this.name);
        xhr.setRequestHeader('x-blades-operation-request-type', this.requestType);
        return xhr;
    }
}


export class JsonOperation extends BaseOperation {

    public constructor(name: string, data: Object) {
        let strData = json.stringify(data);
        super(name, strData, 'JsonOperation', 'POST');
    }

    public execute(): Promise<any> {
        let xhr = this.createXhr();
        xhr.setRequestHeader('Content-Type', 'application/json; charset=utf-8');
        return xhr.execute();
    }
}


export class FormDataOperation extends BaseOperation {

    public constructor(name: string, data: FormData) {
        super(name, data, 'FormDataOperation', 'POST');
    }
}