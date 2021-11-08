export class RegulationSubThemeListItem {
  constructor(
    public id: string = '',
    public name: string = '',
    public order: number = 1,
    public externalKey: string = '',
    public active: boolean = true,
    public regulationThemeId: string = '',
    public regulationThemeName: string = ''
  ) { }
}
