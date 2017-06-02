export default class RequestExecutionError {
    public status: number;
    public statusText: string;
    public message: string;

    constructor(source: any) {
        this.status = typeof (source.status) === 'number' ? source.status : 499;
        this.statusText = typeof (source.statusText) === 'string' ? source.statusText : 'UNKNOWN';
        this.message = typeof (source.responseText) === 'string' ? source.responseText :
            (typeof (source.message) === 'string' ? source.message : 'Неизвестная ошибка');

        this.message = this.processJson(this.message);
    }

    private processJson(message: string): string {
        let result = message;
        try {
            let mesObject = JSON.parse(message);
            if (typeof(mesObject.error) === 'string') {
                result = mesObject.error;
            }

            if (typeof (mesObject.error_description) === 'string') {
                result = mesObject.error_description;
            }
        } catch (e) {
            //ignore
        }

        return result;
    }
};