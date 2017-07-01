import { Promise } from 'es6-promise';


class Barrier {

    private timeoutId: number = 0;
    private deferred: Promise<any> = null;
    public constructor(private waitTime: number) {

    }


    public exec(action: () => any): void {
        if (this.timeoutId) {
            this.cancelWaitAction();
        }


        if (this.isWaitMode()) {
            this.executeWaitAction(action);
        } else {
            action();
        }
    } 


    private isWaitMode(): boolean {
        return this.waitTime >= 0;
    }

    private cancelWaitAction(): void {
        clearTimeout(this.timeoutId);
        this.timeoutId = 0;
    }

    private executeWaitAction(action: () => any): void {
        this.timeoutId = setTimeout(() => {
            action();
            this.timeoutId = 0;
        }, this.waitTime);
    };


    public execAsync(action: () => Promise<any>): void {
        if (this.timeoutId) {
            this.cancelWaitAction();
        }

        if (this.isWaitMode()) {
            this.addToDeferredWithWait(action);
        } else {
            this.addToDeferred(action);
        }
    };


    private addToDeferred(action: () => Promise<any>): void {
        if (this.deferred == null) {
            this.deferred = new Promise((res, rej) => {
                action().then(res).catch(rej);
            });
        } else {
            this.deferred = this.deferred.then(() => {
                return action();
            });
        }
    };


    private addToDeferredWithWait(action: () => Promise<any>): void {
        this.timeoutId = setTimeout(() => {
            this.addToDeferred(action);
            this.timeoutId = 0;
        }, this.waitTime);
    };

};

export default Barrier;