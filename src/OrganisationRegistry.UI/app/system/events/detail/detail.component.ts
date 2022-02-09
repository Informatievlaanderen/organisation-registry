import {Component, ElementRef, OnDestroy, OnInit} from '@angular/core';
import { ActivatedRoute, Params, Router } from '@angular/router';
import { FormBuilder, FormGroup } from '@angular/forms';

import { AlertService, AlertBuilder } from 'core/alert';
import { UpdateAlertMessages } from 'core/alertmessages';
import { Create, ICrud, Update } from 'core/crud';

import { EventData, EventService } from 'services/events';
import {Subscription} from "rxjs/Subscription";

@Component({
  templateUrl: 'detail.template.html',
  styleUrls: ['detail.style.css']
})
export class EventDataDetailComponent implements OnInit, OnDestroy {
  public isBusy: boolean = true;
  public eventData: EventData;

  private readonly alerts = new UpdateAlertMessages('Events');

  private readonly subscriptions: Subscription[] = new Array<Subscription>();

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private alertService: AlertService,
    private eventService: EventService
  ) {
    this.eventData = new EventData();
  }

  ngOnInit() {
    this.route.params.forEach((params: Params) => {
      let id = params['id'];

      this.isBusy = false;

      this.subscriptions.push(this.eventService
        .get(id)
        .finally(() => this.isBusy = false)
        .subscribe(
          item => {
            if (item)
              this.eventData = item;
          },
          error => {
            let alert = this.alerts.loadError;

            this.alertService.setAlert(
              new AlertBuilder()
                .error(error)
                .withTitle(alert.title)
                .withMessage(alert.message)
                .build());
          }));
    });
  }

  ngOnDestroy() {
    this.subscriptions.forEach(sub => sub.unsubscribe());
  }
}
