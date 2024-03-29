import {Component, OnDestroy, OnInit, Renderer} from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { ActivatedRoute, Router} from '@angular/router';

import { Observable } from 'rxjs/Observable';

import { AlertService, AlertBuilder} from 'core/alert';
import { required } from 'core/validation';

import { SelectItem } from 'shared/components/form/form-group-select';

import {
  UpdateBodyLifecyclePhaseRequest,
  BodyLifecyclePhaseService
} from 'services/bodylifecyclephases';

import { LifecyclePhaseTypeService } from 'services/lifecyclephasetypes';
import {Subscription} from "rxjs/Subscription";

@Component({
  templateUrl: 'update.template.html',
  styleUrls: ['update.style.css']
})
export class BodyLifecyclePhasesUpdateBodyLifecyclePhaseComponent implements OnInit, OnDestroy {
  public form: FormGroup;
  public lifecyclePhaseTypes: SelectItem[];

  private readonly subscriptions: Subscription[] = new Array<Subscription>();

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private formBuilder: FormBuilder,
    private lifecyclePhaseTypeService: LifecyclePhaseTypeService,
    private bodyLifecyclePhaseService: BodyLifecyclePhaseService,
    private alertService: AlertService,
    private renderer: Renderer
  ) {
    this.form = formBuilder.group({
      bodyLifecyclePhaseId: ['', required],
      bodyId: ['', required],
      lifecyclePhaseTypeId: ['', required],
      lifecyclePhaseTypeName: ['', required],
      validFrom: [''],
      validTo: [''],
      hasAdjacentGaps: [false]
    });
  }

  ngOnInit() {
    this.lifecyclePhaseTypes = this.route.snapshot.data['lifecyclePhaseTypes'].map(k => new SelectItem(k.id, k.name));

    this.subscriptions.push(Observable.zip(this.route.parent.parent.params, this.route.params)
      .subscribe(res => {
        this.form.disable();
        let orgId = res[0]['id'];
        let bodyLifecyclePhaseId = res[1]['id'];

        this.subscriptions.push(this.bodyLifecyclePhaseService
          .get(orgId, bodyLifecyclePhaseId)
          .subscribe(
            item => this.setForm(item),
            error => this.handleError(error)));
      }));
  }

  ngOnDestroy() {
    this.subscriptions.forEach(sub => sub.unsubscribe());
  }

  get isFormValid() {
    return this.form.enabled && this.form.valid;
  }

  update(value: UpdateBodyLifecyclePhaseRequest) {
    this.form.disable();

    this.subscriptions.push(this.bodyLifecyclePhaseService.update(value.bodyId, value)
      .finally(() => this.form.enable())
      .subscribe(
        result => {
          if (result) {
            this.router.navigate(['./../..'], { relativeTo: this.route });
            this.handleSaveSuccess();
          }
        },
        error => this.handleSaveError(error)
      ));
  }

  private setForm(bodyLifecyclePhase) {
    if (bodyLifecyclePhase) {
      this.form.setValue(bodyLifecyclePhase);
    }

    this.form.enable();
  }

  private handleError(error) {
    this.alertService.setAlert(
      new AlertBuilder()
        .error(error)
        .withTitle('Levensloopfase kon niet geladen worden!')
        .withMessage('Er is een fout opgetreden bij het ophalen van de levensloopfase. Probeer het later opnieuw.')
        .build()
    );
  }

  private handleSaveSuccess() {
    this.alertService.setAlert(
      new AlertBuilder()
        .success()
        .withTitle('Levensloopfase bijgewerkt!')
        .withMessage('Levensloopfase is succesvol bijgewerkt.')
        .build());
  }

  private handleSaveError(error) {
    this.alertService.setAlert(
      new AlertBuilder()
        .error(error)
        .withTitle('Levensloopfase kon niet bewaard worden!')
        .withMessage('Er is een fout opgetreden bij het bewaren van de gegevens. Probeer het later opnieuw.')
        .build());
  }
}
