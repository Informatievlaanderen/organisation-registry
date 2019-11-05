export enum AlertType {
  Success,
  Warning,
  Error
}

export class Alert {
  private _type: AlertType;
  private _title: string;
  private _message: string;
  private _link: string;

  public get type(): AlertType {
    return this._type;
  }

  public get title(): string {
    return this._title;
  }

  public get message(): string {
    return this._message;
  }

  public get link(): string {
    return this._link;
  }

  constructor(type: AlertType, title: string, message?: string, link?: string) {
    this._type = type;
    this._title = title;
    this._message = message;
    this._link = link;
  }
}

export class AlertBuilder {
  private _type: AlertType;
  private _title: string;
  private _message: string;
  private _link: string;
  private _errorTitle: string;
  private _errorMessage: string;

  success(): AlertBuilder {
    this._type = AlertType.Success;
    return this;
  }

  alert(): AlertBuilder {
    this._type = AlertType.Warning;
    return this;
  }

  error(err: any = null): AlertBuilder {
    if (err) {
      let error = err.json();
      if (!err.ok && err.status === 0) {
        this._errorTitle = 'Verbinding verbroken!';
        this._errorMessage = 'Er kan geen verbinding gemaakt worden met de server. Probeer het later opnieuw.';
      } else if ('title' in error && 'detail' in error) {
        this._errorTitle = error.title;
        this._errorMessage = error.detail;
      } else {
        this._errorTitle = 'Deze actie is niet geldig!';
        this._errorMessage = Object.keys(error)
          .map(key => error[key])
          .reduce((a, b) => a.concat(b))
          .join();
      }
    }

    this._type = AlertType.Error;
    return this;
  }

  withTitle(title: string) {
    this._title = title.trim();
    return this;
  }

  withMessage(message: string) {
    this._message = message.trim();
    return this;
  }

  linkTo(link: string) {
    this._link = link.trim();
    return this;
  }

  build(): Alert {
    return new Alert(
      this._type,
      this._errorTitle || this._title,
      this._errorMessage || this._message,
      this._link
    );
  }
}
