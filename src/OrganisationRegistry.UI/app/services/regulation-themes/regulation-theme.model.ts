import { ICrudItem } from 'core/crud';

export class RegulationTheme implements ICrudItem<RegulationTheme> {
  constructor(
    public id: string = '',
    public name: string = ''
  ) { }
}
