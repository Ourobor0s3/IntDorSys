import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { authRoute, loginRoute } from "../constants/routes";
import { Credentials, TokenService } from "./token.service";
import { EventService } from "./event.service";
import { UserService } from "./user.service";
import { UserInfoModel } from "../model/userInfo.model";
import { tap } from "rxjs";

export interface User {
    uid: string;
    email: string;
}

interface AuthData {
    accessToken: string;
    refreshToken: string;
    role?: string;
}

@Injectable({
    providedIn: 'root',
})
export class AuthService {
    public userData: UserInfoModel | null;
    public showLoader: boolean = false;

    constructor(
        private tokenService: TokenService,
        private router: Router,
        private eventService: EventService,
        private userService: UserService,
    ) {
    }

    private static _authData: AuthData | null;

    get authData() {
        if (!AuthService._authData) {
            const raw = localStorage.getItem('auth');
            if (raw) {
                AuthService._authData = JSON.parse(raw) as AuthData;
            }
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
                    const role = (data as any)?.data?.role ?? "";
                    if (!accessToken) {
                        console.error('Access token not found in response');
                        return;
                    }
                    localStorage.setItem('accessToken', accessToken);
                    const tokenObj: AuthData = {
                        accessToken: accessToken,
                        refreshToken: refreshToken,
                        role: role,
                    };
                    this.authData = tokenObj;
                }),
            )
            .toPromise();
    }

    isAdmin(): boolean {
        return this.authData?.role === 'admin';
    }

    logout(): void {
        this.router.routeReuseStrategy.shouldReuseRoute = function () {
            return false;
        };

        this.showLoader = false;
        this.authData = undefined;
        this.userService.clear();
        this.eventService.logout();
        localStorage.removeItem('auth');
        localStorage.removeItem('accessToken');
        localStorage.removeItem('localization');
        this.router.navigate(['/' + authRoute + '/' + loginRoute]);
    }

    getAccessToken(): string | null {
        return localStorage.getItem('accessToken');
    }

    isLoggedIn(): boolean {
        return !!this.authData;
    }
}
