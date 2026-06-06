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

@Component({
    selector: 'app-settings',
    templateUrl: './settings.component.html',
    styleUrls: ['./settings.component.scss'],
})
export class SettingsComponent extends BaseComponent implements OnInit {
    items: SettingItem[] = [];
    saving: boolean = false;

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
    }

    async loadSettings() {
        let t = this;
        t.setLoading(true);
        try {
            let res: any = await (await t.api.get<any>('settings')).toPromise();
            t.items = (res?.data ?? []).map((s: any) => ({
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
            let res: any = await (await t.api.put<any>('settings/' + item.id, { value: item.value })).toPromise();
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
