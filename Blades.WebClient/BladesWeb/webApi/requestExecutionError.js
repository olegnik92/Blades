"use strict";
var RequestExecutionError = (function () {
    function RequestExecutionError(source) {
        this.status = typeof (source.status) === 'number' ? source.status : 499;
        this.statusText = typeof (source.statusText) === 'string' ? source.statusText : 'UNKNOWN';
        this.message = typeof (source.responseText) === 'string' ? source.responseText :
            (typeof (source.message) === 'string' ? source.message : 'Неизвестная ошибка');
        this.message = this.processJson(this.message);
    }
    RequestExecutionError.prototype.processJson = function (message) {
        var result = message;
        try {
            var mesObject = JSON.parse(message);
            if (typeof (mesObject.error) === 'string') {
                result = mesObject.error;
            }
            if (typeof (mesObject.error_description) === 'string') {
                result = mesObject.error_description;
            }
        }
        catch (e) {
        }
        return result;
    };
    return RequestExecutionError;
}());
exports.RequestExecutionError = RequestExecutionError;
//# sourceMappingURL=RequestExecutionError.js.map