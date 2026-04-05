import { environment } from "../../../environments/environment";
import { Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { ApiService } from "./api.service";

export interface Credentials {
    email: string;
    password: string;
}

const tokenUrl = "token";
const apiUrl = environment.apiUrl;

@Injectable({
    providedIn: 'root',
})
export class TokenService {
    private requestHeader = {
        headers: {
            'Content-Type': 'application/x-www-form-urlencoded',
            Localization: 'RU',
        },
        //      new HttpHeaders().set('Content-Type', 'application/x-www-form-urlencoded')
        // .set("Localization","RU")
    };

    constructor(
        private http: HttpClient,
        private api: ApiService,
    ) {
    }

    // old method
    private static encodeToFormdata(requestData) {
        return Object.keys(requestData)
            .map((key) => {
                return encodeURIComponent(key) + '=' + encodeURIComponent(requestData[key]);
            })
            .join('&');
    }

    auth(credentials: Credentials) {
        const requestData = {
            login: credentials.email,
            password: credentials.password,
        };
        return this.api.postAnonym(tokenUrl, requestData);
    }

    // old method
    createToken(credentials: Credentials) {
        const requestData = {
            // ...oauthClientParams,
            grant_type: 'password',
            username: credentials.email,
            password: credentials.password,
        };
        const requestBody = TokenService.encodeToFormdata(requestData);
        return this.api.postAnonym(tokenUrl, requestBody);
    }

    // old method
    refreshToken(refreshToken: string) {
        const requestData = {
            // ...oauthClientParams,
            grant_type: 'refresh_token',
            refresh_token: refreshToken,
        };
        const requestBody = TokenService.encodeToFormdata(requestData);
        return this.http.post(apiUrl + tokenUrl, requestBody, this.requestHeader);
    }
}
