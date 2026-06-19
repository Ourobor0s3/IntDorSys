import { Component, OnInit } from '@angular/core';
import { BaseComponent } from 'src/app/shared/component/base/base.component';
import { TranslateService } from '@ngx-translate/core';
import { ClipboardService } from "ngx-clipboard";
import { SettingsService } from "../../shared/services/settings.service";
import { SettingItem } from "../../shared/interface/setting-item";
import { BotStatus } from "../../shared/interface/bot-status";

@Component({
    selector: 'app-settings',
    templateUrl: './settings.component.html',

})
export class SettingsComponent extends BaseComponent implements OnInit {
    items: (SettingItem & { originalValue: string; editing: boolean })[] = [];
    saving: boolean = false;

    botStatus: BotStatus | null = null;
    botRestarting: boolean = false;

    constructor(
        private settingsService: SettingsService,
        private translate: TranslateService,
        private clipboardService: ClipboardService,
    ) {
        super();
    }

    ngOnInit() {
        this.loadSettings();
        this.loadBotStatus();
    }

    async loadSettings() {
        this.setLoading(true);
        try {
            let res = await this.settingsService.getAll();
            this.items = (res?.data ?? []).map((s) => ({
                ...s,
                originalValue: s.value,
                editing: false,
            }));
        } catch (err) {
            this.showResponseError(err);
        } finally {
            this.setLoading(false);
        }
    }

    async loadBotStatus() {
        try {
            let res = await this.settingsService.getBotStatus();
            this.botStatus = res?.data ?? null;
        } catch {
            this.botStatus = null;
        }
    }

    copyUsername() {
        if (this.botStatus?.username) {
            this.clipboardService.copy('@' + this.botStatus.username);
            this.showToast(this.translate.instant('common.copy_success'));
        }
    }

    async restartBot() {
        this.botRestarting = true;
        try {
            let res = await this.settingsService.restartBot();
            if (res?.isSuccess) {
                this.showToast(this.translate.instant('bot.restart_success'));
                await this.loadBotStatus();
            } else {
                this.showToast(this.translate.instant('bot.restart_fail'));
            }
        } catch {
            this.showToast(this.translate.instant('bot.restart_fail'));
        } finally {
            this.botRestarting = false;
        }
    }

    editItem(item: SettingItem & { editing: boolean }) {
        this.items = this.items.map(i => i.id === item.id ? { ...i, editing: true } : i);
    }

    cancelEdit(item: SettingItem & { originalValue: string; editing: boolean }) {
        this.items = this.items.map(i => i.id === item.id ? { ...i, editing: false, value: i.originalValue } : i);
    }

    async saveItem(item: SettingItem & { originalValue: string; editing: boolean }) {
        if (item.value === item.originalValue) {
            this.items = this.items.map(i => i.id === item.id ? { ...i, editing: false } : i);
            return;
        }
        this.saving = true;
        try {
            let res = await this.settingsService.update(item.id, item.value);
            if (res?.isSuccess) {
                this.items = this.items.map(i => i.id === item.id ? { ...i, originalValue: i.value, editing: false } : i);
                this.showToast(this.translate.instant('common.saved'));
                if (item.key === 'TimeZone') {
                    this.timezoneService.update(item.value);
                }
            }
        } catch (err) {
            this.showResponseError(err);
        } finally {
            this.saving = false;
        }
    }
}
