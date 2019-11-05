import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, FormControl, Validator, Validators } from '@angular/forms';
import { ActivatedRoute, Router, Params } from '@angular/router';

import { AlertService, AlertBuilder, Alert, AlertType } from 'core/alert';
import { CreateAlertMessages } from 'core/alertmessages';
import { Create, ICrud } from 'core/crud';
import { required } from 'core/validation';

import { SelectItem } from 'shared/components/form/form-group-select';

import { Person, PersonService } from 'services/people';
import { Function, FunctionService } from 'services/functions';
import { ContactTypeListItem } from 'services/contacttypes';

import {
  CreateOrganisationFunctionRequest,
  OrganisationFunctionService
} from 'services/organisationfunctions';

@Component({
  templateUrl: 'create.template.html',
  styleUrls: ['create.style.css']
})
export class OrganisationFunctionsCreateOrganisationFunctionComponent implements OnInit {
  public form: FormGroup;
  public functions: SelectItem[];
  public contactTypes: ContactTypeListItem[];

  private readonly createAlerts = new CreateAlertMessages('Functie');

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private formBuilder: FormBuilder,
    private functionService: FunctionService,
    private personService: PersonService,
    private organisationFunctionService: OrganisationFunctionService,
    private alertService: AlertService
  ) {
    this.form = formBuilder.group({
      organisationFunctionId: ['', required],
      organisationId: ['', required],
      functionId: ['', required],
      personId: ['', required],
      validFrom: [''],
      validTo: [''],
    });
  }

  ngOnInit() {
    this.contactTypes = this.route.snapshot.data['contactTypes'];
    this.form.addControl('contacts', this.toFormGroup(this.contactTypes));

    this.route.parent.parent.params.forEach((params: Params) => {
      this.form.disable();
      let initialValue = new CreateOrganisationFunctionRequest(params['id']);

      this.contactTypes.forEach(contactType => {
        initialValue.contacts[contactType.id] = '';
      });

      this.form.setValue(initialValue);

      this.functionService
        .getAllFunctions()
        .finally(() => this.form.enable())
        .subscribe(
          allFunctionTypes => this.functions = allFunctionTypes.map(k => new SelectItem(k.id, k.name)),
          error =>
            this.alertService.setAlert(
              new AlertBuilder()
                .error(error)
                .withTitle('Functies konden niet geladen worden!')
                .withMessage('Er is een fout opgetreden bij het ophalen van de functies. Probeer het later opnieuw.')
                .build()));
    });
  }

  toFormGroup(contactTypes: ContactTypeListItem[]) {
    let group: any = {};

    contactTypes.forEach(contactType => {
      group[contactType.id] = new FormControl('');
    });

    return new FormGroup(group);
  }

  get isFormValid() {
    return this.form.enabled && this.form.valid;
  }

  create(value: CreateOrganisationFunctionRequest) {
    this.form.disable();

    this.organisationFunctionService.create(value.organisationId, value)
      .finally(() => this.form.enable())
      .subscribe(
        result => {
          if (result) {
            this.router.navigate(['./..'], { relativeTo: this.route });

            this.alertService.setAlert(
              new AlertBuilder()
                .success()
                .withTitle('Functie aangemaakt!')
                .withMessage('Functie is succesvol aangemaakt.')
                .build());
          }
        },
        error =>
          this.alertService.setAlert(
            new AlertBuilder()
              .error(error)
              .withTitle('Functie kon niet bewaard worden!')
              .withMessage('Er is een fout opgetreden bij het bewaren van de gegevens. Probeer het later opnieuw.')
              .build()));
  }
}
