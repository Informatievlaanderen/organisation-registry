import {Component, OnDestroy, OnInit} from '@angular/core';

import { Observable } from 'rxjs/Observable';

import { ExceptionsService, Exception } from 'services/exceptions';
import {Subscription} from "rxjs/Subscription";

@Component({
  templateUrl: 'overview.template.html',
  styleUrls: [ 'overview.style.css' ]
})
export class ExceptionOverviewComponent implements OnInit, OnDestroy {
  public exceptions: Exception[];

  private readonly subscriptions: Subscription[] = new Array<Subscription>();

  constructor(
    private exceptionsService: ExceptionsService
  ) {}

  ngOnInit() {
    this.subscriptions.push(this.exceptionsService
      .getAllExceptions()
      .subscribe(result => this.exceptions = result));
  }

  ngOnDestroy() {
    this.subscriptions.forEach(sub => sub.unsubscribe());
  }
}
