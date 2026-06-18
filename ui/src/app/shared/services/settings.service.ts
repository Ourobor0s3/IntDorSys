import { Injectable } from '@angular/core';
import { ApiService } from './api.service';
import { IResponse } from '../interface/response';
import { SettingItem } from '../interface/setting-item';
import { BotStatus } from '../interface/bot-status';

@Injectable({
    providedIn: 'root',
})
export class SettingsService {
    constructor(private api: ApiService) {
    }

    getAll(): Promise<IResponse<SettingItem[]>> {
        return this.api.get<SettingItem[]>('settings');
    }

    update(id: number, value: string): Promise<IResponse<unknown>> {
        return this.api.put<unknown>('settings/' + id, value);
    }

    getBotStatus(): Promise<IResponse<BotStatus>> {
        return this.api.get<BotStatus>('bot/status');
    }

    restartBot(): Promise<IResponse<unknown>> {
        return this.api.post<unknown>('bot/restart', {});
    }
}
