import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from 'src/app/shared/services/auth.service';
import { authRoute, overviewRoute, registerRoute } from "../../shared/constants/routes";
import { FormBuilder, FormGroup, Validators } from "@angular/forms";
import { BaseComponent } from "../../shared/component/base/base.component";
import { Credentials } from "../../shared/services/token.service";
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
        private translate: TranslateService,
    ) {
        super();
        this.loginForm = fb.group({
            login: ['', [Validators.required]],
            password: ['', [Validators.required]],
        });
    }

    async onSubmit(): Promise<void> {
        if (this.loginForm.invalid) {
            this.markFormGroupTouchedAndDirty(this.loginForm);
            return;
        }
        this.setLoading(true);

        let loginCred: Credentials = {
            login: this.loginForm.get('login')!.value,
            password: this.loginForm.get('password')!.value,
        };

        try {
            const res = await this.authService.login(loginCred);
            if (res.isSuccess) {
                this.navigateTab(overviewRoute);
            } else if (res.errors && res.errors.length > 0) {
                this.showError(res.errors[0].message);
            } else {
                this.showResponseError(res);
            }
        } catch (e) {
            this.showError(this.translate.instant('common.system_error'));
        } finally {
            this.setLoading(false);
        }
    }

    navigateToReg(): void {
        this.navigateTab(authRoute + "/" + registerRoute);
    }

    navigateTab(link: string): void {
        this.router.navigateByUrl('/' + link);
    }

    hideOrShowPassword(): void {
        this.showPass = !this.showPass;
    }


}
