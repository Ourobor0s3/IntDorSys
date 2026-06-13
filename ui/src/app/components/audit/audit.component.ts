import { Component, OnDestroy, OnInit } from '@angular/core';
import { BaseComponent } from 'src/app/shared/component/base/base.component';
import { Subject, takeUntil } from "rxjs";
import { DataReloadService } from "../../shared/services/dataReload.service";
import { NgbModal } from "@ng-bootstrap/ng-bootstrap";
import { TranslateService } from '@ngx-translate/core';
import { LoadingService } from "../../shared/services/loading.service";
import { LaundressService } from "../../shared/services/laundress.service";
import { AuditLogModel } from "../../shared/interface/audit-log";
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
        this.loadLogs();
        this.dataReloadService.dataReload$
            .pipe(takeUntil(this.destroy$))
            .subscribe(() => {
                this.loadLogs();
            });
    }

    loadLogs() {
        this.setLoading(true);

        let filter = new BaseFilterModel();
        filter.skip = 0;
        filter.take = 9999;
        filter.startDate = this.startDate ? this.startDate.toISOString() : undefined;
        filter.endDate = this.endDate ? this.endDate.toISOString() : undefined;

        this.laundService.getAudit(filter)
            .then(res => {
                this.allLogs = res.data ?? [];
                this.applyFilter();
            })
            .catch(err => console.error(err))
            .finally(() => this.setLoading(false));
    }

    applyFilter() {
        let filtered = this.allLogs;
        if (this.actionFilter) {
            filtered = filtered.filter(x => x.action === this.actionFilter);
        }
        this.totalPages = Math.max(1, Math.ceil(filtered.length / this.pageSize));
        let start = (this.page - 1) * this.pageSize;
        this.logs = filtered.slice(start, start + this.pageSize);
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
