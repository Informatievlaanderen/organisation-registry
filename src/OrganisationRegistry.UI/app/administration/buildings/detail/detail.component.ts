import { Component, ElementRef, OnInit } from '@angular/core';
import { ActivatedRoute, Params, Router } from '@angular/router';
import { FormBuilder, FormGroup } from '@angular/forms';

import { AlertService } from 'core/alert';
import { CreateAlertMessages, UpdateAlertMessages } from 'core/alertmessages';
import { Create, ICrud, Update } from 'core/crud';
import { optionalNumber, required } from 'core/validation';

import { Building, BuildingService } from 'services/buildings';

@Component({
  templateUrl: 'detail.template.html',
  styleUrls: ['detail.style.css']
})
export class BuildingDetailComponent implements OnInit {
  public isEditMode: boolean;
  public form: FormGroup;

  private crud: ICrud<Building>;
  private readonly createAlerts = new CreateAlertMessages('Gebouw');
  private readonly updateAlerts = new UpdateAlertMessages('Gebouw');

  get isFormValid() {
    return this.form.enabled && this.form.valid;
  }

  constructor(
    formBuilder: FormBuilder,
    private route: ActivatedRoute,
    private router: Router,
    private alertService: AlertService,
    private itemService: BuildingService
  ) {
    this.form = formBuilder.group({
      id: ['', required],
      name: ['', required],
      vimId: ['', optionalNumber]
    });
  }

  ngOnInit() {
    this.route.params.forEach((params: Params) => {
      this.form.disable();

      let id = params['id'];
      this.isEditMode = id !== null && id !== undefined;

      this.crud = this.isEditMode
        ? new Update<BuildingService, Building>(id, this.itemService, this.alertService, this.updateAlerts)
        : new Create<BuildingService, Building>(this.itemService, this.alertService, this.createAlerts);

      this.crud
        .load(Building)
        .finally(() => this.form.enable())
        .subscribe(
          item => {
            if (item)
              this.form.setValue(item);
          },
          error => this.crud.alertLoadError(error));
    });
  }

  createOrUpdate(value: Building) {
    this.form.disable();

    this.crud.save(value)
      .finally(() => this.form.enable())
      .subscribe(
        result => {
          if (result) {
            let buildingUrl = this.router.serializeUrl(
              this.router.createUrlTree(
                ['./../', value.id],
                { relativeTo: this.route }));

            this.router.navigate(['./..'], { relativeTo: this.route });

            this.crud.alertSaveSuccess(value, buildingUrl);
          }
        },
        error => this.crud.alertSaveError(error));
  }
}
