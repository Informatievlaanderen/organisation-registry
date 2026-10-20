# API Contract: Functietype verwijderen

## DELETE /v1/functiontypes/{id}

Spiegel van `DELETE /v1/capacities/{id}`. Het routeprefix volgt `FunctionTypeCommandController`.

- **Auth**: `Permission.ParametersFunctionTypesDelete`
- **Responses**: zelfde gedrag als de capacity-delete (204 bij succes, ook als het type al verwijderd was; 401/403; 404 bij een onbekend id)

## GET /v1/functiontypes (gewijzigd)

- `x-filtering` krijgt `showAll` (bool, standaard `false`), zoals `CapacityListFilter`.
- Standaard komen alleen functietypes met `isRemoved == false` terug.
- Elk item krijgt een veld `isRemoved`.

## Ongewijzigd

Endpoints die een functietype koppelen (organisatie-functie, organisatie-hoedanigheid, orgaan-mandaat) weigeren een verwijderd type **niet**. Dat is hetzelfde gedrag als bij hoedanigheden.
