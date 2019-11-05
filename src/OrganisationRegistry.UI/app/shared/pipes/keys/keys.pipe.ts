import { Pipe, PipeTransform } from '@angular/core';

@Pipe({name: 'keys'})
export class KeysPipe implements PipeTransform {
  transform(value, ...args: any[]): any {
    let keys = [];
    for (let key in value) {
          if (value.hasOwnProperty(key))
            keys.push({key: key, value: value[key]});
    }
    return keys;
  }
}
