import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Params, Router } from '@angular/router';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';

import { AlertService } from 'core/alert';
import { CreateAlertMessages, UpdateAlertMessages } from 'core/alertmessages';
import { Create, ICrud, Update } from 'core/crud';
import { optionalNumber, required } from 'core/validation';

import { RegulationSubTheme, RegulationSubThemeService } from 'services/regulation-sub-themes';

import { SearchResult } from 'shared/components/form/form-group-autocomplete';

@Component({
  templateUrl: 'detail.template.html',
  styleUrls: [ 'detail.style.css' ]
})
export class RegulationSubThemeDetailComponent implements OnInit {
  public isEditMode: boolean;
  public form: FormGroup;
  public initialResult: SearchResult;

  private crud: ICrud<RegulationSubTheme>;
  private readonly createAlerts = new CreateAlertMessages('Regelgevingsubthema');
  private readonly updateAlerts = new UpdateAlertMessages('Regelgevingsubthema');

  get isFormValid() {
    return this.form.enabled && this.form.valid;
  }

  constructor(
    formBuilder: FormBuilder,
    private route: ActivatedRoute,
    private router: Router,
    private alertService: AlertService,
    private itemService: RegulationSubThemeService,
  ) {
    this.form = formBuilder.group({
      id: [ '', required ],
      name: [ '', required ],
      regulationThemeId: [ '', required ]
    });
  }

  ngOnInit() {
    this.form.disable();
    this.route.params.forEach((params: Params) => {
      this.form.disable();

      let id = params[ 'id' ];
      this.isEditMode = id !== null && id !== undefined;

      this.crud = this.isEditMode
        ? new Update<RegulationSubThemeService, RegulationSubTheme>(id, this.itemService, this.alertService, this.updateAlerts)
        : new Create<RegulationSubThemeService, RegulationSubTheme>(this.itemService, this.alertService, this.createAlerts);

      this.crud
        .load(RegulationSubTheme)
        .finally(() => this.form.enable())
        .subscribe(
          item => {
            if (item)
              this.initialResult = new SearchResult(
                item.regulationThemeId,
                item.regulationThemeName);

              this.form.setValue({
                id: item.id,
                name: item.name,
                regulationThemeId: item.regulationThemeId
              });
          },
          error => this.crud.alertLoadError(error)
        );
    });
  }

  createOrUpdate(value: RegulationSubTheme) {
    this.form.disable();

    this.crud.save(value)
      .finally(() => this.form.enable())
      .subscribe(
        result => {
          if (result) {
            let regulationSubThemeUrl = this.router.serializeUrl(
              this.router.createUrlTree(
                [ './../', value.id ],
                { relativeTo: this.route }));

            this.router.navigate(['./..'], { relativeTo: this.route });

            this.crud.alertSaveSuccess(value, regulationSubThemeUrl);
          }
        },
        error => this.crud.alertSaveError(error)
      );
  }
}
