import {Component, OnDestroy, OnInit, Renderer} from '@angular/core';
import { FormBuilder, FormGroup} from '@angular/forms';
import { ActivatedRoute, Router} from '@angular/router';

import { Observable } from 'rxjs/Observable';

import { AlertService, AlertBuilder} from 'core/alert';
import { required } from 'core/validation';

import { SelectItem } from 'shared/components/form/form-group-select';
import { SearchResult } from 'shared/components/form/form-group-autocomplete';

import { OrganisationRelationTypeService } from 'services/organisationrelationtypes';

import {
  UpdateOrganisationRelationRequest,
  OrganisationRelationService
} from 'services/organisationrelations';
import {Subscription} from "rxjs/Subscription";

@Component({
  templateUrl: 'update.template.html',
  styleUrls: ['update.style.css']
})
export class OrganisationRelationsUpdateOrganisationRelationComponent implements OnInit, OnDestroy {
  public form: FormGroup;
  public relations: SelectItem[];
  public organisation: SearchResult;
  private relationId: string;

  private readonly subscriptions: Subscription[] = new Array<Subscription>();

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private formBuilder: FormBuilder,
    private relationService: OrganisationRelationTypeService,
    private organisationRelationService: OrganisationRelationService,
    private alertService: AlertService,
    private renderer: Renderer
  ) {
    this.form = formBuilder.group({
      organisationRelationId: ['', required],
      organisationId: ['', required],
      relationId: ['', required],
      relationName: ['', required],
      relatedOrganisationId: ['', required],
      relatedOrganisationName: ['', required],
      validFrom: [''],
      validTo: [''],
    });
  }

  ngOnInit() {
    let allRelationsObservable = this.relationService.getAllOrganisationRelationTypes();

    this.subscriptions.push(Observable.zip(this.route.parent.parent.params, this.route.params)
      .subscribe(res => {
        this.form.disable();
        let orgId = res[0]['id'];
        let relationId = res[1]['id'];

        this.subscriptions.push(Observable.zip(this.organisationRelationService.get(orgId, relationId), allRelationsObservable)
          .subscribe(
            item => this.setForm(item[0], item[1]),
            error => this.handleError(error)));
      }));
  }

  ngOnDestroy() {
    this.subscriptions.forEach(sub => sub.unsubscribe());
  }

  get isFormValid() {
    return this.form.enabled && this.form.valid;
  }

  update(value: UpdateOrganisationRelationRequest) {
    this.form.disable();

    this.subscriptions.push(this.organisationRelationService.update(value.organisationId, value)
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

  private setForm(organisationRelation, allRelations) {
    if (organisationRelation) {
      this.form.setValue(organisationRelation);
      this.organisation = new SearchResult(organisationRelation.relatedOrganisationId, organisationRelation.relatedOrganisationName);
    }

    this.relations = allRelations.map(k => new SelectItem(k.id, k.name));
    this.form.enable();
  }

  private handleError(error) {
    this.alertService.setAlert(
      new AlertBuilder()
        .error(error)
        .withTitle('Relatie kon niet geladen worden!')
        .withMessage('Er is een fout opgetreden bij het ophalen van de relatie. Probeer het later opnieuw.')
        .build()
    );
  }

  private handleSaveSuccess() {
    this.alertService.setAlert(
      new AlertBuilder()
        .success()
        .withTitle('Relatie bijgewerkt!')
        .withMessage('Relatie is succesvol bijgewerkt.')
        .build());
  }

  private handleSaveError(error) {
    this.alertService.setAlert(
      new AlertBuilder()
        .error(error)
        .withTitle('Relatie kon niet bewaard worden!')
        .withMessage('Er is een fout opgetreden bij het bewaren van de gegevens. Probeer het later opnieuw.')
        .build());
  }
}
