import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';

import { Subscription } from 'rxjs/Subscription';

import { Alert, AlertBuilder, AlertService, AlertType } from '../../core/alert';
import { PagedResult, PagedEvent, SortOrder } from '../../core/pagination';

import {
  PersonSearchListItem,
  PersonSearchService
} from '../../services/search/person';

@Component({
  templateUrl: 'people-search.template.html',
  styleUrls: ['people-search.style.css']
})
export class PersonSearchComponent implements OnInit {
  public isLoading: boolean = true;
  public people: PagedResult<PersonSearchListItem> = new PagedResult<PersonSearchListItem>();

  public query: string;
  private subscription: Subscription = new Subscription();

  constructor(
    private alertService: AlertService,
    private searchService: PersonSearchService,
    private route: ActivatedRoute,
    private router: Router) {}

  ngOnInit() {
    this.subscription = this.route
      .queryParams
      .subscribe(params => {
          // Defaults to empty string if no query param provided.
        this.query = params['q'] || '';
      });

      this.loadPeople();
    }

  ngOnDestroy() {
    this.subscription.unsubscribe();
  }

  private loadPeople() {
    this.isLoading = true;
    let people = this.searchService.search(this.query, 0, 10);

    people
      .finally(() => this.isLoading = false)
      .subscribe(
        newPeople => this.people = newPeople,
        error => this.alertService.setAlert(
          new AlertBuilder()
            .error(error)
            .withTitle('Personen kunnen niet geladen worden!')
            .withMessage('Er is een fout opgetreden bij het ophalen van de gegevens. Probeer het later opnieuw.')
            .build()
        ));
  }
}
