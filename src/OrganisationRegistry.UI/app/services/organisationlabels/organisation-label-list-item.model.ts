export class OrganisationLabelListItem {
  constructor(
    public organisationLabelId: string = '',
    public labelId: string = '',
    public labelTypeName: string = '',
    public validFrom: Date,
    public validTo: Date,
    public labelValue: string,
    public isEditable: boolean,
  ) { }
}
