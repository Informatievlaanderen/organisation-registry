import { Component, ElementRef, Input, ViewChild } from '@angular/core';

@Component({
  selector: 'smart-link',
  styleUrls: ['./smart-link.style.css'],
  templateUrl: 'smart-link.template.html'
})
export class SmartLinkComponent {
  @ViewChild('mySmartLink') input: ElementRef;

  private _value: string;
  public get value(): string {
    return this._value;
  }

  @Input('value')
  public set value(value: string) {
    if (value === this._value)
      return;

    if (this.urlRegex.test(value)) {
      this._value = this.urlify(value);
    } else if (this.emailRegex.test(value)) {
      this._value = this.emailify(value);
    } else {
      this._value = value;
    }
  }

  private urlRegex = /^(((https?:\/\/)|(www\.))[^\s]+)$/ig;
  private emailRegex = /^([A-Z0-9._%+-]+@[A-Z0-9.-]+\.[A-Z]{2,})$/ig;

  private urlify(text) {
    return text.replace(this.urlRegex, function (url, b, c) {
      let url2 = (c === 'www.') ? 'http://' + url : url;
      return '<a href="' + url2 + '" target="_blank">' + url + '</a>';
    });
  }

  private emailify(text) {
    return text.replace(this.emailRegex, function (email) {
      return '<a href="mailto:' + email + '" target="_blank">' + email + '</a>';
    });
  }
}

