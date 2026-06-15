import { Component, OnDestroy, OnInit } from '@angular/core';
import { BaseComponent } from 'src/app/shared/component/base/base.component';
import { LaundressService } from "../../shared/services/laundress.service";
import { PageLaundressModel } from "../../shared/model/laundress.model";
import { DataReloadService } from "../../shared/services/dataReload.service";
import { Subject, takeUntil } from "rxjs";
import { NgbModal, NgbModalRef } from "@ng-bootstrap/ng-bootstrap";
import { BaseFilterModel } from "../../shared/model/filter/baseFilter.model";
import { TranslateService } from '@ngx-translate/core';
import { UsersInfoService } from "../../shared/services/user-info.service";
import { UserInfoModel } from "../../shared/model/userInfo.model";
import { UserService } from "../../shared/services/user.service";
import { AuthService } from "../../shared/services/auth.service";
import { LoadingService } from "../../shared/services/loading.service";

@Component({
    selector: 'app-laundress',
    templateUrl: './laundress.component.html',
    styleUrls: ['./laundress.component.scss'],
})
export class LaundressComponent extends BaseComponent implements OnInit, OnDestroy {
    laundList?: PageLaundressModel[];
    timerId: ReturnType<typeof setTimeout> | undefined;
    filter: BaseFilterModel = new BaseFilterModel();
    isAutoRefresh: boolean = true;
    startDate: Date = new Date();
    endDate: Date = new Date();
    private destroy$ = new Subject<void>();

    isEditMode: boolean = false;
    isAdmin: boolean = false;
    users: UserInfoModel[] = [];
    selectedTimeWash: string = '';
    selectedUserId: number = 0;

    newTimeWashDate: string = '';
    newTimeRangeStartHour: number = 0;
    newTimeRangeEndHour: number = 22;
    evenHours: number[] = [0, 2, 4, 6, 8, 10, 12, 14, 16, 18, 20, 22];

    constructor(
        private laundService: LaundressService,
        private dataReloadService: DataReloadService,
        private modal: NgbModal,
        private translate: TranslateService,
        private usersInfoService: UsersInfoService,
        private userService: UserService,
        private authService: AuthService,
        private loading: LoadingService,
    ) {
        super(translate, modal, loading);
        let dateBE = this.getCurrentDateWithDelta();
        this.startDate = dateBE.dateStart;
        this.endDate = dateBE.dateEnd;
    }

    ngOnInit() {
        this.isAdmin = this.authService.isAdmin();
        this.searchTimeLaund();
        this.dataReloadService.dataReload$
            .pipe(takeUntil(this.destroy$))
            .subscribe(() => {
                this.searchTimeLaund();
            });
    }

    searchTimeLaund(isRunLoading: boolean = true) {
        if (isRunLoading) {
            this.setLoading(true);
            this.disableAutoRefresh()
        }

        this.filter.startDate = this.startDate.toISOString();
        this.filter.endDate = this.endDate.toISOString();

        this.laundService.getLaund(this.filter)
            .then(res => {
                this.laundList = res.data;
            })
            .catch((err) => {
                this.showResponseError(err);
            })
            .finally(() => {
                if (isRunLoading)
                    this.setLoading(false);
                if (this.isAutoRefresh)
                    this.timerId = setTimeout(() => this.searchTimeLaund(false), 1000 * 60);
            });
    }

    toggleEditMode() {
        this.isEditMode = !this.isEditMode;
        if (this.isEditMode) {
            this.loadUsers();
        }
    }

    loadUsers() {
        this.usersInfoService.getUsers()
            .then(res => {
                this.users = res.data?.filter(x => !x.isBlocked && x.isConfirm ) ?? [];
            })
            .catch(err => this.showResponseError(err));
    }

    openCreateModal(content: unknown) {
        let now = new Date();
        this.newTimeWashDate = now.toISOString().split('T')[0];
        this.newTimeRangeStartHour = 8;
        this.newTimeRangeEndHour = 22;
        this.modal.open(content, { centered: true, backdrop: 'static', size: 'md' });
    }

    async createTimeSlotRange(modal: NgbModalRef) {
        this.laundService.createTimeRange(this.newTimeWashDate, this.newTimeRangeStartHour, this.newTimeRangeEndHour)
            .then(res => {
                if (res.isSuccess) {
                    this.showSuccess(this.translate.instant('laundress.create_success', { count: res.data }));
                    modal.close();
                    this.searchTimeLaund();
                } else {
                    this.showResponseError(res);
                }
            })
            .catch(err => this.showResponseError(err));
    }

    openBookModal(content: unknown, timeWash: string) {
        this.selectedTimeWash = timeWash;
        this.selectedUserId = 0;
        if (this.users.length === 0) {
            this.loadUsers();
        }
        this.modal.open(content, { centered: true, backdrop: 'static', size: 'md' });
    }

    bookUser(modal: NgbModalRef) {
        if (this.selectedUserId <= 0) {
            this.showError(this.translate.instant('laundress.select_user_required'));
            return;
        }
        this.laundService.bookUser(this.selectedTimeWash, this.selectedUserId)
            .then(res => {
                if (res.isSuccess) {
                    this.showSuccess(this.translate.instant('laundress.book_success'));
                    modal.close();
                    this.searchTimeLaund();
                } else {
                    this.showResponseError(res);
                }
            })
            .catch(err => this.showResponseError(err));
    }

    async confirmUnbook(timeWash: string, userId: number) {
        try {
            await this.showConfirm(
                this.translate.instant('laundress.confirm_unbook'),
                '',
            );
            try {
                const res = await this.laundService.unbookUser(timeWash, userId);
                if (res.isSuccess) {
                    this.searchTimeLaund();
                } else {
                    this.showResponseError(res);
                }
            } catch (err) {
                this.showResponseError(err);
            }
        } catch {
            this.showError(this.translate.instant('common.operation_cancelled'));
        }
    }

    async confirmDelete(timeWash: string) {
        try {
            await this.showConfirm(
                this.translate.instant('laundress.confirm_delete_slot'),
                '',
            );
            try {
                const res = await this.laundService.deleteTime(timeWash);
                if (res.isSuccess) {
                    this.searchTimeLaund();
                } else {
                    this.showResponseError(res);
                }
            } catch (err) {
                this.showResponseError(err);
            }
        } catch {
            this.showError(this.translate.instant('common.operation_cancelled'));
        }
    }

    // Export
    exportExcel() {
        this.laundService.exportExcel(this.startDate.toISOString(), this.endDate.toISOString())
            .catch(err => this.showResponseError(err));
    }

    isSlotExpired(timeWash: string): boolean {
        return new Date(timeWash) < new Date();
    }

    disableAutoRefresh() {
        this.isAutoRefresh = false;
        clearTimeout(this.timerId);
    }

    ngOnDestroy(): void {
        this.disableAutoRefresh();
        this.destroy$.next();
        this.destroy$.complete();
    }

}
