import { Component, OnInit, OnDestroy } from '@angular/core';
import { ActivatedRoute, Params, Router } from '@angular/router';

import { Observable } from 'rxjs/Observable';
import { Subscription } from 'rxjs/Subscription';

import { AlertBuilder, AlertService, Alert, AlertType } from 'core/alert';
import { PagedEvent, PagedResult, SortOrder } from 'core/pagination';
import { BaseAlertMessages } from 'core/alertmessages';

import { Person } from 'services/people';
import { PersonInfoService } from 'services/peopleinfo';

@Component({
  templateUrl: 'overview.template.html',
  styleUrls: ['overview.style.css']
})
export class PeopleInfoOverviewComponent implements OnInit, OnDestroy {
  public isBusy = true;
  public person: Person;

  private readonly alertMessages: BaseAlertMessages = new BaseAlertMessages('Persoon');
  private id: string;

  private readonly subscriptions: Subscription[] = new Array<Subscription>();

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private alertService: AlertService,
    private store: PersonInfoService
  ) {
    this.person = new Person();
  }

  ngOnInit() {
    let personChangedObservable =
      this.store.personChanged;

    this.subscriptions.push(personChangedObservable
      .finally(() => this.isBusy = false)
      .subscribe(person => {
        if (person) {
          this.person = person;
        }
      }));

    this.subscriptions.push(this.route.parent.parent.params
      .subscribe(params => {
        this.isBusy = true;
        this.id = params['id'];
        this.store.loadPerson(this.id);
      }));
  }

  ngOnDestroy() {
    this.subscriptions.forEach(sub => sub.unsubscribe());
  }
}
