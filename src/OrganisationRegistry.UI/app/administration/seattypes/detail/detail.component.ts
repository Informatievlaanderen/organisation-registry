import { Component, ElementRef, OnInit } from '@angular/core';
import { ActivatedRoute, Params, Router } from '@angular/router';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';

import { AlertService } from 'core/alert';
import { CreateAlertMessages, UpdateAlertMessages } from 'core/alertmessages';
import { Create, ICrud, Update } from 'core/crud';
import { optionalNumber, required } from 'core/validation';

import { SeatType, SeatTypeService } from 'services/seattypes';

@Component({
  templateUrl: 'detail.template.html',
  styleUrls: [ 'detail.style.css' ]
})
export class SeatTypeDetailComponent implements OnInit {
  public isEditMode: boolean;
  public form: FormGroup;

  private crud: ICrud<SeatType>;
  private readonly createAlerts = new CreateAlertMessages('Post type');
  private readonly updateAlerts = new UpdateAlertMessages('Post type');

  get isFormValid() {
    return this.form.enabled && this.form.valid;
  }

  constructor(
    formBuilder: FormBuilder,
    private route: ActivatedRoute,
    private router: Router,
    private alertService: AlertService,
    private itemService: SeatTypeService
  ) {
    this.form = formBuilder.group({
      id: [ '', required ],
      name: [ '', required ],
      order: [ 1 ]
    });
  }

  ngOnInit() {
    this.route.params.forEach((params: Params) => {
      this.form.disable();

      let id = params[ 'id' ];
      this.isEditMode = id !== null && id !== undefined;

      this.crud = this.isEditMode
        ? new Update<SeatTypeService, SeatType>(id, this.itemService, this.alertService, this.updateAlerts)
        : new Create<SeatTypeService, SeatType>(this.itemService, this.alertService, this.createAlerts);

      this.crud
        .load(SeatType)
        .finally(() => this.form.enable())
        .subscribe(
          item => {
            if (item)
              this.form.setValue(item);
          },
          error => this.crud.alertLoadError(error));
    });
  }

  createOrUpdate(value: SeatType) {
    this.form.disable();

    this.crud.save(value)
      .finally(() => this.form.enable())
      .subscribe(
        result => {
          if (result) {
            let seatTypeUrl = this.router.serializeUrl(
              this.router.createUrlTree(
                [ './../', value.id ],
                { relativeTo: this.route }));

            this.router.navigate([ './..' ], { relativeTo: this.route });

            this.crud.alertSaveSuccess(value, seatTypeUrl);
          }
        },
        error => this.crud.alertSaveError(error));
  }
}
