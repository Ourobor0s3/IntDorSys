import { Component, Inject, LOCALE_ID, OnDestroy, OnInit } from '@angular/core';
import { BaseComponent } from 'src/app/shared/component/base/base.component';
import { LaundressService } from "../../shared/services/laundress.service";
import { DataReloadService } from "../../shared/services/dataReload.service";
import { Subject, takeUntil } from "rxjs";
import { NgbModal } from "@ng-bootstrap/ng-bootstrap";
import { BaseFilterModel } from "../../shared/model/filter/baseFilter.model";
import { TranslateService } from '@ngx-translate/core';
import { ReportModel } from "../../shared/model/report.model";

@Component({
    selector: 'app-reports',
    templateUrl: './reports.component.html',
    styleUrls: ['./reports.component.scss'],
})
export class ReportsComponent extends BaseComponent implements OnInit, OnDestroy {
    reportList?: ReportModel[];
    timerId: any;
    isAutoRefresh: boolean = true;
    filter: BaseFilterModel = new BaseFilterModel();
    startDate: Date = new Date();
    endDate: Date = new Date();
    private destroy$ = new Subject<void>();

    constructor(
        @Inject(LOCALE_ID) public locale: string,
        private laundService: LaundressService,
        private dataReloadService: DataReloadService,
        private modal: NgbModal,
        private translate: TranslateService,
    ) {
        super(translate, modal);
        let t = this;
        let dateBE = t.GetCurrentDateWithDelta();
        t.startDate = dateBE.dateStart;
        t.endDate = dateBE.dateEnd;
    }

    ngOnInit() {
        let t = this;
        t.loadData();
        t.dataReloadService.dataReload$
            .pipe(takeUntil(t.destroy$))
            .subscribe(() => {
                t.loadData();
            });
    }

    loadData() {
        let t = this;
        t.searchTimeLaund();
    }

    searchTimeLaund(isRunLoading: boolean = true) {
        let t = this;
        if (isRunLoading) {
            t.setLoading(true);
            t.disableAutoRefresh()
        }

        t.filter.startDate = t.startDate.toISOString();
        t.filter.endDate = t.endDate.toISOString();

        t.laundService.getReports(t.filter)
            .then(res => {
                t.reportList = res.data;
            })
            .catch((err) => {
                console.log(err);
            })
            .finally(() => {
                if (isRunLoading)
                    t.setLoading(false);
                if (t.isAutoRefresh)
                    t.timerId = setTimeout(() => t.searchTimeLaund(false), 1000 * 60);
            });
    }

    public disableAutoRefresh() {
        this.isAutoRefresh = false;
        clearTimeout(this.timerId);
    }

    ngOnDestroy(): void {
        this.disableAutoRefresh();
        this.destroy$.next();
        this.destroy$.complete();
    }
}
