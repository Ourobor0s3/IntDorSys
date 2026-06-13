import { Component, OnDestroy, OnInit } from '@angular/core';
import { BaseComponent } from 'src/app/shared/component/base/base.component';
import { Subject, takeUntil } from "rxjs";
import { DataReloadService } from "../../shared/services/dataReload.service";
import { UserInfoModel } from "../../shared/model/userInfo.model";
import { NgbModal, NgbModalRef } from "@ng-bootstrap/ng-bootstrap";
import { ConfirmModal } from "../../shared/component/modals/confirm";
import { ClipboardService } from "ngx-clipboard";
import { UsersInfoService } from "../../shared/services/user-info.service";
import { TranslateService } from '@ngx-translate/core';
import { UserStatus } from "../../shared/enums/UserStatus";
import { USER_STATUS_STYLES } from "../../shared/constants/statusStyle";
import { LoadingService } from "../../shared/services/loading.service";

@Component({
    selector: 'app-users-info',
    templateUrl: './users-info.component.html',
    styleUrls: ['./users-info.component.scss'],
})
export class UserInfoComponent extends BaseComponent implements OnInit, OnDestroy {
    userList?: UserInfoModel[];
    filteredUsers?: UserInfoModel[];
    users: UserInfoModel[] = [];
    page: number = 1;
    pageSize: number = 50;
    totalPages: number = 1;
    modalRef?: NgbModalRef;
    query: string = '';
    sortKey: string = 'name';
    sortDesc: boolean = false;
    showWithNameOnly: boolean = true;
    protected readonly UserStatus = UserStatus;
    private destroy$ = new Subject<void>();

    constructor(
        private modalService: NgbModal,
        private userService: UsersInfoService,
        private dataReloadService: DataReloadService,
        private clipboardService: ClipboardService,
        private modal: NgbModal,
        private translate: TranslateService,
        private loading: LoadingService,
    ) {
        super(translate, modal, loading);
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
        this.searchUsers();
    }

    searchUsers() {
        this.setLoading(true);

        this.userService.getUsers().then(res => {
            this.userList = res.data;
            this.applyFilter();
        }).catch((err) => {
                this.showResponseError(err);
            })
            .finally(() => {
                this.setLoading(false);
            });
    }

    applyFilter() {
        if (!this.userList) { this.filteredUsers = []; return; }

        let result = [...this.userList];

        if (this.showWithNameOnly) {
            result = result.filter(x => !!x.fullName);
        }

        if (this.query) {
            let q = this.query.trim().toLowerCase();
            result = result.filter(x =>
                (x.fullName && x.fullName.toLowerCase().includes(q)) ||
                (x.username && x.username.toLowerCase().includes(q))
            );
        }

        switch (this.sortKey) {
            case 'id':
                result.sort((a, b) => this.sortDesc ? b.id - a.id : a.id - b.id);
                break;
            case 'status':
                result.sort((a, b) => this.sortDesc
                    ? Number(b.isBlocked) - Number(a.isBlocked)
                    : Number(a.isBlocked) - Number(b.isBlocked));
                break;
            default:
                result.sort((a, b) => {
                    let nameA = (a.fullName || a.username || '').toLowerCase();
                    let nameB = (b.fullName || b.username || '').toLowerCase();
                    return this.sortDesc ? nameB.localeCompare(nameA) : nameA.localeCompare(nameB);
                });
        }

        this.filteredUsers = result;
        this.totalPages = Math.max(1, Math.ceil(result.length / this.pageSize));
        let start = (this.page - 1) * this.pageSize;
        this.users = result.slice(start, start + this.pageSize);
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

    toggleSortDir() {
        this.sortDesc = !this.sortDesc;
        this.onFilterChange();
    }

    toggleNameFilter() {
        this.showWithNameOnly = !this.showWithNameOnly;
        this.onFilterChange();
    }

    ngOnDestroy(): void {
        this.destroy$.next();
        this.destroy$.complete();
    }

    copyText(text: string) {
        this.clipboardService.copy(text);
        this.showToast(this.translate.instant('common.copy_success'));
    }

    trackByUser(index: number, item: UserInfoModel): number {
        return item.id ?? index;
    }

    getStatusStyle(status: UserStatus) {
        return USER_STATUS_STYLES[status];
    }

    async changeStatus(user: UserInfoModel) {
        this.modalRef = this.modalService.open(ConfirmModal, this.getModalOptions('sm'));

        this.modalRef.componentInstance.title = this.translate.instant('common.confirm')
        this.modalRef.componentInstance.showConfirmButton = !user.isBlocked;
        this.modalRef.componentInstance.showErrorButton = user.isBlocked;
        this.modalRef.componentInstance.description = user.isBlocked ? this.translate.instant('users.confirm_unblock') : this.translate.instant('users.confirm_block');
        this.modalRef.componentInstance.buttonError = this.translate.instant('common.ok');
        this.modalRef.componentInstance.buttonConfirm = this.translate.instant('common.ok');
        this.modalRef.componentInstance.buttonDecline = this.translate.instant('common.cancel');

        try {
            const result = await this.modalRef.result;
            if (result) {
                await this.userService.changeUserStatus(user.id, !user.isBlocked);
                this.showToast(this.translate.instant('common.status_changed'));
                this.searchUsers();
            }
        } catch (err) {
            this.showResponseError(err);
        }
    }
}
