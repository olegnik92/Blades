import noop from './noop';

class SyncActionsChain<T> {

    private chain: Array<(data: T) => T>;

    public constructor() {
        this.chain = [];
    }

    public addLast(item: (data: T) => T): () => void {
        return this.add(item, true);
    }

    public addFirst(item: (data: T) => T): () => void {
        return this.add(item, false);
    }

    private add(item: (data: T) => T, toEnd: boolean): () => void {
        if (typeof item !== 'function') {
            return noop;
        }

        if (toEnd) {
            this.chain.push(item);
        } else {
            this.chain.unshift(item);
        }
        
        return () => {
            const index = this.chain.indexOf(item);
            if (index > -1) {
                this.chain.splice(index, 1);
            }
        };
    }

    public run(data: T): T {
        for (let i = 0; i < this.chain.length; i++) {
            data = this.chain[i](data);
        }

        return data;
    }
};

export default SyncActionsChain;