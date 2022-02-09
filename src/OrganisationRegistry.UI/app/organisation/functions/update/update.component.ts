import {Component, OnDestroy, OnInit, Renderer} from '@angular/core';
import { FormBuilder, FormGroup, FormControl } from '@angular/forms';
import { ActivatedRoute, Router} from '@angular/router';

import { Observable } from 'rxjs/Observable';

import { AlertService, AlertBuilder} from 'core/alert';
import { required } from 'core/validation';

import { SelectItem } from 'shared/components/form/form-group-select';
import { SearchResult } from 'shared/components/form/form-group-autocomplete';

import { PersonService } from 'services/people';
import { FunctionService } from 'services/functions';
import { ContactTypeListItem } from 'services/contacttypes';

import {
  UpdateOrganisationFunctionRequest,
  OrganisationFunctionService
} from 'services/organisationfunctions';
import {Subscription} from "rxjs/Subscription";

@Component({
  templateUrl: 'update.template.html',
  styleUrls: ['update.style.css']
})
export class OrganisationFunctionsUpdateOrganisationFunctionComponent implements OnInit, OnDestroy {
  public form: FormGroup;
  public functions: SelectItem[];
  public person: SearchResult;
  public contactTypes: ContactTypeListItem[];
  private functionId: string;

  private readonly subscriptions: Subscription[] = new Array<Subscription>();

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private formBuilder: FormBuilder,
    private functionService: FunctionService,
    private personService: PersonService,
    private organisationFunctionService: OrganisationFunctionService,
    private alertService: AlertService,
    private renderer: Renderer
  ) {
    this.form = formBuilder.group({
      organisationFunctionId: ['', required],
      organisationId: ['', required],
      functionId: ['', required],
      functionName: ['', required],
      personId: ['', required],
      personName: ['', required],
      validFrom: [''],
      validTo: [''],
    });
  }

  ngOnInit() {
    this.contactTypes = this.route.snapshot.data['contactTypes'];
    this.form.addControl('contacts', this.toFormGroup(this.contactTypes));

    let allFunctionsObservable = this.functionService.getAllFunctions();

    this.subscriptions.push(Observable.zip(this.route.parent.parent.params, this.route.params)
      .subscribe(res => {
        this.form.disable();
        let orgId = res[0]['id'];
        let functionId = res[1]['id'];

        this.subscriptions.push(Observable.zip(this.organisationFunctionService.get(orgId, functionId), allFunctionsObservable)
          .subscribe(
            item => this.setForm(item[0], item[1]),
            error => this.handleError(error)));
      }));
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

  get isFormValid() {
    return this.form.enabled && this.form.valid;
  }

  update(value: UpdateOrganisationFunctionRequest) {
    this.form.disable();

    this.subscriptions.push(this.organisationFunctionService.update(value.organisationId, value)
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

  private setForm(organisationFunction, allFunctions) {
    if (organisationFunction) {
      if (!organisationFunction.contacts)
        organisationFunction.contacts = {};

      this.contactTypes.forEach(contactType => {
        if (!organisationFunction.contacts[contactType.id])
          organisationFunction.contacts[contactType.id] = '';
      });

      this.form.setValue(organisationFunction);
      this.person = new SearchResult(organisationFunction.personId, organisationFunction.personName);
    }

    this.functions = allFunctions.map(k => new SelectItem(k.id, k.name));
    this.form.enable();
  }

  private handleError(error) {
    this.alertService.setAlert(
      new AlertBuilder()
        .error(error)
        .withTitle('Functie kon niet geladen worden!')
        .withMessage('Er is een fout opgetreden bij het ophalen van de functie. Probeer het later opnieuw.')
        .build()
    );
  }

  private handleSaveSuccess() {
    this.alertService.setAlert(
      new AlertBuilder()
        .success()
        .withTitle('Functie bijgewerkt!')
        .withMessage('Functie is succesvol bijgewerkt.')
        .build());
  }

  private handleSaveError(error) {
    this.alertService.setAlert(
      new AlertBuilder()
        .error(error)
        .withTitle('Functie kon niet bewaard worden!')
        .withMessage('Er is een fout opgetreden bij het bewaren van de gegevens. Probeer het later opnieuw.')
        .build());
  }
}
