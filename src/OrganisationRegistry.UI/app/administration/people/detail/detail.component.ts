import {Component, ElementRef, OnDestroy, OnInit} from '@angular/core';
import { ActivatedRoute, Params, Router } from '@angular/router';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';

import { AlertService } from 'core/alert';
import { CreateAlertMessages, UpdateAlertMessages } from 'core/alertmessages';
import { Create, ICrud, Update } from 'core/crud';
import { required, date } from 'core/validation';

import { Person, PersonService } from 'services/people';

import { RadioItem } from 'shared/components/form/form-group-radio/form-group-radio.model';
import {Subscription} from "rxjs/Subscription";

declare var moment: any; // global moment

@Component({
  templateUrl: 'detail.template.html',
  styleUrls: [ 'detail.style.css' ]
})
export class PersonDetailComponent implements OnInit, OnDestroy {
  public isEditMode: boolean;
  public form: FormGroup;

  public sexes: RadioItem[];
  public maxDate: string = moment().format('YYYY-MM-DD');

  private crud: ICrud<Person>;
  private readonly createAlerts = new CreateAlertMessages('Persoon');
  private readonly updateAlerts = new UpdateAlertMessages('Persoon');

  private readonly subscriptions: Subscription[] = new Array<Subscription>();

  get isFormValid() {
    return this.form.enabled && this.form.valid;
  }

  constructor(
    formBuilder: FormBuilder,
    private route: ActivatedRoute,
    private router: Router,
    private alertService: AlertService,
    private itemService: PersonService
  ) {
    this.sexes = [
      new RadioItem('Mannelijk', 'male'),
      new RadioItem('Vrouwelijk', 'female'),
    ];

    this.form = formBuilder.group({
      id: [ '', required ],
      firstName: [ '', required ],
      name: ['', required],
      dateOfBirth: ['', date],
      sex: [''],
      fullName: ['', Validators.nullValidator]
    });
  }

  ngOnInit() {
    this.route.params.forEach((params: Params) => {
      this.form.disable();

      let id = params[ 'id' ];
      this.isEditMode = id !== null && id !== undefined;

      this.crud = this.isEditMode
        ? new Update<PersonService, Person>(id, this.itemService, this.alertService, this.updateAlerts)
        : new Create<PersonService, Person>(this.itemService, this.alertService, this.createAlerts);

      this.subscriptions.push(this.crud
        .load(Person)
        .finally(() => this.form.enable())
        .subscribe(
          item => {
            if (item)
              this.form.setValue(item);
          },
          error => this.crud.alertLoadError(error)
        ));
    });
  }

  ngOnDestroy() {
    this.subscriptions.forEach(sub => sub.unsubscribe());
  }

  createOrUpdate(value: Person) {
    this.form.disable();

    this.subscriptions.push(this.crud.save(value)
      .finally(() => this.form.enable())
      .subscribe(
        result => {
          if (result) {
            let personUrl = this.router.serializeUrl(
              this.router.createUrlTree(
                [ './../', value.id ],
                { relativeTo: this.route }));

            this.router.navigate([ './..' ], { relativeTo: this.route });

            let dummyPerson = value;
            dummyPerson.name = `${value.name} ${value.firstName}`;
            this.crud.alertSaveSuccess(dummyPerson, personUrl);
          }
        },
        error => this.crud.alertSaveError(error)
      ));
  }
}
