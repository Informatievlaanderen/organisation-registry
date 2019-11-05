import { Injectable } from '@angular/core';
import { saveAs } from 'file-saver';

@Injectable()
export class FileSaverService {

  saveFile(text: string, name: string): void {
    let realName = `${name}_${this.dateToYMDHMS(new Date())}.csv`;
    let file = new Blob([text], { type: 'text/plain;charset=utf-8' });
    saveAs(file, realName);
  }

  // Taken from http://stackoverflow.com/a/19449076/367388
  // TODO: Consider using moment.js if we start doing lots of date-based stuff.
  private dateToYMDHMS(date: Date): string {
    return date.getFullYear() +
      this.pad2(date.getMonth() + 1) +
      this.pad2(date.getDate()) +
      '_' +
      this.pad2(date.getHours()) +
      this.pad2(date.getMinutes()) +
      this.pad2(date.getSeconds());
  }

  private pad2(n): string {
    return (n < 10 ? '0' : '') + n;
  }
}
