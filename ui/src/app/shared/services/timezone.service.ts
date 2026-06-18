import { Injectable } from '@angular/core';
import { ApiService } from './api.service';
import { BehaviorSubject } from 'rxjs';

@Injectable({
    providedIn: 'root',
})
export class TimezoneService {
    private timezone = '+03:00';
    private timezoneSubject = new BehaviorSubject<string>(this.timezone);
    readonly timezone$ = this.timezoneSubject.asObservable();

    constructor(private api: ApiService) {}

    get current(): string {
        return this.timezone;
    }

    getOffsetMs(): number {
        const match = this.timezone.match(/([+-]\d{2}):(\d{2})/);
        if (!match) return 0;
        const hours = parseInt(match[1], 10);
        const minutes = parseInt(match[2], 10);
        return (hours * 60 + (hours >= 0 ? minutes : -minutes)) * 60 * 1000;
    }

    init(): void {
        const cached = localStorage.getItem('timezone');
        if (cached) {
            this.setTimezone(cached);
            return;
        }

        this.fetchTimezone();
    }

    update(value: string): void {
        this.setTimezone(value);
        localStorage.setItem('timezone', value);
    }

    private setTimezone(value: string): void {
        this.timezone = value;
        this.timezoneSubject.next(value);
    }

    private async fetchTimezone(): Promise<void> {
        try {
            const res = await this.api.getAnonym<{ data: string }>('settings/timezone');
            const value = res?.data?.data;
            if (value) {
                this.setTimezone(value);
                localStorage.setItem('timezone', value);
            }
        } catch {
            this.setTimezone('+03:00');
        }
    }
}
