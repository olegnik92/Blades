import { connection } from 'blades-web/webApi/serverConnection';
import { auth } from 'blades-web/webApi/auth';
import { JsonOperation } from 'blades-web/webApi/serverOperations';
debugger;


class TestEntityState {
    public id: string;

    public num: number;

    public name: string;
};

auth.authorize('someUser', 'w').then(() => {
    var entity = new TestEntityState();
    entity.name = Math.random().toFixed(5);
    entity.num = -5;
    new JsonOperation('SaveTestEntity', entity).execute().then((id: string) => {
        debugger;
        entity.id = id;
        entity.name = entity.name += 'aaa';
        return new JsonOperation('SaveTestEntity', entity).execute();
    }).then((id: string) => {
        entity.name = entity.name += 'b';
        return new JsonOperation('SaveTestEntity', entity).execute();
    }).then((id: string) => {
        entity.name = entity.name += 'b';
        return new JsonOperation('SaveTestEntity', entity).execute();
    }).then((id: string) => {
        entity.name = entity.name += 'b';
        return new JsonOperation('SaveTestEntity', entity).execute();
    }).then((id: string) => {
        entity.name = entity.name += 'b';
        return new JsonOperation('SaveTestEntity', entity).execute();
    }).then((id: string) => {
        entity.name = entity.name += 'b';
        return new JsonOperation('SaveTestEntity', entity).execute();
    }).then((id: string) => {
        return new JsonOperation('AlterTestEntity', entity.id).execute();
    }).then((id: string) => {
        entity.name = entity.name += 'b';
        return new JsonOperation('SaveTestEntity', entity).execute();
    }).then((id: string) => {
        return new JsonOperation('AlterTestEntity', entity.id).execute();
    });

});