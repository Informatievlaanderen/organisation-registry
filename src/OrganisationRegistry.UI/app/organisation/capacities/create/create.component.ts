import {Component, OnDestroy, OnInit} from '@angular/core';
import { FormBuilder, FormGroup, FormControl, Validators } from '@angular/forms';
import { ActivatedRoute, Router, Params } from '@angular/router';

import { AlertService, AlertBuilder} from 'core/alert';
import { required } from 'core/validation';

import { SelectItem } from 'shared/components/form/form-group-select';

import { PersonFunctionService } from 'services/peoplefunctions';
import { PersonService } from 'services/people';
import { CapacityService } from 'services/capacities';
import { ContactTypeListItem } from 'services/contacttypes';

import {
  CreateOrganisationCapacityRequest,
  OrganisationCapacityService
} from 'services/organisationcapacities';
import {Subscription} from "rxjs/Subscription";

@Component({
  templateUrl: 'create.template.html',
  styleUrls: ['create.style.css']
})
export class OrganisationCapacitiesCreateOrganisationCapacityComponent implements OnInit, OnDestroy {
  public form: FormGroup;
  public capacities: SelectItem[];
  public contactTypes: ContactTypeListItem[];

  public functions: Array<SelectItem> = [];

  private personId: string = '';

  private readonly subscriptions: Subscription[] = new Array<Subscription>();

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private formBuilder: FormBuilder,
    private capacityService: CapacityService,
    private personService: PersonService,
    private personFunctionService: PersonFunctionService,
    private organisationCapacityService: OrganisationCapacityService,
    private alertService: AlertService
  ) {
    this.form = formBuilder.group({
      organisationCapacityId: ['', required],
      organisationId: ['', required],
      capacityId: ['', required],
      personId: ['', Validators.nullValidator],
      functionId: ['', Validators.nullValidator],
      locationId: ['', Validators.nullValidator],
      validFrom: [''],
      validTo: [''],
    });
  }

  ngOnInit() {
    this.contactTypes = this.route.snapshot.data['contactTypes'];
    this.form.addControl('contacts', this.toFormGroup(this.contactTypes));

    this.route.parent.parent.params.forEach((params: Params) => {
      this.subscribeToFormChanges();
      this.form.disable();

      let initialValue = new CreateOrganisationCapacityRequest(params['id']);

      this.contactTypes.forEach(contactType => {
        initialValue.contacts[contactType.id] = '';
      });

      this.form.setValue(initialValue);
    });

    this.subscriptions.push(this.capacityService
      .getAllCapacities()
      .finally(() => this.enableForm())
      .subscribe(
        allCapacityTypes => this.capacities = allCapacityTypes.map(k => new SelectItem(k.id, k.name)),
        error =>
          this.alertService.setAlert(
            new AlertBuilder()
              .error(error)
              .withTitle('Hoedanigheden konden niet geladen worden!')
              .withMessage('Er is een fout opgetreden bij het ophalen van de hoedanigheden. Probeer het later opnieuw.')
              .build())));
  }

  ngOnDestroy() {
    this.subscriptions.forEach(sub => sub.unsubscribe());
  }

  toFormGroup(contactTypes: ContactTypeListItem[]) {
    let group: any = {};

    contactTypes.forEach(contactType => {
      group[contactType.id] = new FormControl('');
    });

    return new FormGroup(group);
  }

  subscribeToFormChanges() {
    const personIdChanges$ = this.form.controls['personId'].valueChanges;

    this.subscriptions.push(personIdChanges$
      .subscribe(function (personId) {
        if (this.personId === personId)
          return;

        this.personId = personId;
        this.form.patchValue({ functionId: '' });

        this.form.disable();

        if (personId) {
          this.subscriptions.push(this.personFunctionService
            .getAllPersonFunctions(personId)
            .finally(() => this.enableForm())
            .subscribe(
              allFunctions => {
                this.functions = allFunctions
                  .map(f => {
                    let from = f.validFrom || '∞';
                    let to = f.validTo || '∞';
                    return new SelectItem(f.functionId, `${f.functionName} (${from} -> ${to})`);
                  });
              },
              error =>
                this.alertService.setAlert(
                  new AlertBuilder()
                    .error(error)
                    .withTitle('Functies konden niet geladen worden!')
                    .withMessage('Er is een fout opgetreden bij het ophalen van de functies. Probeer het later opnieuw.')
                    .build())));
        } else {
          this.enableForm();
        }
      }.bind(this)));
  }

  enableForm() {
    this.form.enable();
    if (!this.personId)
      this.form.get('functionId').disable();
  }

  get isFormValid() {
    return this.form.enabled && this.form.valid;
  }

  create(value: CreateOrganisationCapacityRequest) {
    this.form.disable();

    this.subscriptions.push(this.organisationCapacityService.create(value.organisationId, value)
      .finally(() => this.enableForm())
      .subscribe(
        result => {
          if (result) {
            this.router.navigate(['./..'], { relativeTo: this.route });

            this.alertService.setAlert(
              new AlertBuilder()
                .success()
                .withTitle('Hoedanigheid aangemaakt!')
                .withMessage('Hoedanigheid is succesvol aangemaakt.')
                .build());
          }
        },
        error =>
          this.alertService.setAlert(
            new AlertBuilder()
              .error(error)
              .withTitle('Hoedanigheid kon niet bewaard worden!')
              .withMessage('Er is een fout opgetreden bij het bewaren van de gegevens. Probeer het later opnieuw.')
              .build())));
  }
}
