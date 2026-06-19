import { Component, OnDestroy, OnInit } from '@angular/core';
import { BaseComponent } from 'src/app/shared/component/base/base.component';
import { ActivatedRoute, Router } from '@angular/router';
import { Subject, takeUntil } from "rxjs";
import { UserInfoModel } from "../../shared/interface/userInfo.model";
import { UsersInfoService } from "../../shared/services/user-info.service";
import { ClipboardService } from "ngx-clipboard";
import { UserStatus } from "../../shared/enums/UserStatus";
import { ConfirmModal } from "../../shared/component/modals/confirm/confirm.modal";

@Component({
    selector: 'app-user-profile',
    templateUrl: './user-profile.component.html',
    styleUrls: ['./user-profile.component.scss'],
})
export class UserProfileComponent extends BaseComponent implements OnInit, OnDestroy {
    user?: UserInfoModel;
    userId: number = 0;
    protected readonly UserStatus = UserStatus;
    private destroy$ = new Subject<void>();

    constructor(
        private route: ActivatedRoute,
        private router: Router,
        private userService: UsersInfoService,
        private clipboardService: ClipboardService,
    ) {
        super();
    }

    ngOnInit() {
        this.route.params.pipe(takeUntil(this.destroy$)).subscribe(params => {
            this.userId = +params['id'];
            this.loadUser();
        });
    }

    async loadUser() {
        if (!this.userId) return;

        this.setLoading(true);
        try {
            const res = await this.userService.getUserById(this.userId);
            this.user = res.data;
        } catch (err) {
            this.showResponseError(err);
            this.router.navigate(['/user-info']);
        } finally {
            this.setLoading(false);
        }
    }

    copyTelegram() {
        if (!this.user?.username) return;

        this.clipboardService.copy(this.user.username);
        this.showToast(this.translateBase.instant('common.copy_success'));
    }

    async confirmUser() {
        if (!this.user) return;

        const modalRef = this.modalServiceBase.open(ConfirmModal, this.getModalOptions('sm'));
        modalRef.componentInstance.config = {
            title: this.translateBase.instant('common.confirm'),
            description: this.translateBase.instant('user_page.confirm_user_desc'),
            buttonConfirm: this.translateBase.instant('common.ok'),
            buttonDecline: this.translateBase.instant('common.cancel'),
        };

        try {
            const result = await modalRef.result;
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

        const modalRef = this.modalServiceBase.open(ConfirmModal, this.getModalOptions('sm'));
        modalRef.componentInstance.config = {
            title: this.translateBase.instant('common.confirm'),
            description: isBlocking
                ? this.translateBase.instant('users.confirm_block')
                : this.translateBase.instant('users.confirm_unblock'),
            buttonConfirm: this.translateBase.instant('common.ok'),
            buttonDecline: this.translateBase.instant('common.cancel'),
            buttonError: this.translateBase.instant('common.ok'),
            showConfirmButton: !isBlocking,
            showErrorButton: isBlocking,
        };

        try {
            const result = await modalRef.result;
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
