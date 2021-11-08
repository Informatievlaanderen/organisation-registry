import { ICrudItem } from 'core/crud';

export class RegulationSubTheme implements ICrudItem<RegulationSubTheme> {
  constructor(
    public id: string = '',
    public name: string = '',
    public regulationThemeId: string = '',
    public regulationThemeName: string = ''
  ) { }
}
