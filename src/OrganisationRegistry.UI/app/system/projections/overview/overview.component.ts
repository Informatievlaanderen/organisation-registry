import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Params, Router } from '@angular/router';
import { FormBuilder, FormGroup, Validator, Validators } from '@angular/forms';

import { AlertService, AlertBuilder, Alert, AlertType } from 'core/alert';
import { required } from 'core/validation';

import { SelectItem } from 'shared/components/form/form-group-select';

import { ProjectionService } from 'services/projections';
import {
  TaskService,
  TaskData
} from 'services/tasks';

@Component({
  templateUrl: 'overview.template.html',
  styleUrls: [ 'overview.style.css' ]
})
export class ProjectionOverviewComponent implements OnInit {
  public form: FormGroup;
  public projections: SelectItem[];

  get isFormValid() {
    return this.form.enabled && this.form.valid;
  }

  constructor(
    private route: ActivatedRoute,
    private formBuilder: FormBuilder,
    private alertService: AlertService,
    private projectionService: ProjectionService,
    private taskService: TaskService) {
    this.form = formBuilder.group({
      projection: ['', required],
    });
  }

  ngOnInit() {
    this.route.params.forEach((params: Params) => {
      let id = params['id'];

      this.form.disable();
      this.projectionService
        .getAllProjections()
        .finally(() => this.form.enable())
        .subscribe(
          allProjections => this.projections = allProjections.map(x => new SelectItem(x, x)),
          error =>
            this.alertService.setAlert(
              new AlertBuilder()
                .error(error)
                .withTitle('Projecties konden niet geladen worden!')
                .withMessage('Er is een fout opgetreden bij het ophalen van de projecties. Probeer het later opnieuw.')
                .build()));
    });
  }

  rebuildProjection(value) {
    this.form.disable();
    let taskData = new TaskData('rebuildProjection', [value.projection]);

    this.taskService.submit(taskData)
      .finally(() => this.form.enable())
      .subscribe(
        result => this.alertService.setAlert(
            new AlertBuilder()
              .success()
              .withTitle('Taak uitgevoerd!')
              .withMessage('De taak is succesvol uitgevoerd.')
              .build()),
        error =>
          this.alertService.setAlert(
            new AlertBuilder()
              .error(error)
              .withTitle('Taak kon niet verstuurd worden!')
              .withMessage('Er is een fout opgetreden bij het versturen van de taak. Probeer het later opnieuw.')
              .build()));
  }
}
