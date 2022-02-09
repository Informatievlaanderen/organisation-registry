import {Component, OnDestroy, OnInit} from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { ActivatedRoute, Router, Params } from '@angular/router';

import { AlertService, AlertBuilder} from 'core/alert';
import { required } from 'core/validation';

import { SelectItem } from 'shared/components/form/form-group-select';

import {
  CreateBodyLifecyclePhaseRequest,
  BodyLifecyclePhaseService
} from 'services/bodylifecyclephases';

import { LifecyclePhaseTypeService } from 'services/lifecyclephasetypes';
import {Subscription} from "rxjs/Subscription";

@Component({
  templateUrl: 'create.template.html',
  styleUrls: ['create.style.css']
})
export class BodyLifecyclePhasesCreateBodyLifecyclePhaseComponent implements OnInit, OnDestroy {
  public form: FormGroup;
  public lifecyclePhaseTypes: SelectItem[];

  private readonly subscriptions: Subscription[] = new Array<Subscription>();

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private formBuilder: FormBuilder,
    private lifecyclePhaseTypeService: LifecyclePhaseTypeService,
    private bodyLifecyclePhaseService: BodyLifecyclePhaseService,
    private alertService: AlertService
  ) {
    this.form = formBuilder.group({
      bodyLifecyclePhaseId: ['', required],
      bodyId: ['', required],
      lifecyclePhaseTypeId: ['', required],
      validFrom: [''],
      validTo: ['']
    });
  }

  ngOnInit() {
    this.lifecyclePhaseTypes = this.route.snapshot.data['lifecyclePhaseTypes'].map(k => new SelectItem(k.id, k.name));

    this.route.parent.parent.params.forEach((params: Params) => {
      this.form.setValue(new CreateBodyLifecyclePhaseRequest(params['id']));
    });
  }

  ngOnDestroy() {
    this.subscriptions.forEach(sub => sub.unsubscribe());
  }

  get isFormValid() {
    return this.form.enabled && this.form.valid;
  }

  create(value: CreateBodyLifecyclePhaseRequest) {
    this.form.disable();

    this.subscriptions.push(this.bodyLifecyclePhaseService.create(value.bodyId, value)
      .finally(() => this.form.enable())
      .subscribe(
        result => {
          if (result) {
            this.router.navigate(['./..'], { relativeTo: this.route });

            this.alertService.setAlert(
              new AlertBuilder()
                .success()
                .withTitle('Levensloopfase gekoppeld!')
                .withMessage('Levensloopfase is succesvol gekoppeld.')
                .build());
          }
        },
        error =>
          this.alertService.setAlert(
            new AlertBuilder()
              .error(error)
              .withTitle('Levensloopfase kon niet gekoppeld worden!')
              .withMessage('Er is een fout opgetreden bij het koppelen van de gegevens. Probeer het later opnieuw.')
              .build())));
  }
}
