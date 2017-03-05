export class StorageTabsCommunicator {
    private communicationItemKey: string = 'StorageTabsCommunicator.communicationItemKey';
    private tabFocusEventKey: string = 'StorageTabsCommunicator.tabFocusEventKey';


    private seed(): number {
        return Date.now() + Math.random();
    }


    private isActive: boolean;
    public isActiveTab(): boolean {
        return this.isActive;
    }


    public sendDataToOtherTabs(data: any): void {
        let sendItem = {
            data: data, seed: this.seed()
        };
        localStorage.setItem(this.communicationItemKey, JSON.stringify(sendItem));
    }

    private isActivated: boolean;

    private receiveDataHandlers: Array<(data: any) => void> = [];
    public receiveData(handler: (data: any) => void): () => void {
        this.receiveDataHandlers.push(handler);
        return () => {
            let index = this.receiveDataHandlers.indexOf(handler);
            if (index > -1) {
                this.receiveDataHandlers.splice(index, 1);
            }
        }
    }

    private fireReceiveDataHandlers(data: any): void {
        this.receiveDataHandlers.forEach(handler => handler(data));
    }

    constructor(id: string) {
        this.communicationItemKey += id;
        this.tabFocusEventKey += id;

        window.addEventListener('focus', (e: FocusEvent) => {
            localStorage.setItem(this.tabFocusEventKey, this.seed().toString());
            this.isActive = true;
        });

        window.addEventListener('storage', (e: StorageEvent) => {
            if (e.key === this.communicationItemKey) {
                let data = JSON.parse(e.newValue).data;
                this.fireReceiveDataHandlers(data);
            } else if (e.key === this.tabFocusEventKey) {
                this.isActive = false;
            }
        });


    }


    private static inst: StorageTabsCommunicator = null;
    public static get instance() {
        if (StorageTabsCommunicator.inst == null) {
            StorageTabsCommunicator.inst = new StorageTabsCommunicator('');
        }
        return StorageTabsCommunicator.inst;
    }
}



export const storageTabsCommunicator = StorageTabsCommunicator.instance;