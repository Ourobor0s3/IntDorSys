import { Pipe, PipeTransform } from '@angular/core';
import { AbstractControl } from '@angular/forms';

@Pipe({
    name: 'errorMessage',
})
export class ErrorMessagePipe implements PipeTransform {
    transform(elem: AbstractControl, passwordPattern?: RegExp | null): string {
        if (!elem.dirty || elem.untouched || !elem.invalid) return '';

        const customError = Object.getOwnPropertyNames(elem.errors);

        if (elem.errors.required) return 'errors.required';
        if (elem.errors.max != undefined) return 'errors.max';
        if (elem.errors.min != undefined) return 'errors.min';
        if (elem.errors.maxlength != undefined) return 'errors.maxLength';
        if (elem.errors.minlength != undefined) return 'errors.minLength';
        if (elem.errors.pattern != undefined && passwordPattern != null) return 'errors.passwordPattern';
        if (elem.errors.email != undefined) return 'errors.invalidEmail';
        if (elem.errors.mismatch != undefined) return 'errors.mismatch';
        if (customError.length > 0) return customError[0];

        return '';
    }
}
