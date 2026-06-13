import { Component, OnDestroy, OnInit } from '@angular/core';
import { BaseComponent } from 'src/app/shared/component/base/base.component';
import { NgbModal } from "@ng-bootstrap/ng-bootstrap";
import { Subject, takeUntil } from "rxjs";
import { PageLaundressModel } from "../../shared/model/laundress.model";
import { LaundressService } from "../../shared/services/laundress.service";
import { DataReloadService } from "../../shared/services/dataReload.service";
import { AnaliticService } from "../../shared/services/analitic.service";
import { ChartData } from "../../shared/model/chartData.model";
import { TranslateService } from '@ngx-translate/core';
import { BaseFilterModel } from "../../shared/model/filter/baseFilter.model";
import { LoadingService } from "../../shared/services/loading.service";

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
        private modal: NgbModal,
        private translate: TranslateService,
        private loading: LoadingService,
    ) {
        super(translate, modal, loading);
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

    getAnaliticLaund(isRunLoading: boolean = true) {
        if (isRunLoading)
            this.setLoading(true);

        this.analiticService.getAnaliticLaund().then(res => {
            this.chart = res.data;
        }).catch((err) => {
                console.error(err);
            })
            .finally(() => {
                if (isRunLoading)
                    this.setLoading(false);
            });
    }

    searchTimeLaund(isRunLoading: boolean = true) {
        if (isRunLoading) {
            this.setLoading(true);
        }

        this.laundService.getLaund(this.filter)
            .then(res => {
                this.laund = res.data && res.data.length > 0 ? res.data[0] : undefined;
            })
            .catch((err) => {
                console.error(err);
            })
            .finally(() => {
                if (isRunLoading)
                    this.setLoading(false);
            });
    }

    ngOnDestroy(): void {
        this.destroy$.next();
        this.destroy$.complete();
    }
}
