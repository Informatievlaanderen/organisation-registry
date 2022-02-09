import {Component, OnDestroy, OnInit, Renderer} from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { ActivatedRoute, Router} from '@angular/router';

import { Observable } from 'rxjs/Observable';

import { AlertService, AlertBuilder} from 'core/alert';
import { required } from 'core/validation';

import { SearchResult } from 'shared/components/form/form-group-autocomplete';

import {
  UpdateOrganisationParentRequest,
  OrganisationParentService
} from 'services/organisationparents';
import {Subscription} from "rxjs/Subscription";

@Component({
  templateUrl: 'update.template.html',
  styleUrls: ['update.style.css']
})
export class OrganisationParentsUpdateOrganisationParentComponent implements OnInit, OnDestroy {
  public form: FormGroup;
  public parentOrganisation: SearchResult;

  private readonly subscriptions: Subscription[] = new Array<Subscription>();

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private formBuilder: FormBuilder,
    private organisationParentService: OrganisationParentService,
    private alertService: AlertService,
    private renderer: Renderer
  ) {
    this.form = formBuilder.group({
      organisationOrganisationParentId: ['', required],
      organisationId: ['', required],
      parentOrganisationId: ['', required],
      parentOrganisationName: ['', required],
      validFrom: [''],
      validTo: [''],
    });
  }

  ngOnInit() {
    this.subscriptions.push(Observable.zip(this.route.parent.parent.params, this.route.params)
      .subscribe(res => {
        this.form.disable();
        let orgId = res[0]['id'];
        let organisationParentId = res[1]['id'];

        this.subscriptions.push(this.organisationParentService
          .get(orgId, organisationParentId)
          .subscribe(
            item => this.setForm(item),
            error => this.handleError(error)));
      }));
  }

  ngOnDestroy() {
    this.subscriptions.forEach(sub => sub.unsubscribe());
  }

  get isFormValid() {
    return this.form.enabled && this.form.valid;
  }

  update(value: UpdateOrganisationParentRequest) {
    this.form.disable();

    this.subscriptions.push(this.organisationParentService.update(value.organisationId, value)
      .finally(() => this.form.enable())
      .subscribe(
        result => {
          if (result) {
            this.router.navigate(['./../..'], {relativeTo: this.route});
            this.handleSaveSuccess();
          }
        },
        error => this.handleSaveError(error)
      ));
  }

  private setForm(organisationParent) {
    if (organisationParent) {
      this.form.setValue(organisationParent);
      this.parentOrganisation = new SearchResult(organisationParent.parentOrganisationId, organisationParent.parentOrganisationName);
    }

    this.form.enable();
  }

  private handleError(error) {
    this.alertService.setAlert(
      new AlertBuilder()
        .error(error)
        .withTitle('Moeder entiteit kon niet geladen worden!')
        .withMessage('Er is een fout opgetreden bij het ophalen van de moeder entiteit. Probeer het later opnieuw.')
        .build()
    );
  }

  private handleSaveSuccess() {
    this.alertService.setAlert(
      new AlertBuilder()
        .success()
        .withTitle('Moeder entiteit bijgewerkt!')
        .withMessage('Moeder entiteit is succesvol bijgewerkt.')
        .build());
  }

  private handleSaveError(error) {
    this.alertService.setAlert(
      new AlertBuilder()
        .error(error)
        .withTitle('Moeder entiteit kon niet bewaard worden!')
        .withMessage('Er is een fout opgetreden bij het bewaren van de gegevens. Probeer het later opnieuw.')
        .build());
  }
}
