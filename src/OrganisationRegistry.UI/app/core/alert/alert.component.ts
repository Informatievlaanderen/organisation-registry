import { Component, trigger, state, animate, transition, style  } from '@angular/core';

import { AlertService } from './alert.service';
import { Alert, AlertType } from './alert.model';

@Component({
  selector: 'ww-alert',
  templateUrl: './alert.template.html',
  styleUrls: ['./alert.style.css'],
  animations: [
    trigger('visibility', [
      state('shown', style({
        display: 'block',
        opacity: 1
      })),
      state('hidden', style({
        display: 'none',
        opacity: 0
      })),
      transition('* => *', animate('.5s'))
    ])
  ]
})
export class AlertComponent {

  public alertType = AlertType;

  private hideInProgress: boolean = false;

  public get visible(): string {
    if (this.hideInProgress)
      return 'hidden';

    if (this.alertService.hasAlerts)
      return 'shown';

    return 'hidden';
  }

  public get alert(): Alert {
    return this.alertService.activeAlert || new Alert(AlertType.Success, '', '', '');
  }

  public get alertClass(): string {
    switch (this.alert.type) {
      case AlertType.Warning:
        return 'alert--warning';
      case AlertType.Error:
        return 'alert--error';
      case AlertType.Success:
        return 'alert--success';
      default:
        return '';
    }
  }

  constructor(private alertService: AlertService) { }

  public dismissAlert() {
    this.hideInProgress = true;

    setTimeout(function() {
      this.alertService.clearAlert();
      this.hideInProgress = false;
    }.bind(this), 1000);
  }
}
