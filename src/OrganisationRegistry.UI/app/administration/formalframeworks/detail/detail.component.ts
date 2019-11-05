import { Component, ElementRef, OnInit } from '@angular/core';
import { ActivatedRoute, Params, Router } from '@angular/router';
import { FormBuilder, FormGroup } from '@angular/forms';

import { AlertService } from 'core/alert';
import { CreateAlertMessages, UpdateAlertMessages } from 'core/alertmessages';
import { Create, ICrud, Update } from 'core/crud';
import { required } from 'core/validation';

import { SearchResult } from 'shared/components/form/form-group-autocomplete';

import { FormalFramework, FormalFrameworkService } from 'services/formalframeworks';
import { FormalFrameworkCategoryService } from 'services/formalframeworkcategories';

@Component({
  templateUrl: 'detail.template.html',
  styleUrls: [ 'detail.style.css' ]
})
export class FormalFrameworkDetailComponent implements OnInit {
  public isEditMode: boolean;
  public form: FormGroup;

  public initialResult: SearchResult;

  private crud: ICrud<FormalFramework>;
  private readonly createAlerts = new CreateAlertMessages('Toepassingsgebied');
  private readonly updateAlerts = new UpdateAlertMessages('Toepassingsgebied');

  get isFormValid() {
    return this.form.enabled && this.form.valid;
  }

  constructor(
    formBuilder: FormBuilder,
    private route: ActivatedRoute,
    private router: Router,
    private alertService: AlertService,
    private itemService: FormalFrameworkService,
    private formalFrameworkCategoryService: FormalFrameworkCategoryService
  ) {
    this.form = formBuilder.group({
      id: [ '', required ],
      name: [ '', required ],
      code: [ '', required ],
      formalFrameworkCategoryId: [ '', required ]
    });
  }

  ngOnInit() {
    this.route.params.forEach((params: Params) => {
      this.form.disable();

      let id = params[ 'id' ];
      this.isEditMode = id !== null && id !== undefined;

      this.crud = this.isEditMode
        ? new Update<FormalFrameworkService, FormalFramework>(id, this.itemService, this.alertService, this.updateAlerts)
        : new Create<FormalFrameworkService, FormalFramework>(this.itemService, this.alertService, this.createAlerts);

      this.crud
        .load(FormalFramework)
        .finally(() => this.form.enable())
        .subscribe(
          item => {
            if (item) {
              this.initialResult = new SearchResult(
                item.formalFrameworkCategoryId,
                item.formalFrameworkCategoryName);

              this.form.setValue({
                id: item.id,
                name: item.name,
                code: item.code,
                formalFrameworkCategoryId: item.formalFrameworkCategoryId
              });
            }
          },
          error => this.crud.alertLoadError(error)
        );
    });
  }

  createOrUpdate(value: FormalFramework) {
    this.form.disable();

    this.crud.save(value)
      .finally(() => this.form.enable())
      .subscribe(
        result => {
          if (result) {
            let formalFrameworkUrl = this.router.serializeUrl(
              this.router.createUrlTree(
                [ './../', value.id ],
                { relativeTo: this.route }));

            this.router.navigate([ './..' ], { relativeTo: this.route });

            this.crud.alertSaveSuccess(value, formalFrameworkUrl);
          }
        },
        error => this.crud.alertSaveError(error)
      );
  }
}
