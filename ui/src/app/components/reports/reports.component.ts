import { Component, OnDestroy, OnInit } from '@angular/core';
import { BaseComponent } from 'src/app/shared/component/base/base.component';
import { LaundressService } from "../../shared/services/laundress.service";
import { DataReloadService } from "../../shared/services/dataReload.service";
import { Subject, takeUntil } from "rxjs";
import { NgbModal } from "@ng-bootstrap/ng-bootstrap";
import { BaseFilterModel } from "../../shared/model/filter/baseFilter.model";
import { TranslateService } from '@ngx-translate/core';
import { ReportModel } from "../../shared/model/report.model";
import { LoadingService } from "../../shared/services/loading.service";

@Component({
    selector: 'app-reports',
    templateUrl: './reports.component.html',
    styleUrls: ['./reports.component.scss'],
})
export class ReportsComponent extends BaseComponent implements OnInit, OnDestroy {
    reportList: ReportModel[] = [];
    sortedReports: ReportModel[] = [];
    timerId: ReturnType<typeof setTimeout> | null;
    filter: BaseFilterModel = new BaseFilterModel();
    startDate: Date = new Date();
    endDate: Date = new Date();
    sortDesc: boolean = true;
    lightboxReport: ReportModel | null = null;
    lightboxIndex: number = 0;
    private destroy$ = new Subject<void>();

    constructor(
        private laundService: LaundressService,
        private dataReloadService: DataReloadService,
        private modal: NgbModal,
        private translate: TranslateService,
        private loading: LoadingService,
    ) {
        super(translate, modal, loading);
        let t = this;
        let dateBE = t.getCurrentDateWithDelta();
        t.startDate = dateBE.dateStart;
        t.endDate = dateBE.dateEnd;
    }

    ngOnInit() {
        let t = this;
        t.loadData();
        t.startAutoRefresh();
        t.dataReloadService.dataReload$
            .pipe(takeUntil(t.destroy$))
            .subscribe(() => {
                t.loadData();
            });
    }

    loadData() {
        let t = this;
        t.setLoading(true);
        t.filter.startDate = t.startDate.toISOString();
        t.filter.endDate = t.endDate.toISOString();

        t.laundService.getReports(t.filter)
            .then(res => {
                t.reportList = res.data ?? [];
                t.applySort();
            })
            .catch((err) => {
                console.error(err);
            })
            .finally(() => {
                t.setLoading(false);
            });
    }

    searchReports() {
        let t = this;
        t.setLoading(true);
        t.filter.startDate = t.startDate.toISOString();
        t.filter.endDate = t.endDate.toISOString();

        t.laundService.getReports(t.filter)
            .then(res => {
                t.reportList = res.data ?? [];
                t.applySort();
            })
            .catch((err) => {
                console.error(err);
            })
            .finally(() => {
                t.setLoading(false);
            });
    }

    applySort() {
        let t = this;
        t.sortedReports = t.sortDesc
            ? [...t.reportList].sort((a, b) => new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime())
            : [...t.reportList].sort((a, b) => new Date(a.createdAt).getTime() - new Date(b.createdAt).getTime());
    }

    toggleSort() {
        this.sortDesc = !this.sortDesc;
        this.applySort();
    }

    startAutoRefresh() {
        let t = this;
        clearTimeout(t.timerId);
        t.timerId = setTimeout(() => {
            t.searchReports();
            t.startAutoRefresh();
        }, 60000);
    }

    openLightbox(report: ReportModel, index: number) {
        this.lightboxReport = report;
        this.lightboxIndex = index;
    }

    closeLightbox() {
        this.lightboxReport = null;
    }

    lightboxNext() {
        if (!this.lightboxReport || this.lightboxReport.files.length === 0) return;
        this.lightboxIndex = (this.lightboxIndex + 1) % this.lightboxReport.files.length;
    }

    lightboxPrev() {
        if (!this.lightboxReport || this.lightboxReport.files.length === 0) return;
        this.lightboxIndex = (this.lightboxIndex - 1 + this.lightboxReport.files.length) % this.lightboxReport.files.length;
    }

    ngOnDestroy(): void {
        clearTimeout(this.timerId);
        this.destroy$.next();
        this.destroy$.complete();
    }
}
