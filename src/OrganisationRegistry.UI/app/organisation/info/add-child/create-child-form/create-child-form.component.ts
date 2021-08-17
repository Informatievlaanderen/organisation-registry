import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import {
  ChangeDetectionStrategy,
  Component,
  EventEmitter,
  Input,
  OnChanges,
  OnInit,
  Output,
  SimpleChanges
} from '@angular/core';
import { required } from 'core/validation';

import { CreateOrganisationFormValues } from './create-child-form.model';
import {SelectItem} from "../../../../shared/components/form/form-group-select";

@Component({
  selector: 'ww-create-child-form',
  templateUrl: 'create-child-form.template.html',
  styleUrls: ['create-child-form.style.css'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class CreateChildOrganisationFormComponent implements OnInit, OnChanges {
  @Input('cancelRouterLink') cancelRouterLink;
  @Input('primaryButtonText') primaryButtonText;
  @Input('organisation') organisation;
  @Input('parentOrganisationId') parentOrganisationId;
  @Input('purposes') purposes;

  @Input('isBusy')
  public set isBusy(value: boolean) {
    if (value) {
      this.form.disable();
    } else {
      this.form.enable();
    }
  }

  @Output('onSubmit') onSubmit: EventEmitter<CreateOrganisationFormValues> = new EventEmitter<CreateOrganisationFormValues>();

  public form: FormGroup;
  public articles: SelectItem[];

  constructor(
    private formBuilder: FormBuilder
  ) {
    this.form = formBuilder.group({
      id: ['', required],
      name: ['', required],
      shortName: ['', Validators.nullValidator],
      article: [''],
      description: ['', Validators.nullValidator],
      parentOrganisationId: ['', required],
      purposeIds: [[]],
      purposes: [[]],
      showOnVlaamseOverheidSites: [false],
      validFrom: [''],
      validTo: [''],
      operationalValidFrom: [''],
      operationalValidTo: [''],
    });
    this.articles = [
      new SelectItem('de', 'de'),
      new SelectItem('het', 'het'),
    ]
  }

  ngOnInit() {
    this.form.setValue(this.organisation);
  }

  ngOnChanges(changes: SimpleChanges) {
    this.form.get('parentOrganisationId').setValue(this.parentOrganisationId);
  }

  submit(value: CreateOrganisationFormValues) {
    this.onSubmit.next(value);
  }
}
