import { Injectable } from '@angular/core';
import { Router, CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot, NavigationExtras } from '@angular/router';
import { Observable } from 'rxjs/Observable';
import { AdalService } from './adal.service';

@Injectable()
export class EnsureAuthenticatedGuard implements CanActivate {

  constructor(private router: Router, private adalService: AdalService) {
  }

  canActivate(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot): Observable<boolean> | Promise<boolean> | boolean {

        this.adalService.handleCallback();
        
        let navigationExtras: NavigationExtras = {
            queryParams: { 'returnUrl': route.url }
        };

        const user = this.adalService.userInfo;
        console.log(`Enusre authenticated guard Logging to ${route.url} ${user}`);
        if(!user) {
            this.router.navigate(['login'], navigationExtras);
        }
        return true;  
  }
}
