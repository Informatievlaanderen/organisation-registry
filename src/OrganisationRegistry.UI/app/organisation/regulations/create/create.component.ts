import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { ActivatedRoute, Router, Params } from '@angular/router';

import { AlertService, AlertBuilder, Alert, AlertType } from 'core/alert';
import { CreateAlertMessages } from 'core/alertmessages';
import { required } from 'core/validation';

import { SelectItem } from 'shared/components/form/form-group-select';

import {
  CreateOrganisationRegulationRequest,
  OrganisationRegulationService
} from 'services/organisationregulations';

import { RegulationThemeService } from 'services/regulation-themes';
import { RegulationSubThemeService } from 'services/regulation-sub-themes';

@Component({
  templateUrl: 'create.template.html',
  styleUrls: ['create.style.css']
})
export class OrganisationRegulationsCreateOrganisationRegulationComponent implements OnInit {
  public form: FormGroup;
  public regulationThemes: SelectItem[];
  public regulationSubThemes: Array<SelectItem> = [];

  private regulationTheme: string = '';
  private readonly createAlerts = new CreateAlertMessages('Regulation');

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private formBuilder: FormBuilder,
    private regulationThemeService: RegulationThemeService,
    private regulationSubThemeService: RegulationSubThemeService,
    private organisationRegulationService: OrganisationRegulationService,
    private alertService: AlertService
  ) {
    this.form = formBuilder.group({
      organisationRegulationId: ['', required],
      organisationId: ['', required],
      regulationThemeId: [''],
      regulationSubThemeId: [''],
      regulationName: ['', required],
      regulationUrl: [''],
      regulationDate: [''],
      description: [''],
      validFrom: [''],
      validTo: [''],
    });
  }

  ngOnInit() {
    this.form.disable();

    this.route.parent.parent.params.forEach((params: Params) => {
      this.form.setValue(new CreateOrganisationRegulationRequest(params['id']));
    });

    this.regulationThemeService
      .getAllRegulationThemes()
      .finally(() => this.enableForm())
      .subscribe(
        allRegulationThemes => this.regulationThemes = allRegulationThemes.map(k => new SelectItem(k.id, k.name)),
        error =>
          this.alertService.setAlert(
            new AlertBuilder()
              .error(error)
              .withTitle('Regelgevingthema\'s konden niet geladen worden!')
              .withMessage('Er is een fout opgetreden bij het ophalen van de regelgevingsthema\'s. Probeer het later opnieuw.')
              .build()));
    this.subscribeToFormChanges();
  }

  subscribeToFormChanges() {
    const regulationThemeChanges$ = this.form.controls['regulationThemeId'].valueChanges;

    regulationThemeChanges$
      .subscribe(function (regulationTheme) {
        if (this.regulationTheme === regulationTheme)
          return;

        this.regulationTheme = regulationTheme;

        this.form.patchValue({ regulationSubThemeId: '' });

        this.form.disable();

        if (regulationTheme) {
          this.regulationSubThemeService
            .getAllRegulationSubThemes(regulationTheme)
            .finally(() => this.enableForm())
            .subscribe(
              x => this.regulationSubThemes = x.map(c => new SelectItem(c.id, c.name)),
              error =>
                this.alertService.setAlert(
                  new AlertBuilder()
                    .error(error)
                    .withTitle('Regelgevingsubthema\'s konden niet geladen worden!')
                    .withMessage('Er is een fout opgetreden bij het ophalen van de regelgevingsubthema\'s. Probeer het later opnieuw.')
                    .build()));
        } else {
          this.enableForm();
        }
      }.bind(this));
  }

  enableForm() {
    this.form.enable();
    if (!this.regulationTheme)
      this.form.get('regulationSubThemeId').disable();
  }

  get isFormValid() {
    return this.form.enabled && this.form.valid;
  }

  create(value: CreateOrganisationRegulationRequest) {
    this.form.disable();

    this.organisationRegulationService.create(value.organisationId, value)
      .finally(() => this.enableForm())
      .subscribe(
        result => {
          if (result) {
            this.router.navigate(['./..'], { relativeTo: this.route });

            this.alertService.setAlert(
              new AlertBuilder()
                .success()
                .withTitle('Regelgeving aangemaakt!')
                .withMessage('Regelgeving is succesvol aangemaakt.')
                .build());
          }
        },
        error =>
          this.alertService.setAlert(
            new AlertBuilder()
              .error(error)
              .withTitle('Regelgeving kon niet bewaard worden!')
              .withMessage('Er is een fout opgetreden bij het bewaren van de gegevens. Probeer het later opnieuw.')
              .build()));
  }
}
