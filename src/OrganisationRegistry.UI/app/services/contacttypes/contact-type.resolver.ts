import { Injectable } from '@angular/core';
import { Resolve, ActivatedRouteSnapshot } from '@angular/router';

import { ContactTypeService } from './contact-type.service';
import { ContactTypeListItem } from './contact-type-list-item.model';

@Injectable()
export class ContactTypeResolver implements Resolve<ContactTypeListItem[]> {

  constructor(
    private contactTypeService: ContactTypeService
  ) { }

  resolve(route: ActivatedRouteSnapshot) {
    return this.contactTypeService.getAllContactTypes();
  }
}
