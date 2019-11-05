import { Component, OnInit, OnDestroy } from '@angular/core';
import { ActivatedRoute, Params, Routes } from '@angular/router';

import { AlertBuilder, AlertService } from 'core/alert';
import { BaseAlertMessages } from 'core/alertmessages';

import { PersonInfoService } from 'services/peopleinfo';
import { Person } from 'services/people';

@Component({
  templateUrl: 'detail.template.html',
  styleUrls: ['detail.style.css']
})
export class PeopleDetailComponent implements OnInit, OnDestroy {
  public person: Person;

  private readonly alertMessages: BaseAlertMessages = new BaseAlertMessages('Persoon');

  constructor(
    private route: ActivatedRoute,
    private alertService: AlertService,
    private store: PersonInfoService
  ) {
    this.person = new Person();
  }

  ngOnInit() {
    this.store
      .personChanged
      .subscribe(pers => this.person = pers);

    this.route.params.forEach((params: Params) => {
      let id = params['id'];
      this.store.loadPerson(id);
    });
  }

  ngOnDestroy() {
  }
}
