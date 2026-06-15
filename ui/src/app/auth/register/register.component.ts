import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { authRoute, loginRoute, registerRoute } from "../../shared/constants/routes";
import { FormBuilder, FormGroup, Validators } from "@angular/forms";
import { BaseComponent } from "../../shared/component/base/base.component";
import { NgbModal } from "@ng-bootstrap/ng-bootstrap";
import { TranslateService } from "@ngx-translate/core";
import { AccountService, IRegister } from "../../shared/services/account.service";
import { ValidationUtils } from "../../shared/utils/validationUtils";

@Component({
    selector: 'app-register',
    templateUrl: './register.component.html',

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
        this.regForm = fb.group(
            {
                telegram: ['', [Validators.required]],
                email: ['', [Validators.required, Validators.email, Validators.maxLength(50)]],
                password: ['', [Validators.required, Validators.minLength(8), Validators.pattern(this.passwordPattern)]],
                passwordConfirm: ['', [Validators.required]],
            },
            {
                validator: [this.validatePassword.bind(this)],
            });
    }

    get telegram() {
        return this.regForm.get('telegram')!;
    }

    get email() {
        return this.regForm.get('email')!;
    }

    get password() {
        return this.regForm.get('password')!;
    }

    get confirmPassword() {
        return this.regForm.get('passwordConfirm')!;
    }

    createAccount(): void {
        if (this.regForm.invalid) {
            this.markFormGroupTouchedAndDirty(this.regForm);
            return;
        }

        this.setLoading(true);
        let rawTelegram: string = this.telegram.value;
        let parsedTelegram: number | null = rawTelegram ? Number(rawTelegram) : null;
        if (rawTelegram && isNaN(parsedTelegram!)) {
            this.showError(this.translate.instant('errors.telegramIdMustBeNumber'));
            this.setLoading(false);
            return;
        }
        let regCreds: IRegister = {
            telegramId: parsedTelegram,
            email: this.email.value,
            password: this.password.value,
        };

        this.accountService.register(regCreds)
            .then((res) => {
                if (res.isSuccess) {
                    this.navigateToLogin();
                    this.showSuccess(this.translate.instant('registered_success'));
                } else {
                    this.showResponseError(res);
                }
            })
            .catch((e) => {
                this.showResponseError(e);
            })
            .finally(() => {
                this.setLoading(false);
            });
    }

    validatePassword(g: FormGroup): void {
        let password = g.get('password')!;
        if (password.dirty || !!password.value) {
            password.markAsDirty();
            password.markAsTouched();
        }

        let passwordConfirm = g.get('passwordConfirm')!;
        if (passwordConfirm.dirty || !!passwordConfirm.value) {
            passwordConfirm.markAsDirty();
            passwordConfirm.markAsTouched();
            const isValidPasswordConfirm = ValidationUtils.validatePasswordConfirmation(
                password.value,
                passwordConfirm.value,
            );
            if (!isValidPasswordConfirm.isSuccess) {
                let obj = {};
                obj[isValidPasswordConfirm.errors[0].message] = true;
                passwordConfirm.setErrors(obj);
            }
            // обновить валидацию данного элемента при совпадении паролей
            else passwordConfirm.updateValueAndValidity({ onlySelf: true });
        }
    }

    navigateToLogin(): void {
        this.navigateTab(authRoute + "/" + loginRoute);
    }

    navigateTab(link: string): void {
        this.router.navigateByUrl('/' + link);
    }

    hideOrShowPassword(isConfirmField: boolean = false): void {
        if (isConfirmField) {
            this.showConfirmPass = !this.showConfirmPass;
            return;
        }
        this.showPass = !this.showPass;
    }

}
