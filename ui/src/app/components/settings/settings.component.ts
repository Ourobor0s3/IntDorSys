import { Component, OnInit } from '@angular/core';
import { BaseComponent } from 'src/app/shared/component/base/base.component';
import { NgbModal } from "@ng-bootstrap/ng-bootstrap";
import { TranslateService } from '@ngx-translate/core';
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
    styleUrls: ['./settings.component.scss'],
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
    ) {
        super(translate, modal, loading);
    }

    ngOnInit() {
        this.loadSettings();
        this.loadBotStatus();
    }

    async loadSettings() {
        let t = this;
        t.setLoading(true);
        try {
            let res = await (await t.api.get<unknown>('settings')).toPromise();
            t.items = ((res?.data ?? []) as Array<{ id: number; key: string; value: string; isEditable: boolean }>).map((s) => ({
                id: s.id,
                key: s.key,
                value: s.value,
                originalValue: s.value,
                isEditable: s.isEditable,
                editing: false,
            }));
        } catch (err) {
            console.error(err);
        } finally {
            t.setLoading(false);
        }
    }

    async loadBotStatus() {
        try {
            let res = await (await this.api.get<BotStatus>('bot/status')).toPromise();
            this.botStatus = res?.data ?? null;
        } catch {
            this.botStatus = null;
        }
    }

    async restartBot() {
        this.botRestarting = true;
        try {
            let res = await (await this.api.post<unknown>('bot/restart', {})).toPromise();
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
        let t = this;
        if (item.value === item.originalValue) {
            item.editing = false;
            return;
        }
        t.saving = true;
        try {
            let res = await (await t.api.put<unknown>('settings/' + item.id, { value: item.value })).toPromise();
            if (res?.isSuccess) {
                item.originalValue = item.value;
                item.editing = false;
                t.showToast(t.translate.instant('common.saved'));
            }
        } catch (err) {
            console.error(err);
        } finally {
            t.saving = false;
        }
    }
}
