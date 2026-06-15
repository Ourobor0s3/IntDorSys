import { lastValueFrom } from 'rxjs';
import { Component, OnInit } from '@angular/core';
import { BaseComponent } from 'src/app/shared/component/base/base.component';
import { NgbModal } from "@ng-bootstrap/ng-bootstrap";
import { TranslateService } from '@ngx-translate/core';
import { ClipboardService } from "ngx-clipboard";
import { LoadingService } from "../../shared/services/loading.service";
import { ApiService } from "../../shared/services/api.service";

interface SettingItem {
    id: number;
    key: string;
    value: string;
    originalValue: string;
    isEditable: boolean;
    editing: boolean;
}

interface BotStatus {
    running: boolean;
    username: string | null;
    lastStarted: string | null;
}

@Component({
    selector: 'app-settings',
    templateUrl: './settings.component.html',

})
export class SettingsComponent extends BaseComponent implements OnInit {
    items: SettingItem[] = [];
    saving: boolean = false;

    botStatus: BotStatus | null = null;
    botRestarting: boolean = false;

    constructor(
        private api: ApiService,
        private modal: NgbModal,
        private translate: TranslateService,
        private loading: LoadingService,
        private clipboardService: ClipboardService,
    ) {
        super(translate, modal, loading);
    }

    ngOnInit() {
        this.loadSettings();
        this.loadBotStatus();
    }

    async loadSettings() {
        this.setLoading(true);
        try {
            let res = await lastValueFrom(await this.api.get<unknown>('settings'));
            this.items = ((res?.data ?? []) as Array<{ id: number; key: string; value: string; isEditable: boolean }>).map((s) => ({
                id: s.id,
                key: s.key,
                value: s.value,
                originalValue: s.value,
                isEditable: s.isEditable,
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
            let res = await lastValueFrom(await this.api.get<BotStatus>('bot/status'));
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
            let res = await lastValueFrom(await this.api.post<unknown>('bot/restart', {}));
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

    editItem(item: SettingItem) {
        item.editing = true;
    }

    cancelEdit(item: SettingItem) {
        item.editing = false;
        item.value = item.originalValue;
    }

    async saveItem(item: SettingItem) {
        if (item.value === item.originalValue) {
            item.editing = false;
            return;
        }
        this.saving = true;
        try {
            let res = await lastValueFrom(await this.api.put<unknown>('settings/' + item.id, { value: item.value }));
            if (res?.isSuccess) {
                item.originalValue = item.value;
                item.editing = false;
                this.showToast(this.translate.instant('common.saved'));
            }
        } catch (err) {
            this.showResponseError(err);
        } finally {
            this.saving = false;
        }
    }
}
