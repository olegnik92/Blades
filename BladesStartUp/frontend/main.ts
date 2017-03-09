import { connection } from 'blades-web/webApi/serverConnection';
import { auth } from 'blades-web/webApi/auth';
import { JsonOperation } from 'blades-web/webApi/serverOperations';
debugger;

auth.onTokenInfoChanged(info => {
    if (info) {

        let operation = new JsonOperation('TestOperation.PermissionedFailedOperation', 7);

        setTimeout(() => {
            let ws = new WebSocket(`ws://${document.location.host}/ws`);
            operation.execute();
        }, 3000);

        setTimeout(() => {
            let ws = new WebSocket(`ws://${document.location.host}/ws`);
            operation.execute();
        }, 10000);
    }


});

auth.authorize('someUser', 'w');