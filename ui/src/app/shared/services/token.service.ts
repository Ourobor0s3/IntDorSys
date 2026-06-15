import { Injectable } from "@angular/core";
import { ApiService } from "./api.service";
import { Observable } from "rxjs";
import { IResponse } from "../interface/response";

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

    auth<T = unknown>(credentials: Credentials): Observable<IResponse<T>> {
        return this.api.postAnonym<T>(tokenUrl, {
            login: credentials.login,
            password: credentials.password,
        });
    }
}
