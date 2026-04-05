import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from 'src/app/shared/services/auth.service';
import { authRoute, overviewRoute, registerRoute } from "../../shared/constants/routes";
import { FormBuilder, FormGroup, Validators } from "@angular/forms";
import { BaseComponent } from "../../shared/component/base/base.component";
import { Credentials } from "../../shared/services/token.service";
import { NgbModal } from "@ng-bootstrap/ng-bootstrap";
import { TranslateService } from '@ngx-translate/core';

@Component({
    selector: 'app-login',
    templateUrl: './login.component.html',
    styleUrls: ['./login.component.scss'],
})
export class LoginComponent extends BaseComponent {
    loginForm: FormGroup = new FormGroup({});
    showPass: boolean = false;
    protected readonly registerRoute = registerRoute;

    constructor(
        private fb: FormBuilder,
        private authService: AuthService,
        private router: Router,
        private modal: NgbModal,
        private translate: TranslateService,
    ) {
        super(translate, modal);
        let t = this;
        t.loginForm = fb.group({
            email: ['', [Validators.required, Validators.email]],
            password: ['', [Validators.required]],
        });
    }

    get email() {
        return this.loginForm.get('email');
    }

    get password() {
        return this.loginForm.get('password');
    }

    onSubmit(): void {
        let t = this;
        if (t.loginForm.invalid) {
            t.markFormGroupTouchedAndDirty(t.loginForm);
            return;
        }
        t.setLoading(true);

        let loginCred: Credentials = {
            email: t.email!.value,
            password: t.password!.value,
        };

        t.authService.login(loginCred)
            .then(res => {
                if (res.errors[0] != null) {
                    t.showError(res.errors[0].message);
                } else {
                    t.navigateTab(overviewRoute);
                }
            })
            .catch((e) => {
                t.showError(t.translate.instant('common.system_error'));
            })
            .finally(() => {
                t.setLoading(false);
            });
    }

    navigateToReg() {
        this.navigateTab(authRoute + "/" + registerRoute);
    }

    navigateTab(link: string) {
        let t = this;
        t.router.navigateByUrl('/' + link);
    }

    hideOrShowPassword() {
        this.showPass = !this.showPass;
    }


}
