import { FormBuilder, FormGroup } from '@angular/forms';
import { ActivatedRoute, Router, Params } from '@angular/router';
import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';

import * as moment from 'moment/moment';

import { AlertService, AlertBuilder } from 'core/alert';

import { Difference } from './difference.model';
import { CreateOrganisationFormValues } from './create-organisation.model';
import {KboOrganisation, OrganisationService} from 'services/organisations';

@Component({
  templateUrl: 'couple-with-kbo.template.html',
  styleUrls: ['couple-with-kbo.style.css'],
})
export class OrganisationCoupleWithKboComponent implements OnInit {
  @Output('onSubmit') onSubmit: EventEmitter<CreateOrganisationFormValues> = new EventEmitter<CreateOrganisationFormValues>();
  @Output('onCheckkboNumber') onCheckkboNumber: EventEmitter<string> = new EventEmitter<string>();

  public kboNumberForm: FormGroup;
  public organisation: CreateOrganisationFormValues;
  public organisationFromKbo: KboOrganisation;
  public today: string;
  public organisationId: string;
  public differences: Difference[] = new Array();

  constructor(
    formBuilder: FormBuilder,
    private route: ActivatedRoute,
    private router: Router,
    private organisationService: OrganisationService,
    private alertService: AlertService  ) {

    this.today = moment().format('YYYY-MM-DD');

    this.kboNumberForm = formBuilder.group({
      kboNumber: [''],
    });
    this.organisationFromKbo = new KboOrganisation();
  }

  @Input('isBusy')
  public set isBusy(isBusy: boolean) {
    if (isBusy) {
      this.kboNumberForm.disable();
    } else {
      this.kboNumberForm.enable();
    }
  }

  public get primaryButtonText() {
    return this.organisationFromKbo.kboNumber ?
      'Waarden overnemen' :
      'Controleer Kbo nummer';
  }

  ngOnInit() {
    this.kboNumberForm.disable();
    this.route.parent.parent.params.forEach((params: Params) => {
      this.organisationId = params['id'];

      this.organisationService.get(this.organisationId)
        .finally(() => this.kboNumberForm.enable())
        .subscribe(
          item => {
            if (item) {
              this.kboNumberForm.setValue({ kboNumber: item.kboNumber });
              this.organisation = new CreateOrganisationFormValues();
              this.organisation.id = item.id;
              this.organisation.name = item.name;
              this.organisation.shortName = item.shortName;
              this.organisation.kboNumber = item.kboNumber;
            }
          },
          error => this.alertService.setAlert(
            new AlertBuilder()
              .error(error)
              .withTitle('Persoon kon niet geladen worden!')
              .withMessage('Er is een fout opgetreden bij het ophalen van de persoon. Probeer het later opnieuw.')
              .build()));
    });
  }

  submit() {
    if (this.organisationFromKbo.kboNumber) {
      this.putInsz();
    } else {
      this.getOrganisationFromKbo();
    }
  }

  private putInsz(): any {
    this.isBusy = true;
    this.alertService.clearAlert();
    this.organisationService.putKboNumber(this.organisationId, this.organisationFromKbo.kboNumber)
      .finally(() => {
        this.isBusy = false;
      })
      .subscribe(
        result => {
          this.router.navigate(['./..'], { relativeTo: this.route });

          this.alertService.setAlert(
            new AlertBuilder()
              .success()
              .withTitle('Persoon bijgewerkt!')
              .withMessage('Het kbo nummer werd succesvol gekoppeld.')
              .build());
        },
        error => {
            this.router.navigate(['./..'], { relativeTo: this.route });
            this.alertService.setAlert(
              new AlertBuilder()
                .error(error)
                .build());
            this.organisationFromKbo = null;
        });
  }

  private getOrganisationFromKbo(): any {
    this.isBusy = true;
    this.alertService.clearAlert();
    this.differences = new Array();

    this.organisationService.checkKbo(this.kboNumberForm.get('kboNumber').value)
      .finally(() => {
        this.isBusy = false;
      })
      .subscribe(
        result => {
          this.organisationFromKbo = result;

          if (this.organisationFromKbo.formalName.value !== this.organisation.name) {
            this.differences.push(new Difference('Naam', this.organisation.name, this.organisationFromKbo.formalName.value));
          }
          if (this.organisationFromKbo.shortName.value !== this.organisation.shortName) {
            this.differences.push(new Difference('Korte naam', this.organisation.shortName, this.organisationFromKbo.shortName.value));
          }
          if (this.differences.length > 0) {
            this.alertService.setAlert(
              new AlertBuilder()
                .alert()
                .withTitle('Verschillen ontdekt tussen de data uit het VKBO en lokale data!')
                .withMessage('Indien u verdergaat, zullen de onderstaande waarden overschreven worden.')
                .build());
          }
        },
        error => {
          this.alertService.setAlert(
            new AlertBuilder()
              .alert()
              .withTitle('Kbo nummer')
              .withMessage('Het opgegeven kbo nummer werd niet gevonden.')
              .build());
        });
  }
}
