import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, FormControl, Validator, Validators } from '@angular/forms';
import { ActivatedRoute, Router, Params } from '@angular/router';

import { AlertService, AlertBuilder, Alert, AlertType } from 'core/alert';
import { CreateAlertMessages } from 'core/alertmessages';
import { Create, ICrud } from 'core/crud';
import { required } from 'core/validation';

import { SelectItem } from 'shared/components/form/form-group-select';

import { Person, PersonService } from 'services/people';
import { OrganisationRelationType, OrganisationRelationTypeService } from 'services/organisationrelationtypes';
import { ContactTypeListItem } from 'services/contacttypes';

import {
  CreateOrganisationRelationRequest,
  OrganisationRelationService
} from 'services/organisationrelations';

@Component({
  templateUrl: 'create.template.html',
  styleUrls: ['create.style.css']
})
export class OrganisationRelationsCreateOrganisationRelationComponent implements OnInit {
  public form: FormGroup;
  public relations: SelectItem[];

  private readonly createAlerts = new CreateAlertMessages('Relatie');

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private formBuilder: FormBuilder,
    private relationService: OrganisationRelationTypeService,
    private organisationRelationService: OrganisationRelationService,
    private alertService: AlertService
  ) {
    this.form = formBuilder.group({
      organisationRelationId: ['', required],
      organisationId: ['', required],
      relationId: ['', required],
      relatedOrganisationId: ['', required],
      validFrom: [''],
      validTo: [''],
    });
  }

  ngOnInit() {
    this.route.parent.parent.params.forEach((params: Params) => {
      this.form.disable();
      let initialValue = new CreateOrganisationRelationRequest(params['id']);
      this.form.setValue(initialValue);

      this.relationService
        .getAllOrganisationRelationTypes()
        .finally(() => this.form.enable())
        .subscribe(
          allRelationTypes => this.relations = allRelationTypes.map(k => new SelectItem(k.id, k.name)),
          error =>
            this.alertService.setAlert(
              new AlertBuilder()
                .error(error)
                .withTitle('Relaties konden niet geladen worden!')
                .withMessage('Er is een fout opgetreden bij het ophalen van de functies. Probeer het later opnieuw.')
                .build()));
    });
  }

  get isFormValid() {
    return this.form.enabled && this.form.valid;
  }

  create(value: CreateOrganisationRelationRequest) {
    this.form.disable();

    this.organisationRelationService.create(value.organisationId, value)
      .finally(() => this.form.enable())
      .subscribe(
        result => {
          if (result) {
            this.router.navigate(['./..'], { relativeTo: this.route });

            this.alertService.setAlert(
              new AlertBuilder()
                .success()
                .withTitle('Relatie aangemaakt!')
                .withMessage('Relatie is succesvol aangemaakt.')
                .build());
          }
        },
        error =>
          this.alertService.setAlert(
            new AlertBuilder()
              .error(error)
              .withTitle('Relatie kon niet bewaard worden!')
              .withMessage('Er is een fout opgetreden bij het bewaren van de gegevens. Probeer het later opnieuw.')
              .build()));
  }
}
