import { FormBuilder, FormGroup } from '@angular/forms';
import { ActivatedRoute, Router, Params } from '@angular/router';
import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';

import * as moment from 'moment/moment';

import { AlertService, AlertBuilder } from 'core/alert';

import { OrganisationSyncService } from 'services/organisationsync';

@Component({
  templateUrl: 'cancel-coupling-with-kbo.template.html',
  styleUrls: ['cancel-coupling-with-kbo.style.css'],
})
export class OrganisationCancelCouplingWithKboComponent implements OnInit{
  @Output('onCheckkboNumber') onCheckkboNumber: EventEmitter<string> = new EventEmitter<string>();

  public kboNumberForm: FormGroup;
  public today: string;
  public organisationId: string;

  constructor(
    formBuilder: FormBuilder,
    private route: ActivatedRoute,
    private router: Router,
    private organisationSyncService: OrganisationSyncService,
    private alertService: AlertService  ) {

    this.today = moment().format('YYYY-MM-DD');

    this.kboNumberForm = formBuilder.group({
      kboNumber: [''],
    });
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
    this.organisationSyncService.cancelCouplingWithKbo(this.organisationId)
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
              .withMessage('De koppeling met de KBO werd succesvol ongedaan gemaakt.')
              .build());
        },
        error => {
          this.router.navigate(['./..'], { relativeTo: this.route });
          this.alertService.setAlert(
            new AlertBuilder()
              .error(error)
              .build());
        });
  }
}
