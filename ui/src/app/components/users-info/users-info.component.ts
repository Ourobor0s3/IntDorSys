import { Component, Inject, LOCALE_ID, OnDestroy, OnInit } from '@angular/core';
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

@Component({
    selector: 'app-users-info',
    templateUrl: './users-info.component.html',
    styleUrls: ['./users-info.component.scss'],
})
export class UserInfoComponent extends BaseComponent implements OnInit, OnDestroy {
    userList?: UserInfoModel[];
    filteredUsers?: UserInfoModel[];
    modalRef?: NgbModalRef;
    query: string = '';
    sortKey: string = 'name';
    sortDesc: boolean = false;
    showWithNameOnly: boolean = true;
    protected readonly UserStatus = UserStatus;
    private destroy$ = new Subject<void>();

    constructor(
        @Inject(LOCALE_ID) public locale: string,
        private modalService: NgbModal,
        private userService: UsersInfoService,
        private dataReloadService: DataReloadService,
        private clipboardService: ClipboardService,
        private modal: NgbModal,
        private translate: TranslateService,
    ) {
        super(translate, modal);
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
        t.searchUsers();
    }

    searchUsers() {
        let t = this;
        t.setLoading(true);

        t.userService.getUsers().then(res => {
            t.userList = res.data;
            t.applyFilter();
        }).catch((err) => {
                console.log(err);
            })
            .finally(() => {
                t.setLoading(false);
            });
    }

    applyFilter() {
        let t = this;
        if (!t.userList) { t.filteredUsers = []; return; }

        let result = [...t.userList];

        if (t.showWithNameOnly) {
            result = result.filter(x => !!x.fullName);
        }

        if (t.query) {
            let q = t.query.trim().toLowerCase();
            result = result.filter(x =>
                (x.fullName && x.fullName.toLowerCase().includes(q)) ||
                (x.username && x.username.toLowerCase().includes(q))
            );
        }

        switch (t.sortKey) {
            case 'id':
                result.sort((a, b) => t.sortDesc ? b.id - a.id : a.id - b.id);
                break;
            case 'status':
                result.sort((a, b) => t.sortDesc
                    ? Number(b.isBlocked) - Number(a.isBlocked)
                    : Number(a.isBlocked) - Number(b.isBlocked));
                break;
            default:
                result.sort((a, b) => {
                    let nameA = (a.fullName || a.username || '').toLowerCase();
                    let nameB = (b.fullName || b.username || '').toLowerCase();
                    return t.sortDesc ? nameB.localeCompare(nameA) : nameA.localeCompare(nameB);
                });
        }

        t.filteredUsers = result;
    }

    toggleSortDir() {
        this.sortDesc = !this.sortDesc;
        this.applyFilter();
    }

    toggleNameFilter() {
        this.showWithNameOnly = !this.showWithNameOnly;
        this.applyFilter();
    }

    ngOnDestroy(): void {
        this.destroy$.next();
        this.destroy$.complete();
    }

    copyText(text: string) {
        let t = this;
        t.clipboardService.copy(text);
        t.showToast(t.translate.instant('common.copy_success'));
    }

    getStatusStyle(status: UserStatus) {
        return USER_STATUS_STYLES[status];
    }

    changeStatus(user: UserInfoModel) {
        let t = this;
        t.modalRef = t.modalService.open(ConfirmModal, t.getModalOptions('sm'));

        t.modalRef.componentInstance.title = t.translate.instant('common.confirm')
        t.modalRef.componentInstance.showConfirmButton = !user.isBlocked;
        t.modalRef.componentInstance.showErrorButton = user.isBlocked;
        t.modalRef.componentInstance.description = user.isBlocked ? t.translate.instant('users.confirm_unblock') : t.translate.instant('users.confirm_block');
        t.modalRef.componentInstance.buttonError = t.translate.instant('common.ok');
        t.modalRef.componentInstance.buttonConfirm = t.translate.instant('common.ok');
        t.modalRef.componentInstance.buttonDecline = t.translate.instant('common.cancel');

        t.modalRef.result
            .then((result) => {
                if (result) t.userService.changeUserStatus(user.id, !user.isBlocked).then(() => {
                    t.showToast(t.translate.instant('common.status_changed'));
                }).finally(() => {
                    t.searchUsers();
                });
            })
            .catch(() => {
                // Console errors off
            });
    }
}
