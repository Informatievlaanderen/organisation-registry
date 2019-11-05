import { Component, ElementRef, OnInit } from '@angular/core';
import { ActivatedRoute, Params, Router } from '@angular/router';
import { FormBuilder, FormGroup } from '@angular/forms';

import { AlertService } from 'core/alert';
import { CreateAlertMessages, UpdateAlertMessages } from 'core/alertmessages';
import { Create, ICrud, Update } from 'core/crud';
import { required } from 'core/validation';

import { ConfigurationValue, ConfigurationValueService } from 'services/configurationvalues';

@Component({
  templateUrl: 'detail.template.html',
  styleUrls: [ 'detail.style.css' ]
})
export class ConfigurationValueDetailComponent implements OnInit {
  public isEditMode: boolean;
  public form: FormGroup;

  private crud: ICrud<ConfigurationValue>;
  private readonly createAlerts = new CreateAlertMessages('Configuratiewaarde');
  private readonly updateAlerts = new UpdateAlertMessages('Configuratiewaarde');

  get isFormValid() {
    return this.form.enabled && this.form.valid;
  }

  constructor(
    formBuilder: FormBuilder,
    private route: ActivatedRoute,
    private router: Router,
    private alertService: AlertService,
    private itemService: ConfigurationValueService
  ) {
    this.form = formBuilder.group({
      id: [''],
      name: [''],
      key: ['', required],
      description: ['', required],
      value: ['', required]
    });
  }

  ngOnInit() {
    this.route.params.forEach((params: Params) => {
      let id = params[ 'id' ];
      this.isEditMode = id !== null && id !== undefined;

      this.crud = this.isEditMode
        ? new Update<ConfigurationValueService, ConfigurationValue>(id, this.itemService, this.alertService, this.updateAlerts)
        : new Create<ConfigurationValueService, ConfigurationValue>(this.itemService, this.alertService, this.createAlerts);

      this.form.disable();
      this.crud
        .load(ConfigurationValue)
        .finally(() => this.form.enable())
        .subscribe(
          item => {
            if (item) {
              item.id = item.key;
              item.name = item.key;
              this.form.setValue(item);
            }
          },
          error => this.crud.alertLoadError(error)
        );
    });
  }

  createOrUpdate(value: ConfigurationValue) {
    this.form.disable();

    value.id = value.key;
    value.name = value.key;

    this.crud.save(value)
      .finally(() => this.form.enable())
      .subscribe(
        result => {
          if (result) {
            let configurationValueUrl = this.router.serializeUrl(
              this.router.createUrlTree(
                [ './../', value.id ],
                { relativeTo: this.route }));

            this.router.navigate([ './..' ], { relativeTo: this.route });

            this.crud.alertSaveSuccess(value, configurationValueUrl);
          }
        },
        error => this.crud.alertSaveError(error)
      );
  }
}
