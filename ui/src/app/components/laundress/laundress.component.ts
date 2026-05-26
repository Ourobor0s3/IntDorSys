import { Component, Inject, LOCALE_ID, OnDestroy, OnInit } from '@angular/core';
import { BaseComponent } from 'src/app/shared/component/base/base.component';
import { LaundressService } from "../../shared/services/laundress.service";
import { PageLaundressModel } from "../../shared/model/laundress.model";
import { DataReloadService } from "../../shared/services/dataReload.service";
import { Subject, takeUntil } from "rxjs";
import { NgbModal } from "@ng-bootstrap/ng-bootstrap";
import { BaseFilterModel } from "../../shared/model/filter/baseFilter.model";
import { TranslateService } from '@ngx-translate/core';
import { UsersInfoService } from "../../shared/services/user-info.service";
import { UserInfoModel } from "../../shared/model/userInfo.model";
import { UserService } from "../../shared/services/user.service";
import { AuthService } from "../../shared/services/auth.service";

@Component({
    selector: 'app-laundress',
    templateUrl: './laundress.component.html',
    styleUrls: ['./laundress.component.scss'],
})
export class LaundressComponent extends BaseComponent implements OnInit, OnDestroy {
    laundList?: PageLaundressModel[];
    timerId: any;
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
    newTimeRangeStartHour: number = 8;
    newTimeRangeEndHour: number = 10;
    evenHours: number[] = [8, 10, 12, 14, 16, 18, 20, 22];

    constructor(
        @Inject(LOCALE_ID) public locale: string,
        private laundService: LaundressService,
        private dataReloadService: DataReloadService,
        private modal: NgbModal,
        private translate: TranslateService,
        private usersInfoService: UsersInfoService,
        private userService: UserService,
        private authService: AuthService,
    ) {
        super(translate, modal);
        let t = this;
        let dateBE = t.GetCurrentDateWithDelta();
        t.startDate = dateBE.dateStart;
        t.endDate = dateBE.dateEnd;
    }

    ngOnInit() {
        let t = this;
        t.isAdmin = t.authService.isAdmin();
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

        t.laundService.getLaund(t.filter)
            .then(res => {
                t.laundList = res.data;
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

    toggleEditMode() {
        this.isEditMode = !this.isEditMode;
        if (this.isEditMode) {
            this.loadUsers();
        }
    }

    loadUsers() {
        let t = this;
        t.usersInfoService.getUsers()
            .then(res => {
                t.users = res.data ?? [];
            })
            .catch(err => console.log(err));
    }

    openCreateModal(content: any) {
        let now = new Date();
        this.newTimeWashDate = now.toISOString().split('T')[0];
        this.newTimeRangeStartHour = 8;
        this.newTimeRangeEndHour = 10;
        this.modal.open(content, { centered: true, backdrop: 'static', size: 'sm' });
    }

    createTimeSlotRange(modal: any) {
        let t = this;
        let userId = t.userService.user?.id;

        if (!userId) {
            t.showError('User not authenticated');
            return;
        }

        t.laundService.createTimeRange(t.newTimeWashDate, t.newTimeRangeStartHour, t.newTimeRangeEndHour, userId)
            .then(res => {
                if (res.isSuccess) {
                    t.showSuccess(t.translate.instant('laundress.create_success'));
                    modal.close();
                    t.searchTimeLaund();
                } else {
                    t.showResponseError(res);
                }
            })
            .catch(err => t.showResponseError(err));
    }

    openBookModal(content: any, timeWash: string) {
        this.selectedTimeWash = timeWash;
        this.selectedUserId = 0;
        if (this.users.length === 0) {
            this.loadUsers();
        }
        this.modal.open(content, { centered: true, backdrop: 'static', size: 'sm' });
    }

    bookUser(modal: any) {
        let t = this;
        if (t.selectedUserId <= 0) {
            t.showError(t.translate.instant('laundress.select_user_required'));
            return;
        }
        t.laundService.bookUser(t.selectedTimeWash, t.selectedUserId)
            .then(res => {
                if (res.isSuccess) {
                    t.showSuccess(t.translate.instant('laundress.book_success'));
                    modal.close();
                    t.searchTimeLaund();
                } else {
                    t.showResponseError(res);
                }
            })
            .catch(err => t.showResponseError(err));
    }

    confirmUnbook(timeWash: string, userId: number) {
        let t = this;
        t.showConfirm(
            t.translate.instant('laundress.confirm_unbook'),
            '',
        )
            .then(() => {
                t.laundService.unbookUser(timeWash, userId)
                    .then(res => {
                        if (res.isSuccess) {
                            t.searchTimeLaund();
                        } else {
                            t.showResponseError(res);
                        }
                    })
                    .catch(err => t.showResponseError(err));
            })
            .catch(() => {});
    }

    confirmDelete(timeWash: string) {
        let t = this;
        t.showConfirm(
            t.translate.instant('laundress.confirm_delete_slot'),
            '',
        )
            .then(() => {
                t.laundService.deleteTime(timeWash)
                    .then(res => {
                        if (res.isSuccess) {
                            t.searchTimeLaund();
                        } else {
                            t.showResponseError(res);
                        }
                    })
                    .catch(err => t.showResponseError(err));
            })
            .catch(() => {});
    }

    isSlotExpired(timeWash: string): boolean {
        return new Date(timeWash) < new Date();
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
