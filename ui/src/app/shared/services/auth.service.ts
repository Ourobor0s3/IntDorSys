import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { authRoute, loginRoute } from "../constants/routes";
import { Credentials, TokenService } from "./token.service";
import { EventService } from "./event.service";
import { UserService } from "./user.service";
import { tap } from "rxjs";

export interface User {
    uid: string;
    email: string;
}

interface AuthData {
    accessToken: string;
    refreshToken: string;
    //expiresAt: Date;
}

@Injectable({
    providedIn: 'root',
})
export class AuthService {
    public userData: any;
    public showLoader: boolean = false;

    constructor(
        private tokenService: TokenService,
        private router: Router,
        private eventService: EventService,
        private userService: UserService,
    ) {
    }

    private static _authData: any;

    get authData() {
        if (!AuthService._authData && localStorage.getItem('auth')) {
            AuthService._authData = JSON.parse(localStorage.getItem('auth'));
        }

        let cookAuth = !!localStorage.getItem('auth')
            ? (JSON.parse(localStorage.getItem('auth')) as AuthData)
            : null;
        if (!!AuthService._authData &&
            (!cookAuth ||
                AuthService._authData.refreshToken != cookAuth.refreshToken ||
                AuthService._authData.accessToken != cookAuth.accessToken)
        ) {
            this.logout();
            // return;
        }
        return AuthService._authData;
    }

    set authData(newAuth: AuthData) {
        if (!newAuth) {
            localStorage.removeItem('auth');
        } else {
            localStorage.setItem('auth', JSON.stringify(newAuth));
        }
        AuthService._authData = newAuth;
    }

    login(loginCred: Credentials) {
        return this.tokenService
            .auth(loginCred).pipe(
                tap((data) => {
                    const accessToken = (data as any)?.data?.accessToken;
                    const refreshToken = (data as any)?.data?.refreshToken ?? "";
                    if (!accessToken) {
                        console.error('Access token not found in response');
                        return;
                    }
                    localStorage.setItem('accessToken', accessToken);
                    const tokenObj: AuthData = {
                        accessToken: accessToken,
                        refreshToken: refreshToken,
                        //expiresAt
                    };
                    this.authData = tokenObj;
                }),
            )
            .toPromise();
    }

    logout(): void {
        this.router.routeReuseStrategy.shouldReuseRoute = function () {
            return false;
        };

        window.stop();
        this.showLoader = false;
        this.authData = undefined;
        this.userService.clear();
        this.eventService.logout();
        localStorage.removeItem('auth');
        localStorage.removeItem('accessToken');
        localStorage.removeItem('localization')
        //localStorage.clear(); //сбрасывает язык в localStorage (по дефолту 'en')
        this.router.navigate(['/' + authRoute + '/' + loginRoute]);
    }

    isLoggedIn(): boolean {
        return !!this.authData;
    }
}
