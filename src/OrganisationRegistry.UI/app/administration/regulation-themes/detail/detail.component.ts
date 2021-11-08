import { Component, ElementRef, OnInit } from '@angular/core';
import { ActivatedRoute, Params, Router } from '@angular/router';
import { FormBuilder, FormGroup } from '@angular/forms';

import { AlertService } from 'core/alert';
import { CreateAlertMessages, UpdateAlertMessages } from 'core/alertmessages';
import { Create, ICrud, Update } from 'core/crud';
import { required } from 'core/validation';

import { RegulationTheme, RegulationThemeService } from 'services/regulation-themes';

@Component({
  templateUrl: 'detail.template.html',
  styleUrls: [ 'detail.style.css' ]
})
export class RegulationThemeDetailComponent implements OnInit {
  public isEditMode: boolean;
  public form: FormGroup;

  private crud: ICrud<RegulationTheme>;
  private readonly createAlerts = new CreateAlertMessages('Regelgevingthema');
  private readonly updateAlerts = new UpdateAlertMessages('Regelgevingthema');

  get isFormValid() {
    return this.form.enabled && this.form.valid;
  }

  constructor(
    formBuilder: FormBuilder,
    private route: ActivatedRoute,
    private router: Router,
    private alertService: AlertService,
    private itemService: RegulationThemeService
  ) {
    this.form = formBuilder.group({
      id: [ '', required ],
      name: [ '', required ]
    });
  }

  ngOnInit() {
    this.route.params.forEach((params: Params) => {
      this.form.disable();

      let id = params[ 'id' ];
      this.isEditMode = id !== null && id !== undefined;

      this.crud = this.isEditMode
        ? new Update<RegulationThemeService, RegulationTheme>(id, this.itemService, this.alertService, this.updateAlerts)
        : new Create<RegulationThemeService, RegulationTheme>(this.itemService, this.alertService, this.createAlerts);

      this.crud
        .load(RegulationTheme)
        .finally(() => this.form.enable())
        .subscribe(
          item => {
            if (item)
              this.form.setValue(item);
          },
          error => this.crud.alertLoadError(error));
    });
  }

  createOrUpdate(value: RegulationTheme) {
    this.form.disable();

    this.crud.save(value)
      .finally(() => this.form.enable())
      .subscribe(
        result => {
          if (result) {
            let regulationThemeUrl = this.router.serializeUrl(
              this.router.createUrlTree(
                [ './../', value.id ],
                { relativeTo: this.route }));

            this.router.navigate([ './..' ], { relativeTo: this.route });

            this.crud.alertSaveSuccess(value, regulationThemeUrl);
          }
        },
        error => this.crud.alertSaveError(error));
  }
}
