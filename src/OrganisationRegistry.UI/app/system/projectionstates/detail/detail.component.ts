import {Component, OnDestroy, OnInit} from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { FormBuilder, FormGroup } from '@angular/forms';
import { Observable } from 'rxjs/Observable';

import { AlertBuilder, AlertService} from 'core/alert';
import { required } from 'core/validation';

import { ProjectionState, ProjectionStateService } from 'services/projectionstates';
import {Subscription} from "rxjs/Subscription";

@Component({
    templateUrl: 'detail.template.html',
    styleUrls: [ 'detail.style.css' ]
  })
  export class ProjectionStateDetailComponent implements OnInit, OnDestroy {
    public form: FormGroup;
    public isBusy: boolean;

    private readonly subscriptions: Subscription[] = new Array<Subscription>();

    constructor(
      private route: ActivatedRoute,
      private router: Router,
      private formBuilder: FormBuilder,
      private alertService: AlertService,
      private projectionStateService: ProjectionStateService
    ) {
      this.form = formBuilder.group({
        id: ['', required],
        name: ['', required],
        eventNumber: [0, required]
      });
    }

    ngOnInit() {
      this.subscriptions.push(Observable.zip(this.route.params)
        .subscribe(res => {
          this.form.disable();
          let stateId = res[0]['id'];

          this.subscriptions.push(Observable.zip(this.projectionStateService.get(stateId))
            .subscribe(
              item => this.setForm(item[0]),
              error => this.handleError(error)));
        }));
    }

    ngOnDestroy() {
      this.subscriptions.forEach(sub => sub.unsubscribe());
    }

    update(value: ProjectionState) {
      this.form.disable();

      this.subscriptions.push(this.projectionStateService.update(value)
        .finally(() => this.form.enable())
        .subscribe(
          result => {
            if (result) {
              this.router.navigate(['./..'], {relativeTo: this.route});
              this.handleSaveSuccess();
            }
          },
          error => this.handleSaveError(error)
        ));
    }

    get isFormValid() {
      return this.form.enabled && this.form.valid;
    }

    private setForm(projectionState) {
      if (projectionState) {
        this.form.setValue(projectionState);
      }

      this.form.enable();
    }

    private handleError(error) {
      this.alertService.setAlert(
        new AlertBuilder()
          .error(error)
          .withTitle('Projectie status kon niet geladen worden!')
          .withMessage('Er is een fout opgetreden bij het ophalen van de projectie status. Probeer het later opnieuw.')
          .build()
      );
    }

    private handleSaveSuccess() {
      this.alertService.setAlert(
        new AlertBuilder()
          .success()
          .withTitle('Projectie status bijgewerkt!')
          .withMessage('Projectie status is succesvol bijgewerkt.')
          .build());
    }

    private handleSaveError(error) {
      this.alertService.setAlert(
        new AlertBuilder()
          .error(error)
          .withTitle('Projectie status kon niet bewaard worden!')
          .withMessage('Er is een fout opgetreden bij het bewaren van de gegevens. Probeer het later opnieuw.')
          .build());
    }
  }

