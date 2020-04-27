import { ICrudItem } from 'core/crud';

export class SeatType implements ICrudItem<SeatType> {
  constructor(
    public id: string = '',
    public name: string = '',
    public order: number = 1,
    public isEffective: boolean = false,
  ) { }
}
