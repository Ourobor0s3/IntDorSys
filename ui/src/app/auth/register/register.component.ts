import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { authRoute, loginRoute, registerRoute } from "../../shared/constants/routes";
import { FormBuilder, FormGroup, Validators } from "@angular/forms";
import { BaseComponent } from "../../shared/component/base/base.component";
import { NgbModal } from "@ng-bootstrap/ng-bootstrap";
import { TranslateService } from "@ngx-translate/core";
import { AccountService } from "../../shared/services/account.service";
import { ValidationUtils } from "../../shared/utils/validationUtils";

@Component({
    selector: 'app-register',
    templateUrl: './register.component.html',
    styleUrls: ['./register.component.scss'],
})
export class RegisterComponent extends BaseComponent {
    regForm: FormGroup = new FormGroup({});
    showPass: boolean = false;
    showConfirmPass: boolean = false;
    protected readonly registerRoute = registerRoute;

    constructor(
        private fb: FormBuilder,
        private accountService: AccountService,
        private router: Router,
        private modal: NgbModal,
        private translate: TranslateService,
    ) {
        super(translate, modal);
        let t = this;
        t.regForm = fb.group(
            {
                telegram: ['', [Validators.required]],
                email: ['', [Validators.required, Validators.email, Validators.maxLength(50)]],
                password: ['', [Validators.required, Validators.pattern(t.passwordPattern)]],
                passwordConfirm: ['', [Validators.required]],
            },
            {
                validator: [t.validatePassword.bind(this)],
            });
    }

    get telegram() {
        return this.regForm.get('telegram');
    }

    get email() {
        return this.regForm.get('email');
    }

    get password() {
        return this.regForm.get('password');
    }

    get confirmPassword() {
        return this.regForm.get('passwordConfirm');
    }

    createAccount(): void {
        const t = this;

        if (t.regForm.invalid) {
            t.markFormGroupTouchedAndDirty(t.regForm);
            return;
        }

        t.setLoading(true);
        let regCreds = {
            telegramId: t.telegram.value,
            email: t.email.value,
            password: t.password.value,
        };

        t.accountService.register(regCreds)
            .then((res) => {
                if (res.isSuccess) {
                    t.navigateToLogin();
                    t.showSuccess("Successfully registered account");
                } else {
                    t.showResponseError(res);
                }
            })
            .catch((e) => {
                t.showResponseError(e);
            })
            .finally(() => {
                t.setLoading(false);
            });
    }

    validatePassword(g: FormGroup) {
        let password = g.get('password');
        if (password.dirty || !!password.value) {
            password.markAsDirty();
            password.markAsTouched();
        }

        let passwordConfirm = g.get('passwordConfirm');
        if (passwordConfirm.dirty || !!passwordConfirm.value) {
            passwordConfirm.markAsDirty();
            passwordConfirm.markAsTouched();
            var isValidPasswordConfirm = ValidationUtils.validatePasswordConfirmation(
                password.value,
                passwordConfirm.value,
            );
            if (!isValidPasswordConfirm.isValid) {
                let obj = {};
                obj[isValidPasswordConfirm.errorText] = true;
                passwordConfirm.setErrors(obj);
            }
            // обновить валидацию данного элемента при совпадении паролей
            else passwordConfirm.updateValueAndValidity({ onlySelf: true });
        }
    }

    navigateToLogin() {
        this.navigateTab(authRoute + "/" + loginRoute);
    }

    navigateTab(link: string) {
        this.router.navigateByUrl('/' + link);
    }

    hideOrShowPassword(isConfirmField: boolean = false) {
        if (isConfirmField) {
            this.showConfirmPass = !this.showConfirmPass;
            return;
        }
        this.showPass = !this.showPass;
    }

}
