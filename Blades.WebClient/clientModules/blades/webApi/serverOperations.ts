import Xhr from './xhr';
import connection from './serverConnection';
import json from '../tools/json';


abstract class BaseOperation {

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


export default BaseOperation;

export class JsonOperation extends BaseOperation {

    public constructor(name: string, data: Object) {
        let strData = json.stringify(data);
        super(name, strData, 'json', 'POST');
    }

    public execute(): Promise<any> {
        let xhr = this.createXhr();
        xhr.setRequestHeader('Content-Type', 'application/json; charset=utf-8');
        return xhr.execute();
    }


    public executeViaConnection(): void {
        let message = {
            name: this.name,
            requestType: this.requestType,
            data: this.data
        };

        let messageStr = `${this.requestType}@@@@@${this.name}@@@@@${this.data}`;
        connection.send(messageStr);
    }
}


export class FormDataOperation extends BaseOperation {

    public constructor(name: string, data: FormData) {
        super(name, data, 'FormDataOperation', 'POST');
    }
}