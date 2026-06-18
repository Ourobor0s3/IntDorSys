import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

import { lastValueFrom } from 'rxjs';
import { IResponse } from '../interface/response';
import { environment } from '../../../environments/environment';

const apiUrl = environment.apiUrl;

@Injectable({
    providedIn: 'root',
})

export class ApiService {
    constructor(private http: HttpClient) {
    }

    getAccessToken(): string | null {
        try {
            return localStorage.getItem('accessToken');
        } catch {
            return null;
        }
    }

    getAnonym<T>(url: string): Promise<IResponse<T>> {
        return lastValueFrom(this.http.get<IResponse<T>>(apiUrl + url));
    }

    get<T>(url: string): Promise<IResponse<T>> {
        return lastValueFrom(this.http.get<IResponse<T>>(apiUrl + url, { headers: this.getAuthorizationHeaders() }));
    }

    getForPreview(url: string): Promise<string> {
        return lastValueFrom(this.http.get(apiUrl + url, { headers: this.getAuthorizationHeaders(), responseType: 'text' }));
    }

    delete<T>(url: string): Promise<IResponse<T>> {
        return lastValueFrom(this.http.delete<IResponse<T>>(apiUrl + url, { headers: this.getAuthorizationHeaders() }));
    }

    postAnonym<T>(url: string, body: unknown): Promise<IResponse<T>> {
        return lastValueFrom(this.http.post<IResponse<T>>(apiUrl + url, body, { headers: this.getPublicHeaders() }));
    }

    post<T>(url: string, body: unknown): Promise<IResponse<T>> {
        return lastValueFrom(this.http.post<IResponse<T>>(apiUrl + url, body, { headers: this.getAuthorizationHeaders() }));
    }

    putAnonym<T>(url: string, body: unknown): Promise<IResponse<T>> {
        return lastValueFrom(this.http.put<IResponse<T>>(apiUrl + url, body));
    }

    put<T>(url: string, body: unknown): Promise<IResponse<T>> {
        return lastValueFrom(this.http.put<IResponse<T>>(apiUrl + url, body, { headers: this.getAuthorizationHeaders() }));
    }

    patch<T>(url: string, body: unknown = {}): Promise<IResponse<T>> {
        return lastValueFrom(this.http.patch<IResponse<T>>(apiUrl + url, body, { headers: this.getAuthorizationHeaders() }));
    }

    upload<T>(url: string, body: unknown): Promise<IResponse<T>> {
        return lastValueFrom(this.http.post<IResponse<T>>(apiUrl + url, body, { headers: this.getAuthorizationHeaders(true) }));
    }

    uploadImage<T>(url: string, body: Record<string, unknown> | null, image: File): Promise<IResponse<T>> {
        if (!!body) {
            body.file = image;
        }

        const formData: FormData = new FormData();
        formData.append('file', image);

        return lastValueFrom(this.http.put<IResponse<T>>(apiUrl + url, formData, { params: body as never, headers: this.getAuthorizationHeaders(true) }));
    }

    uploadAvatar<T>(url: string, body: Blob): Promise<IResponse<T>> {
        const formData = new FormData();

        formData.append('file', body);
        return lastValueFrom(this.http.put<IResponse<T>>(apiUrl + url, formData, { headers: this.getAuthorizationHeaders(true) }));
    }

    private getPublicHeaders(isFile: boolean = false) {
        const defaultHeaders = {
            'Content-Type': 'application/json',
            Localization: localStorage.getItem('localization') ?? 'en',
        };

        const fileHeaders = {
            Localization: localStorage.getItem('localization') ?? 'en',
        };

        return isFile ? fileHeaders : defaultHeaders;
    }

    private getAuthorizationHeaders(isFile: boolean = false) {
        return {
            Authorization: `Bearer ${this.getAccessToken()}`,
            ...this.getPublicHeaders(isFile),
        };
    }
}
