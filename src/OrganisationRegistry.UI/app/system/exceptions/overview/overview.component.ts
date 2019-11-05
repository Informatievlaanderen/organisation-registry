import { Component, OnInit } from '@angular/core';

import { Observable } from 'rxjs/Observable';

import { ExceptionsService, Exception } from 'services/exceptions';

@Component({
  templateUrl: 'overview.template.html',
  styleUrls: [ 'overview.style.css' ]
})
export class ExceptionOverviewComponent implements OnInit {
  public exceptions: Exception[];

  constructor(
    private exceptionsService: ExceptionsService
  ) {}

  ngOnInit() {
    this.exceptionsService
      .getAllExceptions()
      .subscribe(result => this.exceptions = result);
  }
}
