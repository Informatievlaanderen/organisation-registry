# Location Registratie - Front-End

## Overzicht

Dit document beschrijft hoe de location registratie werkt in de front-end van de Organisatie Registry. De UI is gebouwd met **Angular** en biedt een intuïtieve interface voor het aanmaken, bewerken en zoeken van locaties met optionele **CRAB** (Centraal Referentie Adressenbestand) integratie.

## Technologie Stack

- **Framework**: Angular (met RxJS voor reactive programming)
- **Forms**: Reactive Forms (FormBuilder, FormGroup, FormControl)
- **Routing**: Angular Router met RoleGuard voor authorization
- **HTTP**: Angular Http module met custom HeadersBuilder
- **Validatie**: Custom validators + reactive form validation
- **UI Components**: Custom component library (ww-* prefixed components)

## Module Structuur

### Locations Module

**Bestand**: `src/OrganisationRegistry.UI/app/administration/locations/locations.module.ts`

De locations module is een feature module binnen de administration sectie:

```
administration/
└── locations/
    ├── locations.module.ts         # Module definitie
    ├── locations-routing.module.ts  # Routing configuratie
    ├── components/                  # Shared components
    │   ├── filter/                 # Filter component
    │   └── list/                   # List component
    ├── detail/                      # Create/Edit component
    │   ├── detail.component.ts
    │   └── detail.template.html
    └── overview/                    # Overview/list page
        ├── overview.component.ts
        └── overview.template.html
```

## Routing

### Routes

**Bestand**: `src/OrganisationRegistry.UI/app/administration/locations/locations-routing.module.ts`

```typescript
const routes: Routes = [
  {
    path: 'administration/locations',
    component: LocationOverviewComponent,
    canActivate: [RoleGuard],
    data: {
      roles: [Role.AlgemeenBeheerder, Role.CjmBeheerder],
      title: 'Parameters - Locaties'
    }
  },
  {
    path: 'administration/locations/create',
    component: LocationDetailComponent,
    canActivate: [RoleGuard],
    data: {
      roles: [Role.AlgemeenBeheerder, Role.CjmBeheerder],
      title: 'Parameters - Nieuwe locatie'
    }
  },
  {
    path: 'administration/locations/:id',
    component: LocationDetailComponent,
    canActivate: [RoleGuard],
    data: {
      roles: [Role.AlgemeenBeheerder, Role.CjmBeheerder],
      title: 'Parameters - Bewerken locatie'
    }
  },
];
```

### Authorization

Alle location routes zijn beveiligd met `RoleGuard` en vereisen één van deze rollen:
- **AlgemeenBeheerder**: Algemene beheerder
- **CjmBeheerder**: CJM beheerder

Gebruikers zonder deze rollen krijgen **403 Forbidden**.

## Data Models

### Location Model

**Bestand**: `src/OrganisationRegistry.UI/app/services/locations/location.model.ts`

```typescript
export class Location implements ICrudItem<Location> {
  public crabLocationId: string = '';
  public formattedAddress: string = '';
  public street: string = '';
  public zipCode: string = '';
  public city: string = '';
  public country: string = '';
  public id: string = '';

  constructor() {
    this.id = UUID.UUID();  // Automatically generates GUID
  }

  public get name() {
    return `${this.street}, ${this.zipCode} ${this.city}`;
  }

  public withValues(location: Location): Location {
    // Creates a new instance with copied values
  }
}
```

**Kenmerken**:
- **ID generatie**: Automatisch UUID bij constructie
- **name property**: Computed property voor display naam
- **withValues()**: Factory method voor immutable updates

### LocationListItem Model

**Bestand**: `src/OrganisationRegistry.UI/app/services/locations/location-list-item.model.ts`

```typescript
export class LocationListItem {
  constructor(
    public id: string = '',
    public crabLocationId: string = '',
    public formattedAddress: string = '',
    public street: string = '',
    public zipCode: string = '',
    public city: string = '',
    public country: string = '',
  ) { }
}
```

Simplified model voor lijstweergave (zonder computed properties).

### LocationFilter Model

**Bestand**: `src/OrganisationRegistry.UI/app/services/locations/location-filter.model.ts`

```typescript
export class LocationFilter {
  constructor(
    public street: string = '',
    public zipCode: string = '',
    public city: string = '',
    public country: string = '',
    public nonCrabOnly: boolean = false  // Filter voor niet-CRAB locaties
  ) { }
}
```

## Location Service

### API Communication

**Bestand**: `src/OrganisationRegistry.UI/app/services/locations/location.service.ts`

```typescript
@Injectable()
export class LocationService implements ICrudService<Location> {
  private locationsUrl = `${this.configurationService.apiUrl}/v1/locations`;

  // GET /v1/locations - Paginated list with filtering and sorting
  public getLocations(
    filter: LocationFilter,
    sortBy: string,
    sortOrder: SortOrder,
    page: number = 1,
    pageSize: number = 10
  ): Observable<PagedResult<LocationListItem>>

  // GET /v1/locations - Search with filter (bounded page size)
  public search(filter: LocationFilter): Observable<PagedResult<LocationListItem>>

  // GET /v1/locations - All locations without pagination
  public getAllLocations(): Observable<LocationListItem[]>

  // GET https://geo.api.vlaanderen.be/geolocation/v3/Location
  // JSONP call to CRAB API
  public getCrabLocations(searchTerm: string): Observable<any[]>

  // GET /v1/locations/{id}
  public get(id: string): Observable<Location>

  // POST /v1/locations
  public create(location: Location): Observable<string>

  // PUT /v1/locations/{id}
  public update(location: Location): Observable<string>
}
```

### CRAB API Integration

**Endpoint**: `https://geo.api.vlaanderen.be/geolocation/v3/Location`

**Bestand**: `src/OrganisationRegistry.UI/app/services/locations/location.service.ts:71-75`

```typescript
public getCrabLocations(searchTerm: string): Observable<any[]> {
  return this.jsonP
    .request(
      `https://geo.api.vlaanderen.be/geolocation/v3/Location?q=${searchTerm}&c=50&callback=JSONP_CALLBACK`,
      { method: 'Get' }
    )
    .map(r => r.json().LocationResult);
}
```

**Parameters**:
- `q`: Search term (address string)
- `c`: Max results (50)
- `callback`: JSONP callback naam

**Response**: Array van CRAB locations met `ID` en `FormattedAddress`.

## Overview Component (Locatie Lijst)

### Component

**Bestand**: `src/OrganisationRegistry.UI/app/administration/locations/overview/overview.component.ts`

```typescript
@Component({
  templateUrl: 'overview.template.html',
  styleUrls: ['overview.style.css']
})
export class LocationOverviewComponent implements OnInit, OnDestroy {
  public isLoading: boolean = true;
  public locations: PagedResult<LocationListItem>;

  private filter: LocationFilter = new LocationFilter();
  private currentSortBy: string = 'name';
  private currentSortOrder: SortOrder = SortOrder.Ascending;

  ngOnInit() {
    this.loadLocations();
  }

  search(event: SearchEvent<LocationFilter>) {
    this.filter = event.fields;
    this.loadLocations();
  }

  changePage(event: PagedEvent) {
    this.currentSortBy = event.sortBy;
    this.currentSortOrder = event.sortOrder;
    this.loadLocations(event);
  }

  private loadLocations(event?: PagedEvent) {
    this.isLoading = true;
    this.locationService
      .getLocations(this.filter, sortBy, sortOrder, page, pageSize)
      .finally(() => this.isLoading = false)
      .subscribe(
        newLocations => this.locations = newLocations,
        error => this.alertService.setAlert(/* error alert */)
      );
  }
}
```

### Template

**Bestand**: `src/OrganisationRegistry.UI/app/administration/locations/overview/overview.template.html`

```html
<div class="grid">
  <div class="col--1-1">
    <div class="cta-title">
      <h1 class="h2 cta-title__title">Locaties</h1>
      <a class="button cta-title__cta" [routerLink]="['./create']">
        Voeg locatie toe
      </a>
    </div>

    <ww-location-filter
      [isBusy]="isLoading"
      (search)="search($event)">
    </ww-location-filter>

    <ww-location-list
      [items]="locations"
      [isBusy]="isLoading"
      (changePage)="changePage($event)">
    </ww-location-list>
  </div>
</div>
```

**Functionaliteit**:
- **"Voeg locatie toe" knop**: Navigeert naar create route
- **Filter component**: Emits search events bij filter wijzigingen
- **List component**: Toont gepagineerde lijst met sorteer functionaliteit

## Filter Component

### Template

**Bestand**: `src/OrganisationRegistry.UI/app/administration/locations/components/filter/filter.template.html`

```html
<form class="form" [formGroup]="form" (ngSubmit)="filterForm(form.value)">
  <div class="grid">
    <div class="col--6-12">
      <ww-form-group-textbox
        [id]="'street'"
        [label]="'Straat'"
        [control]="form.get('street')"
        [placeholder]="'Straat'">
      </ww-form-group-textbox>

      <ww-form-group-textbox
        [id]="'city'"
        [label]="'Gemeente'"
        [control]="form.get('city')">
      </ww-form-group-textbox>

      <ww-form-group-toggle
        [id]="'nonCrabOnly'"
        [label]="'Enkel niet-Crab locaties'"
        [control]="form.get('nonCrabOnly')">
      </ww-form-group-toggle>
    </div>

    <div class="col--6-12">
      <ww-form-group-textbox
        [id]="'zipCode'"
        [label]="'Postcode'"
        [control]="form.get('zipCode')">
      </ww-form-group-textbox>

      <ww-form-group-textbox
        [id]="'country'"
        [label]="'Land'"
        [control]="form.get('country')">
      </ww-form-group-textbox>
    </div>

    <div class="col--1-1">
      <button type="reset" *ngIf="filterActive" (click)="resetForm()">
        Filter wissen
      </button>
      <button type="submit">Zoeken</button>
    </div>
  </div>
</form>
```

**Filter Velden**:
- **Straat**: Free text search
- **Postcode**: Free text search
- **Gemeente**: Free text search
- **Land**: Free text search
- **Enkel niet-Crab locaties**: Boolean toggle

## List Component

### Template

**Bestand**: `src/OrganisationRegistry.UI/app/administration/locations/components/list/list.template.html`

```html
<table class="data-table">
  <thead>
    <tr>
      <th>
        <a (click)="sortBy('street')">
          Straat
          <i class="sort-icon"
             [ngClass]="{'reversed': isSortedByDescending('street')}">
          </i>
        </a>
      </th>
      <th><a (click)="sortBy('zipCode')">Postcode</a></th>
      <th><a (click)="sortBy('city')">Gemeente</a></th>
      <th><a (click)="sortBy('country')">Land</a></th>
      <th><a (click)="sortBy('hasCrabLocation')">Crab Locatie</a></th>
    </tr>
  </thead>
  <tbody>
    <tr *ngIf="isBusy">
      <td colspan="5">Bezig met laden...</td>
    </tr>

    <tr *ngIf="!hasData(data)">
      <td colspan="5">Geen data beschikbaar...</td>
    </tr>

    <tr *ngFor="let location of data">
      <td>
        <a [routerLink]="['./', location.id]">{{location.street}}</a>
      </td>
      <td>{{location.zipCode}}</td>
      <td>{{location.city}}</td>
      <td>{{location.country}}</td>
      <td>
        <i *ngIf="location.hasCrabLocation"
           class="vi vi-check vi-u-badge--green">
        </i>
      </td>
    </tr>
  </tbody>
</table>

<!-- Pagination -->
<div class="pager" *ngIf="totalPages > 1">
  <strong>{{firstItem}} - {{lastItem}}</strong> van {{totalItems}}
  <a *ngIf="firstItem > 1" (click)="previousPage()">vorige</a>
  <a *ngIf="lastItem < totalItems" (click)="nextPage()">volgende</a>
</div>
```

**Functionaliteit**:
- **Sorteerbare kolommen**: Click op header sorteert data
- **Row links**: Klik op straat navigeert naar detail page
- **CRAB indicator**: Groen vinkje als location CRAB gekoppeld is
- **Paginatie**: Vorige/volgende knoppen bij multiple pages

## Detail Component (Create/Edit)

### Component Logic

**Bestand**: `src/OrganisationRegistry.UI/app/administration/locations/detail/detail.component.ts`

```typescript
@Component({
  templateUrl: 'detail.template.html',
  styleUrls: ['detail.style.css']
})
export class LocationDetailComponent implements OnInit, OnDestroy {
  public isEditMode: boolean;
  public form: FormGroup;
  public location: Location;
  public lastCrabLocation = null;  // Tracks last CRAB-selected values
  public formattedAddress: SearchResult;  // For CRAB autocomplete

  constructor(
    formBuilder: FormBuilder,
    private route: ActivatedRoute,
    private router: Router,
    private alertService: AlertService,
    private locationService: LocationService
  ) {
    // Form setup with validators
    this.form = formBuilder.group({
      id: ['', required],
      crabLocationId: [''],
      formattedAddress: [''],
      street: ['', required],
      zipCode: ['', required],
      city: ['', required],
      country: ['', required]
    });
  }

  ngOnInit() {
    this.location = new Location();  // Generates UUID
    this.form.setValue(this.location);

    this.route.params.forEach((params: Params) => {
      let id = params['id'];
      this.isEditMode = id !== null && id !== undefined;

      if (this.isEditMode) {
        // EDIT MODE: Load existing location
        this.form.disable();
        this.locationService.get(id)
          .finally(() => this.form.enable())
          .subscribe(
            item => {
              this.form.setValue(item);
              if (item.crabLocationId) {
                this.lastCrabLocation = this.form.value;
                this.formattedAddress = new SearchResult(
                  item.crabLocationId,
                  item.formattedAddress
                );
              }
            },
            error => this.alertService.setAlert(/* error */)
          );
      } else {
        // CREATE MODE: Enable form immediately
        this.form.enable();
      }
    });
  }

  // Called when CRAB autocomplete value changes
  crabValueChanged(value: string) {
    let fullAddressRegex = /(.*),([\s][0-9]{4})? (.*)/;

    if (!value) return;

    let groups = fullAddressRegex.exec(value);
    if (groups && groups.length === 4) {
      // Parse CRAB formatted address: "Street, 1000 City"
      this.form.get('street').setValue(groups[1]);
      this.form.get('zipCode').setValue(groups[2]);
      this.form.get('city').setValue(groups[3]);
      this.form.get('country').setValue('België');

      if (groups[2]) {  // Has zipcode = valid CRAB location
        this.lastCrabLocation = this.form.value;
      } else {
        // Invalid format, clear CRAB ID
        this.form.get('zipCode').markAsTouched();
        this.form.get('crabLocationId').setValue(null);
        this.lastCrabLocation = null;
      }
    } else if (value.indexOf(',') === -1) {
      // Only city, no street (incomplete)
      this.form.get('street').setValue('');
      this.form.get('street').markAsTouched();
      this.form.get('zipCode').setValue('');
      this.form.get('zipCode').markAsTouched();
      this.form.get('city').setValue(value);
      this.form.get('country').setValue('België');
      this.form.get('crabLocationId').setValue(null);
      this.lastCrabLocation = null;
    } else {
      // Reset form on parse failure
      this.form.setValue(new Location());
    }
  }

  // Called on manual input field changes
  onKey() {
    if (!this.lastCrabLocation) return;

    let location = this.form.value;

    // If user manually changes any field, clear CRAB ID
    if (location.street !== this.lastCrabLocation.street ||
        location.zipCode !== this.lastCrabLocation.zipCode ||
        location.city !== this.lastCrabLocation.city ||
        location.country !== this.lastCrabLocation.country) {
      this.form.get('crabLocationId').setValue(null);
    }
  }

  createOrUpdate(value: Location) {
    let location = new Location().withValues(value);
    if (this.isEditMode) {
      this.update(location);
    } else {
      this.create(location);
    }
  }

  private create(location: Location) {
    this.locationService.create(location)
      .finally(() => this.form.enable())
      .subscribe(
        result => {
          this.router.navigate(['./..'], { relativeTo: this.route });
          this.alertService.setAlert(
            new AlertBuilder()
              .success()
              .withTitle('Locatie aangemaakt')
              .withMessage(`Locatie "${location.name}" werd succesvol aangemaakt.`)
              .build()
          );
        },
        error => {
          this.alertService.setAlert(
            new AlertBuilder()
              .error(error)
              .withTitle('Fout bij aanmaken')
              .withMessage('Er is een fout opgetreden.')
              .build()
          );
        }
      );
  }

  private update(location: Location) {
    this.locationService.update(location)
      .finally(() => this.form.enable())
      .subscribe(
        result => {
          this.router.navigate(['./..'], { relativeTo: this.route });
          this.alertService.setAlert(/* success alert */);
        },
        error => this.alertService.setAlert(/* error alert */)
      );
  }
}
```

### Template

**Bestand**: `src/OrganisationRegistry.UI/app/administration/locations/detail/detail.template.html`

```html
<div class="grid">
  <div class="col--8-12">
    <div *ngIf="isEditMode">
      <h1 class="h2">Bewerken locatie</h1>
    </div>
    <div *ngIf="!isEditMode">
      <h1 class="h2">Nieuwe locatie</h1>
    </div>

    <form [formGroup]="form" (ngSubmit)="createOrUpdate(form.value)">
      <div class="form__group">
        <h2 class="h3">Zoek een Crab adres</h2>

        <!-- CRAB Autocomplete -->
        <ww-crab-location-autocomplete
          [id]="'crabLocation'"
          [label]="'Zoek een Crab adres'"
          [control]="form.get('crabLocationId')"
          [initialValue]="formattedAddress"
          [focus]="true"
          (valueChanged)="crabValueChanged($event)">
        </ww-crab-location-autocomplete>

        <h2 class="h3">Of, vul zelf een adres in</h2>

        <!-- Manual Address Input -->
        <ww-form-group-textbox
          [id]="'Straat'"
          [label]="'Straat'"
          [control]="form.get('street')"
          [placeholder]="'Straat'"
          (keyup)="onKey()">
        </ww-form-group-textbox>

        <ww-form-group-textbox
          [id]="'Postcode'"
          [label]="'Postcode'"
          [control]="form.get('zipCode')"
          (keyup)="onKey()">
        </ww-form-group-textbox>

        <ww-form-group-textbox
          [id]="'Gemeente'"
          [label]="'Gemeente'"
          [control]="form.get('city')"
          (keyup)="onKey()">
        </ww-form-group-textbox>

        <ww-form-group-textbox
          [id]="'Land'"
          [label]="'Land'"
          [control]="form.get('country')"
          (keyup)="onKey()">
        </ww-form-group-textbox>

        <!-- CRAB Status Indicator -->
        <div class="form__row">
          <div class="form__buttons__left" *ngIf="form.get('crabLocationId').value">
            <i class="vi vi-check vi-u-badge--green"></i>
            <span>Het gekozen adres is gekend in Crab.</span>
          </div>
          <div class="form__buttons__left" *ngIf="!form.get('crabLocationId').value">
            <i class="vi vi-info vi-u-badge--yellow"></i>
            <span>Het gekozen adres is niet gekend in Crab.</span>
          </div>
        </div>

        <!-- Form Actions -->
        <div class="form__row">
          <div class="form__buttons">
            <div class="form__buttons__left">
              <a [routerLink]="['./..']" class="cancel__button">
                <i class="vi vi-cross">Annuleer</i>
              </a>
            </div>
            <div class="form__buttons__right">
              <button class="button"
                      type="submit"
                      [disabled]="!isFormValid || form.disabled">
                <span *ngIf="!isEditMode">Voeg locatie toe</span>
                <span *ngIf="isEditMode">Wijzigingen opslaan</span>
              </button>
            </div>
          </div>
        </div>
      </div>
    </form>
  </div>
</div>
```

## CRAB Autocomplete Component

### Component

**Bestand**: `src/OrganisationRegistry.UI/app/shared/components/autocomplete/crab-location/autocomplete.component.ts`

```typescript
@Component({
  selector: 'ww-crab-location-autocomplete',
  changeDetection: ChangeDetectionStrategy.OnPush,
  templateUrl: 'autocomplete.template.html',
  styleUrls: ['autocomplete.style.css']
})
export class CrabLocationAutoComplete {
  @Input() control: FormControl;
  @Input() id: string;
  @Input() label: string;
  @Input() placeholder: string;
  @Input() name: string;
  @Input() focus: boolean;
  @Input() inline: boolean;
  @Input() initialValue: SearchResult;  // Pre-populate in edit mode

  @Output() valueChanged = new EventEmitter<any>();

  constructor(private locationService: LocationService) {}

  onValueChanged(data) {
    this.valueChanged.next(data);
  }

  searchCrabLocations(search: string): Promise<SearchResult[]> {
    return this.locationService.getCrabLocations(search)
      .map(pagedResult => pagedResult.map(x =>
        new SearchResult(x.ID, x.FormattedAddress)
      ))
      .toPromise()
      .catch(this.handleError);
  }

  private handleError(error: any): any {
    console.error('An error occurred', error);
    return new SearchResult(
      '',
      'Er heeft zich een fout voorgedaan bij het ophalen van de gegevens.'
    );
  }
}
```

**Functionaliteit**:
- **Autocomplete**: Type-ahead search via CRAB API
- **Debouncing**: Voorkomt excessive API calls
- **Error handling**: Toont gebruiksvriendelijke error message
- **Change detection**: OnPush strategie voor performance

## User Flow

### Create Flow

```
1. User navigates to /administration/locations
   ↓
2. Click "Voeg locatie toe"
   ↓
3. Navigate to /administration/locations/create
   ↓
4. LocationDetailComponent initializes
   - New Location() creates UUID
   - Form enabled
   - isEditMode = false
   ↓
5. User has two options:

   A) CRAB Search Path:
      ↓
      5a. Type in CRAB autocomplete
      ↓
      5b. JSONP request to geo.api.vlaanderen.be
      ↓
      5c. Select CRAB result
      ↓
      5d. crabValueChanged() parses formatted address
      ↓
      5e. Form fields auto-populated:
          - street = "Kunstlaan 16"
          - zipCode = "1000"
          - city = "Brussel"
          - country = "België"
          - crabLocationId = "123456"
      ↓
      5f. Green badge: "Het gekozen adres is gekend in Crab"

   B) Manual Input Path:
      ↓
      5a. User types in street, zipCode, city, country fields
      ↓
      5b. onKey() clears crabLocationId (if any)
      ↓
      5c. Yellow badge: "Het gekozen adres is niet gekend in Crab"
   ↓
6. User clicks "Voeg locatie toe"
   ↓
7. Form validation:
   - street: required
   - zipCode: required
   - city: required
   - country: required
   ↓
8. POST /v1/locations with JSON payload
   ↓
9. Backend validates and creates location
   ↓
10. Navigate back to /administration/locations
    ↓
11. Success alert shown with link to created location
```

### Edit Flow

```
1. User navigates to /administration/locations
   ↓
2. Click on location street in table
   ↓
3. Navigate to /administration/locations/{id}
   ↓
4. LocationDetailComponent initializes
   - isEditMode = true
   - Form disabled
   ↓
5. GET /v1/locations/{id}
   ↓
6. Form populated with existing data
   - If crabLocationId exists: green badge + autocomplete pre-filled
   - If no crabLocationId: yellow badge
   ↓
7. Form enabled
   ↓
8. User modifies fields
   - CRAB search: replaces all address fields
   - Manual edit: clears crabLocationId via onKey()
   ↓
9. User clicks "Wijzigingen opslaan"
   ↓
10. Form validation
    ↓
11. PUT /v1/locations/{id} with JSON payload
    ↓
12. Backend validates and updates location
    ↓
13. Navigate back to /administration/locations
    ↓
14. Success alert shown
```

## CRAB Integration Details

### CRAB Address Format

CRAB API returns addresses in format:
```
"Kunstlaan 16, 1000 Brussel"
```

### Parsing Logic

**Regex**: `/(.*),([\s][0-9]{4})? (.*)/`

**Capture Groups**:
1. `(.*)` = Street (e.g., "Kunstlaan 16")
2. `([\s][0-9]{4})?` = ZipCode (e.g., " 1000") - optional space + 4 digits
3. `(.*)` = City (e.g., "Brussel")

**Example Parsing**:
```typescript
Input:  "Kunstlaan 16, 1000 Brussel"
Groups: ["Kunstlaan 16, 1000 Brussel", "Kunstlaan 16", " 1000", "Brussel"]

Result:
  street  = "Kunstlaan 16"
  zipCode = " 1000"
  city    = "Brussel"
  country = "België"  (hardcoded)
```

### CRAB Status Tracking

```typescript
// lastCrabLocation tracks the last CRAB-selected values
public lastCrabLocation = null;

// When CRAB address selected:
if (groups[2]) {  // Has zipcode = valid
  this.lastCrabLocation = this.form.value;
}

// On manual edit:
onKey() {
  if (!this.lastCrabLocation) return;

  // If ANY field changed from CRAB value:
  if (location.street !== this.lastCrabLocation.street ||
      location.zipCode !== this.lastCrabLocation.zipCode ||
      location.city !== this.lastCrabLocation.city ||
      location.country !== this.lastCrabLocation.country) {

    // Clear CRAB ID → Yellow badge
    this.form.get('crabLocationId').setValue(null);
  }
}
```

**Logic**:
- **CRAB selected**: Green badge, `crabLocationId` set
- **Manual edit after CRAB**: Yellow badge, `crabLocationId` cleared
- **Pure manual**: Yellow badge, no `crabLocationId`

## Validation

### Client-Side Validation

**Required Fields**:
```typescript
this.form = formBuilder.group({
  id: ['', required],           // Auto-generated UUID
  crabLocationId: [''],         // Optional
  formattedAddress: [''],       // Optional, computed
  street: ['', required],       // Required
  zipCode: ['', required],      // Required
  city: ['', required],         // Required
  country: ['', required]       // Required
});
```

**Form State**:
```typescript
get isFormValid() {
  return this.form.enabled && this.form.valid;
}
```

**Submit Button**:
```html
<button type="submit"
        [disabled]="!isFormValid || form.disabled">
  Voeg locatie toe
</button>
```

### Server-Side Validation

Na submit valideert de backend:
- **Uniciteit**: FormattedAddress mag niet al bestaan
- **Length constraints**: Street max 200, ZipCode max 50, etc.
- **Required fields**: Alle velden behalve crabLocationId

Zie `LOCATION_REGISTRATIE.md` voor backend validatie details.

## Error Handling

### Network Errors

```typescript
this.locationService.create(location)
  .subscribe(
    result => { /* success */ },
    error => {
      this.alertService.setAlert(
        new AlertBuilder()
          .error(error)
          .withTitle('Fout bij aanmaken')
          .withMessage('Er is een fout opgetreden bij het opslaan.')
          .build()
      );
    }
  );
```

### CRAB API Errors

```typescript
searchCrabLocations(search: string): Promise<SearchResult[]> {
  return this.locationService.getCrabLocations(search)
    .map(/* ... */)
    .toPromise()
    .catch(error => {
      console.error('An error occurred', error);
      return new SearchResult(
        '',
        'Er heeft zich een fout voorgedaan bij het ophalen van de gegevens.'
      );
    });
}
```

### Validation Errors

Backend validation errors (400 Bad Request) worden getoond via alerts:

```typescript
{
  "message": "Street is required.",
  "errors": [
    { "field": "street", "message": "Street is required." }
  ]
}
```

Alert toont error message aan gebruiker.

## State Management

### Component State

```typescript
public isEditMode: boolean;           // Create vs Edit mode
public form: FormGroup;               // Reactive form state
public location: Location;            // Domain model
public lastCrabLocation = null;       // CRAB tracking
public formattedAddress: SearchResult; // Pre-populate autocomplete
public isLoading: boolean;            // Loading indicator
```

### Form Lifecycle

```
1. Initialize: new Location() → UUID generated
   ↓
2. Set Form Values: form.setValue(location)
   ↓
3. Edit Mode: GET location → form.setValue(item)
   ↓
4. User Input: form control values change
   ↓
5. Submit: form.value → Location model
   ↓
6. API Call: POST/PUT with Location JSON
   ↓
7. Navigate: Router.navigate(['./..'])
   ↓
8. Alert: Success/Error message
```

### Subscription Management

```typescript
private readonly subscriptions: Subscription[] = [];

ngOnInit() {
  this.subscriptions.push(
    this.locationService.get(id).subscribe(/* ... */)
  );
}

ngOnDestroy() {
  this.subscriptions.forEach(sub => sub.unsubscribe());
}
```

**Best Practice**: Alle subscriptions worden opgeslagen en unsubscribed in `ngOnDestroy` om memory leaks te voorkomen.

## UI Components

### Custom Form Components

De applicatie gebruikt custom form components met `ww-` prefix:

#### ww-form-group-textbox
```html
<ww-form-group-textbox
  [id]="'street'"
  [label]="'Straat'"
  [control]="form.get('street')"
  [placeholder]="'Straat'"
  [name]="'Straat'">
</ww-form-group-textbox>
```

**Features**:
- Label + input field
- Validation state display
- Error messages
- Required indicator

#### ww-form-group-toggle
```html
<ww-form-group-toggle
  [id]="'nonCrabOnly'"
  [label]="'Enkel niet-Crab locaties'"
  [control]="form.get('nonCrabOnly')">
</ww-form-group-toggle>
```

**Features**:
- Checkbox/toggle switch
- Boolean value binding

#### ww-crab-location-autocomplete
```html
<ww-crab-location-autocomplete
  [id]="'crabLocation'"
  [label]="'Zoek een Crab adres'"
  [control]="form.get('crabLocationId')"
  [initialValue]="formattedAddress"
  [focus]="true"
  (valueChanged)="crabValueChanged($event)">
</ww-crab-location-autocomplete>
```

**Features**:
- Type-ahead search
- CRAB API integration
- Formatted address display
- Pre-populate in edit mode

## Performance Optimizations

### Change Detection Strategy

```typescript
@Component({
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class CrabLocationAutoComplete { }
```

OnPush strategie reduceert change detection cycles.

### Reactive Forms

Reactive forms zijn efficiënter dan template-driven forms:
- Form state is immutable
- Value changes zijn Observable streams
- Makkelijker te testen

### JSONP voor CRAB API

JSONP wordt gebruikt voor cross-origin requests naar CRAB API:
```typescript
this.jsonP.request(url, { method: 'Get' })
```

Alternatief voor CORS-restricted API calls.

### Lazy Loading

De locations module is lazy loaded via routing:
```typescript
{
  path: 'administration',
  loadChildren: () => import('./administration/administration.module')
    .then(m => m.AdministrationModule)
}
```

Reduceert initial bundle size.

## Testing Strategies

### Unit Tests

Test component logic in isolatie:

```typescript
describe('LocationDetailComponent', () => {
  it('should generate UUID on initialization', () => {
    const component = new LocationDetailComponent(/* deps */);
    component.ngOnInit();
    expect(component.location.id).toMatch(/^[a-f0-9-]{36}$/);
  });

  it('should clear crabLocationId on manual edit', () => {
    component.form.patchValue({
      street: 'Kunstlaan 16',
      crabLocationId: '123'
    });
    component.lastCrabLocation = component.form.value;

    component.form.get('street').setValue('Wetstraat 18');
    component.onKey();

    expect(component.form.get('crabLocationId').value).toBeNull();
  });
});
```

### Integration Tests

Test component + service interactie:

```typescript
describe('LocationDetailComponent Integration', () => {
  it('should create location via API', fakeAsync(() => {
    const spy = spyOn(locationService, 'create').and.returnValue(
      Observable.of('/locations/123')
    );

    component.form.patchValue({
      street: 'Test',
      zipCode: '1000',
      city: 'Brussels',
      country: 'Belgium'
    });
    component.createOrUpdate(component.form.value);
    tick();

    expect(spy).toHaveBeenCalled();
    expect(router.navigate).toHaveBeenCalledWith(['./..']);
  }));
});
```

### E2E Tests

Test volledige user flows:

```typescript
describe('Location Registration E2E', () => {
  it('should create location with CRAB search', () => {
    browser.get('/administration/locations/create');

    // Type in CRAB autocomplete
    element(by.id('crabLocation')).sendKeys('Kunstlaan 16');

    // Select first result
    element(by.css('.autocomplete-result:first-child')).click();

    // Verify fields populated
    expect(element(by.id('Straat')).getAttribute('value')).toBe('Kunstlaan 16');
    expect(element(by.id('Postcode')).getAttribute('value')).toBe('1000');

    // Submit
    element(by.css('button[type="submit"]')).click();

    // Verify redirect
    expect(browser.getCurrentUrl()).toContain('/administration/locations');

    // Verify alert
    expect(element(by.css('.alert--success')).getText())
      .toContain('Locatie aangemaakt');
  });
});
```

## Best Practices

### 1. Gebruik Reactive Forms

```typescript
// ✅ Good: Reactive form met FormBuilder
this.form = formBuilder.group({
  street: ['', required],
  zipCode: ['', required]
});

// ❌ Bad: Template-driven form
<input [(ngModel)]="location.street" required>
```

### 2. Unsubscribe bij OnDestroy

```typescript
// ✅ Good: Track subscriptions
private subscriptions: Subscription[] = [];

ngOnInit() {
  this.subscriptions.push(
    this.service.getData().subscribe(/* */)
  );
}

ngOnDestroy() {
  this.subscriptions.forEach(sub => sub.unsubscribe());
}

// ❌ Bad: Subscription leak
ngOnInit() {
  this.service.getData().subscribe(/* */);  // Never unsubscribed!
}
```

### 3. CRAB ID Clearing

```typescript
// ✅ Good: Clear CRAB ID on manual edit
onKey() {
  if (!this.lastCrabLocation) return;

  if (/* values changed */) {
    this.form.get('crabLocationId').setValue(null);
  }
}

// ❌ Bad: Keep CRAB ID na manual edit
// User thinks it's still a CRAB location, but it's modified!
```

### 4. UUID Generatie Client-Side

```typescript
// ✅ Good: Generate UUID in constructor
constructor() {
  this.id = UUID.UUID();
}

// ❌ Bad: Generate UUID in ngOnInit
// ID might change unexpectedly
```

### 5. Form Disable tijdens API Calls

```typescript
// ✅ Good: Disable form during submission
this.form.disable();
this.service.create(location)
  .finally(() => this.form.enable())
  .subscribe(/* */);

// ❌ Bad: Allow multiple submissions
// User can click submit multiple times!
```

## Veelvoorkomende Problemen

### Probleem 1: CRAB ID niet gewist na edit

**Symptoom**: Gebruiker selecteert CRAB adres, wijzigt straat manueel, maar groene badge blijft staan.

**Oorzaak**: `onKey()` methode niet aangeroepen op input velden.

**Oplossing**:
```html
<ww-form-group-textbox
  [control]="form.get('street')"
  (keyup)="onKey()">  <!-- Important! -->
</ww-form-group-textbox>
```

### Probleem 2: Form submission werkt niet

**Symptoom**: Submit knop is enabled maar er gebeurt niets bij klik.

**Mogelijke oorzaken**:
1. Form validation faalt
2. `(ngSubmit)="createOrUpdate(form.value)"` ontbreekt
3. Backend geeft error maar error handling ontbreekt

**Debug**:
```typescript
createOrUpdate(value: Location) {
  console.log('Form valid:', this.form.valid);
  console.log('Form value:', value);
  // ...
}
```

### Probleem 3: CRAB autocomplete geeft geen resultaten

**Symptoom**: Type in CRAB search, maar geen suggestions.

**Mogelijke oorzaken**:
1. CRAB API down
2. JSONP blocked door browser
3. Search term te kort (minimum 3 karakters aanbevolen)

**Debug**:
```typescript
searchCrabLocations(search: string): Promise<SearchResult[]> {
  console.log('Searching CRAB for:', search);
  return this.locationService.getCrabLocations(search)
    .map(result => {
      console.log('CRAB result:', result);
      return result.map(x => new SearchResult(x.ID, x.FormattedAddress));
    })
    .toPromise();
}
```

### Probleem 4: Memory leak

**Symptoom**: Applicatie wordt langzamer na meerdere navigaties.

**Oorzaak**: Subscriptions niet unsubscribed.

**Oplossing**: Zie "Subscription Management" hierboven.

## Configuratie

### API Base URL

**Bestand**: Configuration service injects API URL

```typescript
private locationsUrl = `${this.configurationService.apiUrl}/v1/locations`;
```

API base URL wordt geconfigureerd in environment files:

```typescript
// environment.ts (development)
export const environment = {
  apiUrl: 'https://localhost:5001/api'
};

// environment.prod.ts (production)
export const environment = {
  apiUrl: 'https://api.wegwijs.vlaanderen.be'
};
```

### Page Size

Default page size wordt geconfigureerd in ConfigurationService:

```typescript
public defaultPageSize = 10;
public boundedPageSize = 50;  // For search
```

## Conclusie

De front-end location registratie biedt een gebruiksvriendelijke interface met:

1. **Dual Input Method**: CRAB autocomplete of manuele invoer
2. **Real-time Validation**: Reactive forms met instant feedback
3. **CRAB Status Tracking**: Visuele indicatoren voor CRAB koppeling
4. **Smart Clearing**: Automatisch CRAB ID wissen bij manual edits
5. **Responsive Design**: Grid-based layout voor alle schermformaten
6. **Role-Based Access**: Authorization via RoleGuard
7. **Error Handling**: User-friendly error messages
8. **Performance**: OnPush change detection, lazy loading

De implementatie volgt Angular best practices en biedt een solide basis voor location management in de Organisatie Registry.
