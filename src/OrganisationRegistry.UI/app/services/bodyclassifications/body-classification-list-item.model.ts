export class BodyClassificationListItem {
  constructor(
    public id: string = '',
    public name: string = '',
    public order: number = 1,
    public active: boolean = true,
    public bodyClassificationTypeId: string = '',
    public bodyClassificationTypeName: string = ''
  ) { }
}
