import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import {ChangeDetectionStrategy, Component, EventEmitter, Input, OnDestroy, OnInit, Output} from '@angular/core';
import { CreateOrganisationFormValues } from './create-organisation-form.model';
import { required, isKboNumber } from 'core/validation';
import { AlertService, AlertBuilder } from 'core/alert';

import {
  Kbo,
  KboService
} from 'services/kbo';
import {SelectItem} from "../../../shared/components/form/form-group-select";
import {Subscription} from "rxjs/Subscription";

@Component({
  selector: 'ww-create-organisation-form',
  templateUrl: 'create-organisation-form.template.html',
  styleUrls: ['create-organisation-form.style.css'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class CreateOrganisationFormComponent implements OnInit, OnDestroy {
  @Input('cancelRouterLink') cancelRouterLink;
  @Input('primaryButtonText') primaryButtonText;
  @Input('organisation') organisation;
  @Input('purposes') purposes;
  @Input('showOnVlaamseOverheidSites') showOnVlaamseOverheidSites;
  @Output('onSubmit') onSubmit: EventEmitter<CreateOrganisationFormValues> = new EventEmitter<CreateOrganisationFormValues>();

  public form: FormGroup;
  public kboForm: FormGroup;
  public kboNumber: string;
  public articles: SelectItem[];

  private readonly subscriptions: Subscription[] = new Array<Subscription>();

  @Input('isBusy')
  public set isBusy(value: boolean) {
    if (value) {
      this.form.disable();
      this.kboForm.disable();
    } else {
      this.form.enable();
      this.kboForm.enable();

      if (this.hasKbo) {
        this.disableKboControls();
      }
    }
  }

  public get hasKbo() {
    return this.form.controls['kboNumber'].value;
  }

  constructor(
    private formBuilder: FormBuilder,
    private kboService: KboService,
    private alertService: AlertService
  ) {
    this.form = formBuilder.group({
      id: ['', required],
      name: ['', required],
      shortName: ['', Validators.nullValidator],
      article: [''],
      parentOrganisationId: ['', Validators.nullValidator],
      description: ['', Validators.nullValidator],
      purposeIds: [[]],
      purposes: [[]],
      showOnVlaamseOverheidSites: [false],
      validFrom: [''],
      validTo: [''],
      operationalValidFrom: [''],
      operationalValidTo: [''],
      kboNumber: ['', Validators.nullValidator],
      bankAccounts: [[]],
      legalForms: [[]],
      addresses: [[]]
    });

    this.articles = [
      new SelectItem('de', 'de'),
      new SelectItem('het', 'het'),
    ]

    this.kboForm = formBuilder.group({
      kboNumber: ['', isKboNumber]
    });
  }

  ngOnDestroy() {
    this.subscriptions.forEach(sub => sub.unsubscribe());
  }

  disableKboControls() {
    this.form.controls['name'].disable();
    this.form.controls['shortName'].disable();
    this.form.controls['validFrom'].disable();
    this.form.controls['validTo'].disable();
  }

  enableKboControls() {
    this.form.controls['name'].enable();
    this.form.controls['shortName'].enable();
    this.form.controls['validFrom'].enable();
    this.form.controls['validTo'].enable();
  }

  ngOnInit() {
    this.form.setValue(this.organisation);
  }

  submit(value: CreateOrganisationFormValues) {
    this.onSubmit.next(value);
  }

  search(value) {
    this.isBusy = true;
    this.subscriptions.push(this.kboService.get(value.kboNumber)
      .finally(() => this.isBusy = false)
      .subscribe(
        result => {
          if (result.formalName && result.formalName.value) {
            this.form.controls['name'].setValue(result.formalName.value);
            this.form.controls['shortName'].setValue(result.shortName.value);
            this.form.controls['kboNumber'].setValue(result.kboNumber);
            this.form.controls['validFrom'].setValue(result.validFrom);
            this.form.controls['validTo'].setValue(result.validTo);
          } else {
            this.alertService.setAlert(
              new AlertBuilder()
                .error()
                .withTitle('KBO Nr onbekend')
                .withMessage('Het opgegeven KBO Nr werd niet terugevonden in de KBO.')
                .build())
          }
        },
        error =>
          this.alertService.setAlert(
            new AlertBuilder()
              .error(error)
              .withTitle('Probleem bij het ophalen van de KBO informatie')
              .withMessage('Er is een fout opgetreden bij het ophalen van de organisatie in de KBO. Probeer het later opnieuw.')
              .build())));
  }
}
