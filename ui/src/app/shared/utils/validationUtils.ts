import { RegexUtils } from "./regexUtils";

const ERROR_REQUIRED = 'errors.required';

export class ValidationUtils {

    /** валидация подтверждения пароля */
    public static validatePasswordConfirmation(
        password: string,
        confirmation: string,
    ): IValidationResult {
        if (RegexUtils.isNullOrWhitespace(confirmation)) {
            return {
                isValid: false,
                errorText: ERROR_REQUIRED,
            };
        }

        if (password !== confirmation) {
            return {
                isValid: false,
                errorText: 'errors.passwordConfirmation.notMatch',
            };
        }

        return {
            isValid: true,
            errorText: '',
        };
    }
}

/**
 * Результат валидации
 */
export interface IValidationResult {
    /**
     * Валидно/невалидно
     */
    isValid: boolean;
    /**
     * Текст сообщения об ошибке
     */
    errorText: string;
}
