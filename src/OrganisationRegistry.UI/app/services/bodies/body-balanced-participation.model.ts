export class BodyBalancedParticipation {
  public obligatory: string;
  public extraRemark: string;
  public exceptionMeasure: string;
  public id: string;

  constructor(
    responseJson: any
  ) {
    this.id = responseJson.id;
    this.obligatory = responseJson.obligatory !== null ?
      responseJson.obligatory.toString() :
      '';

    this.exceptionMeasure = responseJson.exceptionMeasure;
    this.extraRemark = responseJson.extraRemark;
  }
}
