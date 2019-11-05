import { ICrudItem } from 'core/crud';

export class EventData implements ICrudItem<EventData> {
  constructor(
    public id: string = '',
    public name: string = '',
    public timestamp: string = '',
    public lastName: string = '',
    public firstName: string = '',
    public version: string = '',
    public ip: string = '',
    public userId: string = '',
    public number: string = '',
    public data: string = '',
  ) { }
}
