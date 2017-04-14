import { connection } from 'blades/webApi/serverConnection';
import { auth } from 'blades/webApi/auth';
import { JsonOperation } from 'blades/webApi/serverOperations';
debugger;


class TestEntityState {
    public id: string;

    public num: number;

    public name: string;
};

auth.authorize('someUser', 'w').then(() => {
    //var entity = new TestEntityState();
    //entity.name = Math.random().toFixed(5);
    //entity.num = -5;
    //new JsonOperation('SaveTestEntity', entity).execute().then((id: string) => {
    //    debugger;
    //    entity.id = id;
    //    entity.name = entity.name += 'aaa';
    //    return new JsonOperation('SaveTestEntity', entity).execute();
    //}).then((id: string) => {
    //    entity.name = entity.name += 'b';
    //    return new JsonOperation('SaveTestEntity', entity).execute();
    //}).then((id: string) => {
    //    entity.name = entity.name += 'b';
    //    return new JsonOperation('SaveTestEntity', entity).execute();
    //}).then((id: string) => {
    //    entity.name = entity.name += 'b';
    //    return new JsonOperation('SaveTestEntity', entity).execute();
    //}).then((id: string) => {
    //    entity.name = entity.name += 'b';
    //    return new JsonOperation('SaveTestEntity', entity).execute();
    //}).then((id: string) => {
    //    entity.name = entity.name += 'b';
    //    return new JsonOperation('SaveTestEntity', entity).execute();
    //}).then((id: string) => {
    //    return new JsonOperation('AlterTestEntity', entity.id).execute();
    //}).then((id: string) => {
    //    entity.name = entity.name += 'b';
    //    return new JsonOperation('SaveTestEntity', entity).execute();
    //}).then((id: string) => {
    //    return new JsonOperation('AlterTestEntity', entity.id).execute();
    //});

    debugger;
    new JsonOperation('TestEntityTestCommand', { someData: -66 }).execute();

});