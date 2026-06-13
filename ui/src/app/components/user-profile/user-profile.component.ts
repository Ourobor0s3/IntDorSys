import { Component, OnDestroy, OnInit } from '@angular/core';
import { BaseComponent } from 'src/app/shared/component/base/base.component';
import { ActivatedRoute, Router } from '@angular/router';
import { Subject, takeUntil } from "rxjs";
import { UserInfoModel } from "../../shared/model/userInfo.model";
import { UsersInfoService } from "../../shared/services/user-info.service";
import { NgbModal } from "@ng-bootstrap/ng-bootstrap";
import { TranslateService } from '@ngx-translate/core';
import { LoadingService } from "../../shared/services/loading.service";
import { UserStatus } from "../../shared/enums/UserStatus";
import { USER_STATUS_STYLES } from "../../shared/constants/statusStyle";

@Component({
    selector: 'app-user-profile',
    templateUrl: './user-profile.component.html',
    styleUrls: ['./user-profile.component.scss'],
})
export class UserProfileComponent extends BaseComponent implements OnInit, OnDestroy {
    user?: UserInfoModel;
    userId: number = 0;
    private destroy$ = new Subject<void>();

    constructor(
        private route: ActivatedRoute,
        private router: Router,
        private userService: UsersInfoService,
        private modal: NgbModal,
        private translate: TranslateService,
        private loading: LoadingService,
    ) {
        super(translate, modal, loading);
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

    ngOnDestroy(): void {
        this.destroy$.next();
        this.destroy$.complete();
    }
}
