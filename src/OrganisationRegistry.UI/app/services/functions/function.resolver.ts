import { Injectable } from '@angular/core';
import { Resolve, ActivatedRouteSnapshot } from '@angular/router';

import { FunctionService } from './function.service';
import { FunctionListItem } from './function-list-item.model';

@Injectable()
export class FunctionResolver implements Resolve<FunctionListItem[]> {

  constructor(
    private functionService: FunctionService
  ) { }

  resolve(route: ActivatedRouteSnapshot) {
    return this.functionService.getAllFunctions();
  }
}
