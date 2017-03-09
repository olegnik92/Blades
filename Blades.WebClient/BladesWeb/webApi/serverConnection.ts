import { json } from '../tools/json';
import { noop } from '../tools/noop';

export class ServerConnection {

    public connectionProtocol: string = 'ws'; //or wss

    public connectionRoute: string = 'ws';

    private ws: WebSocket;


    private reconnectTimeout: number = 15000;
    private reconnectIntervalRef: number = 0; 

    private sendMessagesTimeout: number =  100;
    private sendMessagesIntervalRef: number = 0; 

    public open(): void {
        if (this.isOpen) {
            return;
        }

        this.connect();

        clearInterval(this.reconnectIntervalRef);
        this.reconnectIntervalRef = setInterval(() => {
            if (!this.ws || this.ws.readyState !== WebSocket.OPEN) {
                this.connect();
            }
        }, this.reconnectTimeout);
    }


    public close(): void {
        if (this.ws) {
            this.ws.close();
        }

        clearInterval(this.reconnectIntervalRef);
    }

    public get isOpen() {
        if (!this.ws) {
            return false;
        }

        return this.ws.readyState === WebSocket.OPEN;
    }

    private connect() {
        this.ws = new WebSocket(`${this.connectionProtocol}://${document.location.host}/${this.connectionRoute}`);
        this.bindEvents();

        clearInterval(this.sendMessagesIntervalRef);
        this.sendMessagesIntervalRef = setInterval(() => {
            this.sendMessagesFromQueue();
        }, this.sendMessagesTimeout);
    }

    private bindEvents(): void {
        this.ws.onclose = () => {
            clearInterval(this.sendMessagesIntervalRef);
        };

        this.ws.onmessage = (ev) => {
            this.rawMessage(ev);
            this.processNotifyMessage(ev);
        };
    }


    private messagesQueue: Array<any> = [];
    private sendMessagesFromQueue(): void {
        while (this.messagesQueue.length > 0) {
            if (this.ws.readyState !== WebSocket.OPEN) {
                break;
            }

            let data = this.messagesQueue.shift();
            this.ws.send(data);
        }
    }

    public send(data: any): void {
        this.messagesQueue.push(data);
    }



    private rawMessageHandlers: Array<(e: MessageEvent) => void> = [];
    private rawMessage(e: MessageEvent): void {
        this.rawMessageHandlers.forEach(handler => handler(e));
    }
    public onRawMessage(handler: (e: MessageEvent) => void): () => void {
        if (!handler) {
            return noop;
        }        

        this.rawMessageHandlers.push(handler);
        return () => {
            let index = this.rawMessageHandlers.indexOf(handler);
            if (index > -1) {
                this.rawMessageHandlers.splice(index, 1);
            }
        };
    }


    private notifyMessageHandlers: {
        [name: string]: Array<(data: any, attrs: any) => void>
    } = {};

    private processNotifyMessage(e: MessageEvent): void {
        try {
            let message = json.parse(e.data);
            if (message.messageRole === 'NotifyMessage'
                && message.name
                && this.notifyMessageHandlers[message.name]) {

                this.notifyMessageHandlers[message.name]
                    .forEach(handler => handler(message.data, message.attributes));
            }
        } catch (e) {
            //ignore
        }
    }

    public onNotifyMessage(name: string, handler: (data: any, attrs: any) => void): () => void {
        if (!(name && handler)) {
            return noop;
        } 

        if (!this.notifyMessageHandlers[name]) {
            this.notifyMessageHandlers[name] = [];
        }

        this.notifyMessageHandlers[name].push(handler);

        return () => {
            let index = this.notifyMessageHandlers[name].indexOf(handler);
            if (index > -1) {
                this.notifyMessageHandlers[name].splice(index, 1);
            }
        };
    }
}



export const connection = new ServerConnection();


