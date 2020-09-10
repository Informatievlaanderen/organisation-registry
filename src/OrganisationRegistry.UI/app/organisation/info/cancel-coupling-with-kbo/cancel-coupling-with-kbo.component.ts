import { FormBuilder, FormGroup } from '@angular/forms';
import { ActivatedRoute, Router, Params } from '@angular/router';
import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';

import * as moment from 'moment/moment';

import { AlertService, AlertBuilder } from 'core/alert';

import {KboOrganisation, OrganisationService} from 'services/organisations';
import {CreateOrganisationFormValues} from "../couple-with-kbo/create-organisation.model";

@Component({
  templateUrl: 'cancel-coupling-with-kbo.template.html',
  styleUrls: ['cancel-coupling-with-kbo.style.css'],
})
export class OrganisationCancelCouplingWithKboComponent implements OnInit{
  @Output('onCheckkboNumber') onCheckkboNumber: EventEmitter<string> = new EventEmitter<string>();

  public kboNumberForm: FormGroup;
  public organisationFromKbo: KboOrganisation;
  public today: string;
  public organisationId: string;

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

  ngOnInit() {
    this.route.parent.parent.params
      .forEach((params: Params) => {
      this.organisationId = params['id'];
    });
  }

  submit() {
    this.organisationService.cancelCouplingWithKbo(this.organisationId, this.organisationFromKbo.kboNumber)
      .finally(() => {
        this.isBusy = false;
      })
      .subscribe(
        result => {
          this.router.navigate(['./..'], { relativeTo: this.route });

          this.alertService.setAlert(
            new AlertBuilder()
              .success()
              .withTitle('Organisatie bijgewerkt!')
              .withMessage('De koppeling met de KBO werd succesvol geannuleerd.')
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
}
