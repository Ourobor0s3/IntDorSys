import { Injectable } from "@angular/core";
import { ApiService } from "./api.service";

export interface Credentials {
    login: string;
    password: string;
}

const tokenUrl = "token";

@Injectable({
    providedIn: 'root',
})
export class TokenService {

    constructor(private api: ApiService) {}

    auth(credentials: Credentials) {
        return this.api.postAnonym(tokenUrl, {
            login: credentials.login,
            password: credentials.password,
        });
    }
}
