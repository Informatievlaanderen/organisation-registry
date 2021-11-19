import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { ActivatedRoute, Router, Params } from '@angular/router';

import { AlertService, AlertBuilder, Alert, AlertType } from 'core/alert';
import { CreateAlertMessages } from 'core/alertmessages';
import { required } from 'core/validation';

import { SelectItem } from 'shared/components/form/form-group-select';

import * as showdown from 'showdown';

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

  private converter = new showdown.Converter();

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
      name: ['', required],
      url: [''],
      date: [''],
      description: [''],
      descriptionRendered: [''],
      validFrom: [''],
      validTo: ['']
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
    this.form.controls['description'].valueChanges.subscribe(function(x) {
      this.form.controls['descriptionRendered'].patchValue(this.converter.makeHtml(x));
    }.bind(this));

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

  showExample(e) {
    if(this.form.controls['description'].value === '' ||
      confirm('Let op, hierdoor zult u de huidige tekst verliezen. Wilt u doorgaan?')){
      this.form.controls["description"].patchValue('# h1 Titel\n' +
        'Gevolgd door wat tekst.\n' +
        '## h2 Titel\n' +
        '\n' +
        'Gevolgd door nog wat tekst.\n' +
        '### h3 Titel\n' +
        'We kunnen ook meerdere paragrafen voorzien.\n' +
        '\n' +
        'Hiervoor plaatsen we een lege lijn tussen de paragrafen. \n' +
        'Lijnen die niet gescheiden zijn door een lege lijn, worden bij dezelfde paragraaf geplaatst.\n' +
        '#### h4 Titel\n' +
        '##### h5 Titel\n' +
        '###### h6 Titel\n' +
        '___\n' +
        '\n' +
        '## Tekststijl\n' +
        '\n' +
        '**Deze tekst is bold**\n' +
        '\n' +
        '__Deze tekst is ook bold__\n' +
        '\n' +
        '*Deze tekst is italic*\n' +
        '\n' +
        '_Deze tekst is ook italic_\n' +
        '\n' +
        '## Links\n' +
        '\n' +
        '[link text](http://dev.nodeca.com)\n' +
        '\n' +
        '## Afbeeldingen\n' +
        '\n' +
        '![Vlaanderen](https://www.vlaanderen.be/img/logo/vlaanderen-logo.svg)\n' +
        '\n' +
        '## Lijsten\n' +
        '\n' +
        '#### Ongeordend\n' +
        '\n' +
        '+ Maak een lijst met *,\n' +
        '+ Maak een lijst met +, of \n' +
        '+ Maak een lijst met -\n' +
        '\n' +
        '#### Ongeordend\n' +
        '\n' +
        '1. Lorem ipsum dolor sit amet\n' +
        '2. Consectetur adipiscing elit\n' +
        '3. Integer molestie lorem at massa\n' +
        '\n' +
        '\n' +
        '## Quotes\n' +
        '\n' +
        '\n' +
        '> Quotes kunnen genest worden...\n' +
        '>> ...door verschillende groter dan tekens na elkaar...\n' +
        '> > > ...of met spaties tussen de tekens.\n' +
        '\n' +
        '## Code\n' +
        '\n' +
        'Indented code\n' +
        '\n' +
        '    // Some comments\n' +
        '    line 1 of code\n' +
        '    line 2 of code\n' +
        '    line 3 of code\n' +
        '\n' +
        '\n' +
        'Block code\n' +
        '\n' +
        '```\n' +
        'Sample text here...\n' +
        'console.log(foo(5));\n' +
        '```');
    }
    return false;
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
