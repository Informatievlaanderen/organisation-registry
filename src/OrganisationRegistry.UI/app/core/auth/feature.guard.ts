import {Injectable} from "@angular/core";
import {ActivatedRouteSnapshot, CanActivate, CanActivateChild, RouterStateSnapshot} from "@angular/router";
import {FeaturesService} from "services/features";
import {Observable} from "rxjs/Observable";

@Injectable()
export class FeatureGuard implements CanActivate, CanActivateChild {
    constructor(
        private featuresService: FeaturesService,
    ) {
    }

    public canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<boolean> {
      return this.isFeatureEnabled(route);
    }

    public canActivateChild(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<boolean> {
        return this.isFeatureEnabled(route);
    }

    private isFeatureEnabled(route: ActivatedRouteSnapshot): Observable<boolean> {
        let desiredFeatures = route.data['features'] as Array<string>;

        return this
            .featuresService
            .getAllFeatures()
            .map(toggle =>
                desiredFeatures.every(dt => toggle[dt]));
    }
}
