import { Injectable } from '@angular/core';
import { ApiService } from './api.service';

@Injectable({
    providedIn: 'root',
})
export class TimezoneService {
    static timezone: string = '+03:00';

    static getOffsetMs(): number {
        const match = TimezoneService.timezone.match(/([+-]\d{2}):(\d{2})/);
        if (!match) return 0;
        const hours = parseInt(match[1], 10);
        const minutes = parseInt(match[2], 10);
        return (hours * 60 + (hours >= 0 ? minutes : -minutes)) * 60 * 1000;
    }

    constructor(private api: ApiService) {}

    init(): void {
        const cached = localStorage.getItem('timezone');
        if (cached) {
            TimezoneService.timezone = cached;
            return;
        }

        this.fetchTimezone();
    }

    private async fetchTimezone(): Promise<void> {
        try {
            const res = await this.api.getAnonym<{ data: string }>('settings/timezone').toPromise();
            const value = (res as any)?.data;
            if (value) {
                TimezoneService.timezone = value;
                localStorage.setItem('timezone', value);
            }
        } catch {
            TimezoneService.timezone = '+03:00';
        }
    }
}
