import { Component, OnInit, ViewChild, ElementRef, Renderer } from '@angular/core';
import { FormBuilder, FormGroup, FormControl, Validators } from '@angular/forms';
import { ActivatedRoute, Router, Params } from '@angular/router';

import { Observable } from 'rxjs/Observable';

import { AlertService, AlertBuilder, Alert, AlertType } from 'core/alert';
import { UpdateAlertMessages } from 'core/alertmessages';
import { Update, ICrud } from 'core/crud';
import { required } from 'core/validation';

import { SelectItem } from 'shared/components/form/form-group-select';
import { SearchResult } from 'shared/components/form/form-group-autocomplete';

import { PersonFunctionListItem, PersonFunctionService } from 'services/peoplefunctions';
import { Person, PersonService } from 'services/people';
import { Capacity, CapacityService } from 'services/capacities';
import { ContactTypeListItem } from 'services/contacttypes';

import {
  UpdateOrganisationCapacityRequest,
  OrganisationCapacityService
} from 'services/organisationcapacities';

@Component({
  templateUrl: 'update.template.html',
  styleUrls: ['update.style.css']
})
export class OrganisationCapacitiesUpdateOrganisationCapacityComponent implements OnInit {
  public form: FormGroup;
  public capacities: SelectItem[];
  public person: SearchResult;
  public location: SearchResult;
  public contactTypes: ContactTypeListItem[];

  public functions: Array<SelectItem> = [];

  private personId: string = '';
  private readonly updateAlerts = new UpdateAlertMessages('Hoedanigheid');
  private capacityId: string;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private formBuilder: FormBuilder,
    private capacityService: CapacityService,
    private personService: PersonService,
    private personFunctionService: PersonFunctionService,
    private organisationCapacityService: OrganisationCapacityService,
    private alertService: AlertService,
    private renderer: Renderer
  ) {
    this.form = formBuilder.group({
      organisationCapacityId: ['', required],
      organisationId: ['', required],
      capacityId: ['', required],
      capacityName: ['', required],
      personId: ['', Validators.nullValidator],
      personName: ['', Validators.nullValidator],
      functionId: ['', Validators.nullValidator],
      functionName: ['', Validators.nullValidator],
      locationId: ['', Validators.nullValidator],
      locationName: ['', Validators.nullValidator],
      validFrom: [''],
      validTo: [''],
    });
  }

  ngOnInit() {
    this.contactTypes = this.route.snapshot.data['contactTypes'];
    this.form.addControl('contacts', this.toFormGroup(this.contactTypes));

    let allCapacitiesObservable = this.capacityService.getAllCapacities();
    this.subscribeToFormChanges();

    Observable.zip(this.route.parent.parent.params, this.route.params)
      .subscribe(res => {
        this.form.disable();
        let orgId = res[0]['id'];
        let capacityId = res[1]['id'];

        Observable.zip(this.organisationCapacityService.get(orgId, capacityId), allCapacitiesObservable)
          .subscribe(
            item => this.setForm(item[0], item[1]),
            error => this.handleError(error));
      });
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

    personIdChanges$
      .subscribe(function (personId) {
        if (this.personId === personId)
          return;

        this.personId = personId;

        this.form.patchValue({ functionId: '' });

        if (personId) {
          this.isFunctionsBusy = true;

          this.personFunctionService
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
                    .build()));
        } else {
          this.enableForm();
        }
      }.bind(this));
  }

  enableForm() {
    this.form.enable();
    if (!this.personId)
      this.form.get('functionId').disable();
  }

  get isFormValid() {
    return this.form.enabled && this.form.valid;
  }

  update(value: UpdateOrganisationCapacityRequest) {
    this.form.disable();

    this.organisationCapacityService.update(value.organisationId, value)
      .finally(() => this.enableForm())
      .subscribe(
        result => {
          if (result) {
            this.router.navigate(['./../..'], { relativeTo: this.route });
            this.handleSaveSuccess();
          }
        },
        error => this.handleSaveError(error));
  }

  private setForm(organisationCapacity, allCapacities) {
    if (organisationCapacity) {
      if (!organisationCapacity.contacts)
        organisationCapacity.contacts = {};

      this.contactTypes.forEach(contactType => {
        if (!organisationCapacity.contacts[contactType.id])
          organisationCapacity.contacts[contactType.id] = '';
      });

      this.form.setValue(organisationCapacity);
      this.person = new SearchResult(organisationCapacity.personId, organisationCapacity.personName);
      this.location = new SearchResult(organisationCapacity.locationId, organisationCapacity.locationName);
    }

    this.capacities = allCapacities.map(k => new SelectItem(k.id, k.name));
    this.enableForm();
  }

  private handleError(error) {
    this.alertService.setAlert(
      new AlertBuilder()
        .error(error)
        .withTitle('Hoedanigheid kon niet geladen worden!')
        .withMessage('Er is een fout opgetreden bij het ophalen van de hoedanigheid. Probeer het later opnieuw.')
        .build()
    );
  }

  private handleSaveSuccess() {
    this.alertService.setAlert(
      new AlertBuilder()
        .success()
        .withTitle('Hoedanigheid bijgewerkt!')
        .withMessage('Hoedanigheid is succesvol bijgewerkt.')
        .build());
  }

  private handleSaveError(error) {
    this.alertService.setAlert(
      new AlertBuilder()
        .error(error)
        .withTitle('Hoedanigheid kon niet bewaard worden!')
        .withMessage('Er is een fout opgetreden bij het bewaren van de gegevens. Probeer het later opnieuw.')
        .build());
  }
}
