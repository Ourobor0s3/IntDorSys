import { Injectable } from '@angular/core';
import { HttpEvent, HttpHandler, HttpInterceptor, HttpRequest } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { Router } from '@angular/router';
import { AuthService } from './auth.service';

@Injectable()
export class AuthInterceptor implements HttpInterceptor {
    constructor(
        private authService: AuthService,
        private router: Router,
    ) {
    }

    intercept(req: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
        let authReq = req;

        const token = this.authService.getAccessToken();
        if (token) {
            authReq = req.clone({
                setHeaders: { Authorization: `Bearer ${token}` },
            });
        }

        return next.handle(authReq).pipe(
            catchError(error => {
                if (error.status === 401) {
                    this.authService.logout();
                    this.router.navigate(['/auth']);
                }
                return throwError(() => error);
            }),
        );
    }
}
