import {Component, OnDestroy, OnInit} from '@angular/core';
import { ActivatedRoute, Params} from '@angular/router';

import { AlertService } from 'core/alert';

import { PersonInfoService } from 'services/peopleinfo';
import { Person } from 'services/people';
import {Subscription} from "rxjs/Subscription";

@Component({
  templateUrl: 'detail.template.html',
  styleUrls: ['detail.style.css']
})
export class PeopleDetailComponent implements OnInit, OnDestroy {
  public person: Person;

  private readonly subscriptions: Subscription[] = new Array<Subscription>();

  constructor(
    private route: ActivatedRoute,
    private alertService: AlertService,
    private store: PersonInfoService
  ) {
    this.person = new Person();
  }

  ngOnInit() {
    this.subscriptions.push(this.store
      .personChanged
      .subscribe(pers => this.person = pers));

    this.route.params.forEach((params: Params) => {
      let id = params['id'];
      this.store.loadPerson(id);
    });
  }

  ngOnDestroy() {
    this.subscriptions.forEach(sub => sub.unsubscribe());
  }
}
