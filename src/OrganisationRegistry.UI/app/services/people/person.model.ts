import { ICrudItem } from 'core/crud';

export class Person implements ICrudItem<Person> {
  constructor(
    public id: string = '',
    public firstName: string = '',
    public name: string = '',
    public dateOfBirth: Date = null,
    public sex = '',
    public fullName = '',
  ) { }
}
