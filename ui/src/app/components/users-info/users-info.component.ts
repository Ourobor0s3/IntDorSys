import { Component, OnDestroy, OnInit } from '@angular/core';
import { BaseComponent } from 'src/app/shared/component/base/base.component';
import { Subject, takeUntil } from "rxjs";
import { DataReloadService } from "../../shared/services/dataReload.service";
import { UserInfoModel } from "../../shared/interface/userInfo.model";
import { NgbModal } from "@ng-bootstrap/ng-bootstrap";
import { ClipboardService } from "ngx-clipboard";
import { UsersInfoService } from "../../shared/services/user-info.service";
import { TranslateService } from '@ngx-translate/core';
import { UserStatus } from "../../shared/enums/UserStatus";
import { PaginationState } from "../../shared/component/base/pagination-state";
import { ConfirmModal } from "../../shared/component/modals/confirm/confirm.modal";
@Component({
    selector: 'app-users-info',
    templateUrl: './users-info.component.html',
    styleUrls: ['./users-info.component.scss'],
})
export class UserInfoComponent extends BaseComponent implements OnInit, OnDestroy {
    userList?: UserInfoModel[];
    filteredUsers?: UserInfoModel[];
    users: UserInfoModel[] = [];
    pag = new PaginationState();
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
        private translate: TranslateService,
    ) {
        super();
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

    async searchUsers() {
        this.setLoading(true);

        try {
            const res = await this.userService.getUsers();
            this.userList = res.data;
            this.applyFilter();
        } catch (err) {
            this.showResponseError(err);
        } finally {
            this.setLoading(false);
        }
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
        this.users = this.pag.slice(result);
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

    async changeStatus(user: UserInfoModel) {
        const isBlocking = !user.isBlocked;

        const modalRef = this.modalService.open(ConfirmModal, this.getModalOptions('sm'));
        modalRef.componentInstance.config = {
            title: this.translate.instant('common.confirm'),
            description: isBlocking
                ? this.translate.instant('users.confirm_block')
                : this.translate.instant('users.confirm_unblock'),
            buttonConfirm: this.translate.instant('common.ok'),
            buttonDecline: this.translate.instant('common.cancel'),
            buttonError: this.translate.instant('common.ok'),
            showConfirmButton: !isBlocking,
            showErrorButton: isBlocking,
        };

        try {
            const result = await modalRef.result;
            if (result) {
                await this.userService.changeUserStatus(user.id, isBlocking);
                this.showToast(this.translate.instant('common.status_changed'));
                this.searchUsers();
            }
        } catch (err) {
            this.showResponseError(err);
        }
    }
}
