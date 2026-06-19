import { Component, OnDestroy, OnInit } from '@angular/core';
import { BaseComponent } from 'src/app/shared/component/base/base.component';
import { Subject, takeUntil } from "rxjs";
import { DataReloadService } from "../../shared/services/dataReload.service";
import { LaundressService } from "../../shared/services/laundress.service";
import { AuditLogModel } from "../../shared/interface/audit-log";
import { BaseFilterModel } from "../../shared/model/filter/baseFilter.model";
import { PaginationState } from "../../shared/component/base/pagination-state";

export const AUDIT_ACTIONS = ['CreateSlot', 'CreateSlotRange', 'BookSlot', 'UnbookSlot', 'DeleteSlot', 'ChangeUserStatus', 'UpdateSetting', 'ConfirmUser', 'RemoveRole'] as const;

@Component({
    selector: 'app-audit',
    templateUrl: './audit.component.html',
    styleUrls: ['./audit.component.scss'],
})
export class AuditComponent extends BaseComponent implements OnInit, OnDestroy {
    allLogs: AuditLogModel[] | null = null;
    logs: AuditLogModel[] = [];
    pag = new PaginationState();
    actionFilter: string = '';
    startDate: Date;
    endDate: Date;
    readonly actions = AUDIT_ACTIONS;
    private destroy$ = new Subject<void>();

    constructor(
        private laundService: LaundressService,
        private dataReloadService: DataReloadService,
    ) {
        super();
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

    async loadLogs() {
        this.setLoading(true);

        let filter = new BaseFilterModel();
        filter.skip = 0;
        filter.take = 9999;
        filter.startDate = this.startDate ? this.startDate.toISOString() : undefined;
        filter.endDate = this.endDate ? this.endDate.toISOString() : undefined;

        try {
            const res = await this.laundService.getAudit(filter);
            this.allLogs = res.data ?? [];
            this.applyFilter();
        } catch (err) {
            this.showResponseError(err);
        } finally {
            this.setLoading(false);
        }
    }

    applyFilter() {
        let filtered = this.allLogs;
        if (!filtered) {
            this.logs = [];
            this.pag.totalPages = 1;
            return;
        }
        if (this.actionFilter) {
            filtered = filtered.filter(x => x.action === this.actionFilter);
        }
        this.logs = this.pag.slice(filtered);
    }

    onFilterChange() {
        this.pag.onFilterChange();
        this.applyFilter();
    }

    prevPage() {
        if (this.pag.prev()) {
            this.applyFilter();
        }
    }

    nextPage() {
        if (this.pag.next()) {
            this.applyFilter();
        }
    }

    ngOnDestroy(): void {
        this.destroy$.next();
        this.destroy$.complete();
    }

}
