import { Component, Inject, LOCALE_ID, OnDestroy, OnInit } from '@angular/core';
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
        @Inject(LOCALE_ID) public locale: string,
        private laundService: LaundressService,
        private analiticService: AnaliticService,
        private dataReloadService: DataReloadService,
        private modal: NgbModal,
        private translate: TranslateService,
    ) {
        super(translate, modal);
        let t = this;
        t.filter.startDate = t.GetTodayDate().toISOString();
        t.filter.endDate = t.GetTodayDate().toISOString();
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
        t.getAnaliticLaund();
        t.searchTimeLaund()
    }

    getAnaliticLaund(isRunLoading: boolean = true) {
        let t = this;
        if (isRunLoading)
            t.setLoading(true);

        t.analiticService.getAnaliticLaund().then(res => {
            t.chart = res.data;
        }).catch((err) => {
                console.log(err);
            })
            .finally(() => {
                if (isRunLoading)
                    t.setLoading(false);
            });
    }

    searchTimeLaund(isRunLoading: boolean = true) {
        let t = this;
        if (isRunLoading) {
            t.setLoading(true);
        }

        t.laundService.getLaund(t.filter)
            .then(res => {
                t.laund = res.data[0];
            })
            .catch((err) => {
                console.log(err);
            })
            .finally(() => {
                if (isRunLoading)
                    t.setLoading(false);
            });
    }

    ngOnDestroy(): void {
        this.destroy$.next();
        this.destroy$.complete();
    }
}
