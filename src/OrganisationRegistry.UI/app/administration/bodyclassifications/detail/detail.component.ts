import {Component, ElementRef, OnDestroy, OnInit} from '@angular/core';
import { ActivatedRoute, Params, Router } from '@angular/router';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';

import { AlertService } from 'core/alert';
import { CreateAlertMessages, UpdateAlertMessages } from 'core/alertmessages';
import { Create, ICrud, Update } from 'core/crud';
import { optionalNumber, required } from 'core/validation';

import { BodyClassification, BodyClassificationService } from 'services/bodyclassifications';
import { BodyClassificationTypeService } from 'services/bodyclassificationtypes';

import { SearchResult } from 'shared/components/form/form-group-autocomplete';
import {Subscription} from "rxjs/Subscription";

@Component({
  templateUrl: 'detail.template.html',
  styleUrls: [ 'detail.style.css' ]
})
export class BodyClassificationDetailComponent implements OnInit, OnDestroy {
  public isEditMode: boolean;
  public form: FormGroup;
  public initialResult: SearchResult;

  private crud: ICrud<BodyClassification>;
  private readonly createAlerts = new CreateAlertMessages('Orgaanclassificatie');
  private readonly updateAlerts = new UpdateAlertMessages('Orgaanclassificatie');

  private readonly subscriptions: Subscription[] = new Array<Subscription>();

  get isFormValid() {
    return this.form.enabled && this.form.valid;
  }

  constructor(
    formBuilder: FormBuilder,
    private route: ActivatedRoute,
    private router: Router,
    private alertService: AlertService,
    private itemService: BodyClassificationService,
    private bodyClassificationCategoryService: BodyClassificationTypeService
  ) {
    this.form = formBuilder.group({
      id: [ '', required ],
      name: [ '', required ],
      order: [ 1, Validators.compose([required, optionalNumber]) ],
      active: [ true, Validators.required ],
      bodyClassificationTypeId: [ '', required ]
    });
  }

  ngOnInit() {
    this.form.disable();
    this.route.params.forEach((params: Params) => {
      this.form.disable();

      let id = params[ 'id' ];
      this.isEditMode = id !== null && id !== undefined;

      this.crud = this.isEditMode
        ? new Update<BodyClassificationService, BodyClassification>(id, this.itemService, this.alertService, this.updateAlerts)
        : new Create<BodyClassificationService, BodyClassification>(this.itemService, this.alertService, this.createAlerts);

      this.subscriptions.push(this.crud
        .load(BodyClassification)
        .finally(() => this.form.enable())
        .subscribe(
          item => {
            if (item)
              this.initialResult = new SearchResult(
                item.bodyClassificationTypeId,
                item.bodyClassificationTypeName);

              this.form.setValue({
                id: item.id,
                name: item.name,
                order: item.order,
                active: item.active,
                bodyClassificationTypeId: item.bodyClassificationTypeId
              });
          },
          error => this.crud.alertLoadError(error)
        ));
    });
  }

  ngOnDestroy() {
    this.subscriptions.forEach(sub => sub.unsubscribe());
  }

  createOrUpdate(value: BodyClassification) {
    this.form.disable();

    this.subscriptions.push(this.crud.save(value)
      .finally(() => this.form.enable())
      .subscribe(
        result => {
          if (result) {
            let bodyClassificationUrl = this.router.serializeUrl(
              this.router.createUrlTree(
                [ './../', value.id ],
                { relativeTo: this.route }));

            this.router.navigate(['./..'], { relativeTo: this.route });

            this.crud.alertSaveSuccess(value, bodyClassificationUrl);
          }
        },
        error => this.crud.alertSaveError(error)
      ));
  }
}
