import { Component, OnDestroy, OnInit } from '@angular/core';
import { BaseComponent } from 'src/app/shared/component/base/base.component';
import { Subject, takeUntil } from "rxjs";
import { DataReloadService } from "../../shared/services/dataReload.service";
import { NgbModal } from "@ng-bootstrap/ng-bootstrap";
import { TranslateService } from '@ngx-translate/core';
import { LoadingService } from "../../shared/services/loading.service";
import { LaundressService, AuditLogModel } from "../../shared/services/laundress.service";
import { BaseFilterModel } from "../../shared/model/filter/baseFilter.model";

export const AUDIT_ACTIONS = ['CreateSlot', 'CreateSlotRange', 'BookSlot', 'UnbookSlot', 'DeleteSlot', 'ChangeUserStatus', 'UpdateSetting'] as const;

@Component({
    selector: 'app-audit',
    templateUrl: './audit.component.html',
    styleUrls: ['./audit.component.scss'],
})
export class AuditComponent extends BaseComponent implements OnInit, OnDestroy {
    allLogs: AuditLogModel[] | null = null;
    logs: AuditLogModel[] = [];
    page: number = 1;
    pageSize: number = 50;
    totalPages: number = 1;
    actionFilter: string = '';
    startDate: Date;
    endDate: Date;
    readonly actions = AUDIT_ACTIONS;
    private destroy$ = new Subject<void>();

    constructor(
        private modalService: NgbModal,
        private laundService: LaundressService,
        private dataReloadService: DataReloadService,
        private modal: NgbModal,
        private translate: TranslateService,
        private loading: LoadingService,
    ) {
        super(translate, modal, loading);
        let dates = this.getCurrentDateWithDelta(30);
        this.startDate = dates.dateStart;
        this.endDate = dates.dateEnd;
    }

    ngOnInit() {
        let t = this;
        t.loadLogs();
        t.dataReloadService.dataReload$
            .pipe(takeUntil(t.destroy$))
            .subscribe(() => {
                t.loadLogs();
            });
    }

    loadLogs() {
        let t = this;
        t.setLoading(true);

        let filter = new BaseFilterModel();
        filter.skip = 0;
        filter.take = 9999;
        filter.startDate = t.startDate ? t.startDate.toISOString() : undefined;
        filter.endDate = t.endDate ? t.endDate.toISOString() : undefined;

        t.laundService.getAudit(filter)
            .then(res => {
                t.allLogs = res.data ?? [];
                t.applyFilter();
            })
            .catch(err => console.error(err))
            .finally(() => t.setLoading(false));
    }

    applyFilter() {
        let t = this;
        let filtered = t.allLogs;
        if (t.actionFilter) {
            filtered = filtered.filter(x => x.action === t.actionFilter);
        }
        t.totalPages = Math.max(1, Math.ceil(filtered.length / t.pageSize));
        let start = (t.page - 1) * t.pageSize;
        t.logs = filtered.slice(start, start + t.pageSize);
    }

    onFilterChange() {
        this.page = 1;
        this.applyFilter();
    }

    prevPage() {
        if (this.page > 1) {
            this.page--;
            this.applyFilter();
        }
    }

    nextPage() {
        if (this.page < this.totalPages) {
            this.page++;
            this.applyFilter();
        }
    }

    ngOnDestroy(): void {
        this.destroy$.next();
        this.destroy$.complete();
    }

}
