import { ICrudItem } from 'core/crud';

export class ConfigurationValue implements ICrudItem<ConfigurationValue> {
  public id: string;
  public name: string;

  constructor(
    public key: string = '',
    public description: string = '',
    public value: string = ''
  ) {
    this.id = key;
    this.name = key;
  }
}
