import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

import { Observable } from 'rxjs';
import { IResponse } from '../interface/responce';
import { environment } from '../../../environments/environment';

const apiUrl = environment.apiUrl;

@Injectable({
    providedIn: 'root',
})

export class ApiService {
    constructor(private http: HttpClient) {
    }

    async getAccessToken() {
        let t = this;
        // потом поменять
        let accessToken = localStorage.getItem('accessToken');
        return accessToken;
    }

    getAnonym<T>(url: string): Observable<IResponse<T>> {
        return this.http.get<IResponse<T>>(apiUrl + url);
    }

    async get<T>(url: string): Promise<Observable<IResponse<T>>> {
        const headers = await this.getAuthorizationHeaders();
        return this.http.get<IResponse<T>>(apiUrl + url, { headers });
    }

    async getForPreview(url: string): Promise<Observable<any>> {
        const headers = await this.getAuthorizationHeaders();
        return this.http.get(apiUrl + url, { headers, responseType: 'text' });
    }

    async delete<T>(url: string): Promise<Observable<IResponse<T>>> {
        const headers = await this.getAuthorizationHeaders();
        return this.http.delete<IResponse<T>>(apiUrl + url, { headers });
    }

    postAnonym<T>(url: string, body: any): Observable<IResponse<T>> {
        return this.http.post<IResponse<T>>(apiUrl + url, body, { headers: this.getPublicHeaders() });
    }

    async post<T>(url: string, body: any): Promise<Observable<IResponse<T>>> {
        const headers = await this.getAuthorizationHeaders();
        return this.http.post<IResponse<T>>(apiUrl + url, body, { headers });
    }

    putAnonym<T>(url: string, body: any): Observable<IResponse<T>> {
        return this.http.put<IResponse<T>>(apiUrl + url, body);
    }

    async put<T>(url: string, body: any): Promise<Observable<IResponse<T>>> {
        const headers = await this.getAuthorizationHeaders();
        return this.http.put<IResponse<T>>(apiUrl + url, body, { headers });
    }

    async patch<T>(url: string, body: any = {}): Promise<Observable<IResponse<T>>> {
        const headers = await this.getAuthorizationHeaders();
        return this.http.patch<IResponse<T>>(apiUrl + url, body, { headers });
    }

    async upload<T>(url: string, body: any): Promise<Observable<IResponse<T>>> {
        const headers = await this.getAuthorizationHeaders(true);
        return this.http.post<IResponse<T>>(apiUrl + url, body, { headers });
    }

    async uploadImage<T>(url: string, body: any, image: any) {
        if (!!body) {
            body.file = image;
        }

        const formData: FormData = new FormData();
        formData.append('file', image);

        const headers = await this.getAuthorizationHeaders(true);
        return this.http.put<IResponse<T>>(apiUrl + url, formData, { params: body, headers });
    }

    async uploadAvatar<T>(url: string, body: any): Promise<Observable<IResponse<T>>> {
        const formData = new FormData();

        formData.append('file', body);
        const headers = await this.getAuthorizationHeaders(true);
        return this.http.put<IResponse<T>>(apiUrl + url, formData, { headers });
    }

    private getPublicHeaders(isFile: boolean = false) {
        // var confirmCode = localStorage.getItem('confirmCode');
        // if (confirmCode) localStorage.removeItem('confirmCode');

        var defaultHeaders = {
            'Content-Type': 'application/json',
            // 'Confirmation-Code': confirmCode ?? 'not_set',
            Localization: localStorage.getItem('localization') ?? 'en',
        };

        var fileHeaders = {
            Localization: localStorage.getItem('localization') ?? 'en',
        };

        return isFile ? fileHeaders : defaultHeaders;
    }

    private async getAuthorizationHeaders(isFile: boolean = false) {
        return {
            Authorization: `Bearer ${await this.getAccessToken()}`,
            ...this.getPublicHeaders(isFile),
        };
    }
}
