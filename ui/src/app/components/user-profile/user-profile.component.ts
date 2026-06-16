import { Component, OnDestroy, OnInit } from '@angular/core';
import { BaseComponent } from 'src/app/shared/component/base/base.component';
import { ActivatedRoute, Router } from '@angular/router';
import { Subject, takeUntil } from "rxjs";
import { UserInfoModel } from "../../shared/model/userInfo.model";
import { UsersInfoService } from "../../shared/services/user-info.service";
import { NgbModal, NgbModalRef } from "@ng-bootstrap/ng-bootstrap";
import { TranslateService } from '@ngx-translate/core';
import { UserStatus } from "../../shared/enums/UserStatus";
import { USER_STATUS_STYLES } from "../../shared/constants/statusStyle";
import { ConfirmModal } from "../../shared/component/modals/confirm";

@Component({
    selector: 'app-user-profile',
    templateUrl: './user-profile.component.html',
    styleUrls: ['./user-profile.component.scss'],
})
export class UserProfileComponent extends BaseComponent implements OnInit, OnDestroy {
    user?: UserInfoModel;
    userId: number = 0;
    modalRef?: NgbModalRef;
    protected readonly UserStatus = UserStatus;
    private destroy$ = new Subject<void>();

    constructor(
        private route: ActivatedRoute,
        private router: Router,
        private userService: UsersInfoService,
        modal: NgbModal,
        translate: TranslateService,
    ) {
        super(translate, modal);
    }

    ngOnInit() {
        this.route.params.pipe(takeUntil(this.destroy$)).subscribe(params => {
            this.userId = +params['id'];
            this.loadUser();
        });
    }

    loadUser() {
        if (!this.userId) return;

        this.setLoading(true);
        this.userService.getUserById(this.userId)
            .then(res => {
                this.user = res.data;
            })
            .catch(err => {
                this.showResponseError(err);
                this.router.navigate(['/user-info']);
            })
            .finally(() => this.setLoading(false));
    }

    goBack() {
        this.router.navigate(['/user-info']);
    }

    getStatusStyle(status: UserStatus) {
        return USER_STATUS_STYLES[status];
    }

    async confirmUser() {
        if (!this.user) return;

        this.modalRef = this.modalServiceBase.open(ConfirmModal, this.getModalOptions('sm'));

        this.modalRef.componentInstance.title = this.translateBase.instant('common.confirm');
        this.modalRef.componentInstance.description = this.translateBase.instant('user_page.confirm_user_desc');
        this.modalRef.componentInstance.buttonConfirm = this.translateBase.instant('common.ok');
        this.modalRef.componentInstance.buttonDecline = this.translateBase.instant('common.cancel');

        try {
            const result = await this.modalRef.result;
            if (result) {
                await this.userService.confirmUser(this.user.id);
                this.showToast(this.translateBase.instant('common.status_changed'));
                this.loadUser();
            }
        } catch (err) {
            this.showResponseError(err);
        }
    }

    async changeStatus() {
        if (!this.user) return;

        const isBlocking = this.user.status !== UserStatus.Blocked;
        this.modalRef = this.modalServiceBase.open(ConfirmModal, this.getModalOptions('sm'));

        this.modalRef.componentInstance.title = this.translateBase.instant('common.confirm');
        this.modalRef.componentInstance.showConfirmButton = !isBlocking;
        this.modalRef.componentInstance.showErrorButton = isBlocking;
        this.modalRef.componentInstance.description = isBlocking
            ? this.translateBase.instant('users.confirm_block')
            : this.translateBase.instant('users.confirm_unblock');
        this.modalRef.componentInstance.buttonError = this.translateBase.instant('common.ok');
        this.modalRef.componentInstance.buttonConfirm = this.translateBase.instant('common.ok');
        this.modalRef.componentInstance.buttonDecline = this.translateBase.instant('common.cancel');

        try {
            const result = await this.modalRef.result;
            if (result) {
                await this.userService.changeUserStatus(this.user.id, isBlocking);
                this.showToast(this.translateBase.instant('common.status_changed'));
                this.loadUser();
            }
        } catch (err) {
            this.showResponseError(err);
        }
    }

    ngOnDestroy(): void {
        this.destroy$.next();
        this.destroy$.complete();
    }
}
