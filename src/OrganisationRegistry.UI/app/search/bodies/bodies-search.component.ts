import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';

import { Subscription } from 'rxjs/Subscription';

import { Alert, AlertBuilder, AlertService, AlertType } from '../../core/alert';
import { PagedResult, PagedEvent, SortOrder } from '../../core/pagination';

import {
  BodySearchListItem,
  BodySearchService
} from '../../services/search/body';

@Component({
  templateUrl: 'bodies-search.template.html',
  styleUrls: ['bodies-search.style.css']
})
export class BodySearchComponent implements OnInit {
  public isLoading: boolean = true;
  public bodies: PagedResult<BodySearchListItem> = new PagedResult<BodySearchListItem>();

  public query: string;
  private subscription: Subscription = new Subscription();

  constructor(
    private alertService: AlertService,
    private searchService: BodySearchService,
    private route: ActivatedRoute,
    private router: Router) {}

  ngOnInit() {
    this.subscription = this.route
      .queryParams
      .subscribe(params => {
          // Defaults to empty string if no query param provided.
          this.query = params['q'] || '';
      });

      this.loadBodies();
    }

  ngOnDestroy() {
    this.subscription.unsubscribe();
  }

  private loadBodies() {
    this.isLoading = true;
    let bodies = this.searchService.search(this.query, 0, 10);

    bodies
      .finally(() => this.isLoading = false)
      .subscribe(
        newBodies => this.bodies = newBodies,
        error => this.alertService.setAlert(
          new AlertBuilder()
            .error(error)
            .withTitle('Organen kunnen niet geladen worden!')
            .withMessage('Er is een fout opgetreden bij het ophalen van de gegevens. Probeer het later opnieuw.')
            .build()
        ));
  }
}
