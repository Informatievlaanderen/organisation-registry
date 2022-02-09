import { Component, OnInit, OnDestroy } from '@angular/core';
import { ActivatedRoute, Params, Routes } from '@angular/router';

import { AlertBuilder, AlertService } from 'core/alert';
import { BaseAlertMessages } from 'core/alertmessages';

import { BodyInfoService } from 'services/bodyinfo';
import { Body } from 'services/bodies';
import {Subscription} from "rxjs/Subscription";

@Component({
  templateUrl: 'detail.template.html',
  styleUrls: ['detail.style.css']
})
export class BodyDetailComponent implements OnInit, OnDestroy {
  public body: Body;

  private readonly subscriptions: Subscription[] = new Array<Subscription>();

  constructor(
    private route: ActivatedRoute,
    private alertService: AlertService,
    private store: BodyInfoService
  ) {
    this.body = new Body();
  }

  ngOnInit() {
    this.subscriptions.push(this.store
      .bodyChanged
      .subscribe(body => this.body = body));

    this.route.params.forEach((params: Params) => {
      let id = params['id'];
      this.store.loadBody(id);
    });
  }

  ngOnDestroy() {
    this.subscriptions.forEach(sub => sub.unsubscribe());
  }
}
