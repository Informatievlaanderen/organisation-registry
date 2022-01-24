import {Component, ElementRef, OnDestroy, OnInit} from '@angular/core';
import { ActivatedRoute, Params, Router } from '@angular/router';
import { FormBuilder, FormGroup } from '@angular/forms';

import { AlertService } from 'core/alert';
import { CreateAlertMessages, UpdateAlertMessages } from 'core/alertmessages';
import { Create, ICrud, Update } from 'core/crud';
import { required } from 'core/validation';

import { Function, FunctionService } from 'services/functions';
import {Subscription} from "rxjs/Subscription";

@Component({
  templateUrl: 'detail.template.html',
  styleUrls: ['detail.style.css']
})
export class FunctionDetailComponent implements OnInit, OnDestroy {
  public isEditMode: boolean;
  public form: FormGroup;

  private crud: ICrud<Function>;
  private readonly createAlerts = new CreateAlertMessages('Functie type');
  private readonly updateAlerts = new UpdateAlertMessages('Functie type');

  private readonly subscriptions: Subscription[] = new Array<Subscription>();

  get isFormValid() {
    return this.form.enabled && this.form.valid;
  }

  constructor(
    formBuilder: FormBuilder,
    private route: ActivatedRoute,
    private router: Router,
    private alertService: AlertService,
    private itemService: FunctionService
  ) {
    this.form = formBuilder.group({
      id: ['', required],
      name: ['', required]
    });
  }

  ngOnInit() {
    this.route.params.forEach((params: Params) => {
      this.form.disable();

      let id = params['id'];
      this.isEditMode = id !== null && id !== undefined;

      this.crud = this.isEditMode
        ? new Update<FunctionService, Function>(id, this.itemService, this.alertService, this.updateAlerts)
        : new Create<FunctionService, Function>(this.itemService, this.alertService, this.createAlerts);

      this.subscriptions.push(this.crud
        .load(Function)
        .finally(() => this.form.enable())
        .subscribe(
          item => {
            if (item)
              this.form.setValue(item);
          },
          error => this.crud.alertLoadError(error)));
    });
  }

  ngOnDestroy() {
    this.subscriptions.forEach(sub => sub.unsubscribe());
  }

  createOrUpdate(value: Function) {
    this.form.disable();

    this.subscriptions.push(this.crud.save(value)
      .finally(() => this.form.enable())
      .subscribe(
        result => {
          if (result) {
            let functionUrl = this.router.serializeUrl(
              this.router.createUrlTree(
                ['./../', value.id],
                { relativeTo: this.route }));

            this.router.navigate(['./..'], { relativeTo: this.route });

            this.crud.alertSaveSuccess(value, functionUrl);
          }
        },
        error => this.crud.alertSaveError(error)));
  }
}
