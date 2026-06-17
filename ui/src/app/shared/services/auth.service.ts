import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { authRoute, loginRoute } from "../constants/routes";
import { Credentials, TokenService } from "./token.service";
import { EventService } from "./event.service";
import { UserService } from "./user.service";
import { UserInfoModel } from "../interface/userInfo.model";
import { lastValueFrom, tap } from "rxjs";
import { IResponse } from '../interface/response';

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

    login(loginCred: Credentials): Promise<IResponse<AuthData>> {
        return lastValueFrom(
            this.tokenService.auth<AuthData>(loginCred).pipe(
                tap((data: IResponse<AuthData>) => {
                    const accessToken = data?.data?.accessToken;
                    const refreshToken = data?.data?.refreshToken ?? "";
                    const role = data?.data?.role ?? "";
                    if (!accessToken) {
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
            ),
        );
    }

    isAdmin(): boolean {
        return this.authData?.role === 'admin';
    }

    logout(): void {
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
