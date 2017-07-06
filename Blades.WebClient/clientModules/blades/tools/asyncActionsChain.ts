import { Promise } from 'es6-promise';
import noop, { echoPromise } from './noop';


class AsyncActionsChain<T> {

    private chain: Array<(data: T) => Promise<T>>;

    private asyncAction: (data: T) => Promise<T>;

    public constructor() {
        this.chain = [];
        this.asyncAction = this.buildChain();
    }


    public addLast(item: (data: T) => Promise<T>): () => void {
        return this.add(item, true);
    }

    public addFirst(item: (data: T) => Promise<T>): () => void {
        return this.add(item, false);
    }

    private add(item: (data: T) => Promise<T>, toEnd: boolean): () => void {
        if (typeof item !== 'function') {
            return noop;
        }

        if (toEnd) {
            this.chain.push(item);
        } else {
            this.chain.unshift(item);
        }

        this.asyncAction = this.buildChain();
        return () => {
            const index = this.chain.indexOf(item);
            if (index > -1) {
                this.chain.splice(index, 1);
                this.asyncAction = this.buildChain();
            }
        };
    }

    public run(data: T): Promise<T> {
        return this.asyncAction(data);
    }


    private buildChain(): (data: T) => Promise<T> {
        let action = (<(d: T) => Promise<T>>echoPromise);
        if (this.chain.length === 0) {
            return action;
        }

        for (let i = 0; i < this.chain.length; i++) {
            const oldAction = action;
            action = ((data: T) => oldAction(data).then(this.chain[i]));
        }

        return action;
    }
};

export default AsyncActionsChain;