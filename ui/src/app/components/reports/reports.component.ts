import { Component, OnDestroy, OnInit } from '@angular/core';
import { BaseComponent } from 'src/app/shared/component/base/base.component';
import { LaundressService } from "../../shared/services/laundress.service";
import { DataReloadService } from "../../shared/services/dataReload.service";
import { Subject, takeUntil } from "rxjs";
import { BaseFilterModel } from "../../shared/model/filter/baseFilter.model";
import { ReportModel } from "../../shared/interface/report.model";
@Component({
    selector: 'app-reports',
    templateUrl: './reports.component.html',
    styleUrls: ['./reports.component.scss'],
})
export class ReportsComponent extends BaseComponent implements OnInit, OnDestroy {
    reportList: ReportModel[] = [];
    sortedReports: ReportModel[] = [];
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
    ) {
        super();
        let dateBE = this.getCurrentDateWithDelta();
        this.startDate = dateBE.dateStart;
        this.endDate = dateBE.dateEnd;
    }

    ngOnInit() {
        this.loadData();
        this.dataReloadService.dataReload$
            .pipe(takeUntil(this.destroy$))
            .subscribe(() => {
                this.loadData();
            });
    }

    async loadData() {
        this.setLoading(true);
        this.filter.startDate = this.startDate.toISOString();
        this.filter.endDate = this.endDate.toISOString();

        try {
            const res = await this.laundService.getReports(this.filter);
            this.reportList = res.data ?? [];
            this.applySort();
        } catch (err) {
            this.showResponseError(err);
        } finally {
            this.setLoading(false);
        }
    }

    searchReports() {
        this.loadData();
    }

    applySort() {
        this.sortedReports = this.sortDesc
            ? [...this.reportList].sort((a, b) => new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime())
            : [...this.reportList].sort((a, b) => new Date(a.createdAt).getTime() - new Date(b.createdAt).getTime());
    }

    toggleSort() {
        this.sortDesc = !this.sortDesc;
        this.applySort();
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
        this.destroy$.next();
        this.destroy$.complete();
    }
}
