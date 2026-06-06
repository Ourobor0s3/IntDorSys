import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, CanActivate, Router, RouterStateSnapshot } from '@angular/router';
import { AuthService } from '../services/auth.service';
import { Observable } from "rxjs";

@Injectable({
    providedIn: 'root',
})
export class AuthGuard implements CanActivate {

    constructor(private authService: AuthService, private router: Router) {
    }

    canActivate(
        next: ActivatedRouteSnapshot,
        state: RouterStateSnapshot,
    ): Observable<boolean> | Promise<boolean> | boolean {
        let t = this;
        if (!this.authService.isLoggedIn()) {
            return t.goAway();
        }
        return true;
    }

    goAway(redirectPath: string = ''): boolean {
        if (redirectPath != '')
            this.router.navigateByUrl(redirectPath);
        else
            this.authService.logout();
        return false;
    }
}
