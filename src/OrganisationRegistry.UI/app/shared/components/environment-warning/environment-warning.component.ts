import {Component, OnInit, Input} from '@angular/core';

import {Environments} from "../../../environments";

@Component({
  selector: 'ww-environment-warning',
  styleUrls: ['./environment-warning.style.css'],
  templateUrl: 'environment-warning.template.html'
})
export class EnvironmentWarningComponent implements OnInit {
  @Input('environment') environment: string;

  public showEnvironment: boolean;

  constructor() {
  }

  ngOnInit() {
    this.showEnvironment = this.environment != Environments.production;
  }
}
