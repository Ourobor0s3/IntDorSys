import { Component, OnDestroy, OnInit } from '@angular/core';
import { BaseComponent } from 'src/app/shared/component/base/base.component';
import { Subject, takeUntil } from "rxjs";
import { PageLaundressModel } from "../../shared/interface/laundress.model";
import { LaundressService } from "../../shared/services/laundress.service";
import { DataReloadService } from "../../shared/services/dataReload.service";
import { AnaliticService } from "../../shared/services/analitic.service";
import { ChartData } from "../../shared/interface/chartData.model";
import { BaseFilterModel } from "../../shared/model/filter/baseFilter.model";
@Component({
    selector: 'app-home',
    templateUrl: './home.component.html',
    styleUrls: ['./home.component.scss'],
})
export class HomeComponent extends BaseComponent implements OnInit, OnDestroy {
    chart?: ChartData[];
    filter: BaseFilterModel = new BaseFilterModel();
    isAutoRefresh: boolean = true;
    laund?: PageLaundressModel;
    private destroy$ = new Subject<void>();

    constructor(
        private laundService: LaundressService,
        private analiticService: AnaliticService,
        private dataReloadService: DataReloadService,
    ) {
        super();
        this.filter.startDate = this.getTodayDate().toISOString();
        this.filter.endDate = this.getTodayDate().toISOString();
    }

    ngOnInit() {
        this.loadData();
        this.dataReloadService.dataReload$
            .pipe(takeUntil(this.destroy$))
            .subscribe(() => {
                this.loadData();
            });
    }

    loadData() {
        this.getAnaliticLaund();
        this.searchTimeLaund()
    }

    async getAnaliticLaund(isRunLoading: boolean = true) {
        if (isRunLoading)
            this.setLoading(true);

        try {
            const res = await this.analiticService.getAnaliticLaund();
            this.chart = res.data;
        } catch (err) {
            this.showResponseError(err);
        } finally {
            if (isRunLoading)
                this.setLoading(false);
        }
    }

    async searchTimeLaund(isRunLoading: boolean = true) {
        if (isRunLoading) {
            this.setLoading(true);
        }

        try {
            const res = await this.laundService.getLaund(this.filter);
            this.laund = res.data && res.data.length > 0 ? res.data[0] : undefined;
        } catch (err) {
            this.showResponseError(err);
        } finally {
            if (isRunLoading)
                this.setLoading(false);
        }
    }

    ngOnDestroy(): void {
        this.destroy$.next();
        this.destroy$.complete();
    }
}
