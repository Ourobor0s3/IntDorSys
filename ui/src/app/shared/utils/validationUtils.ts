import { RegexUtils } from "./regexUtils";
import { IResponse } from "../interface/response";

const ERROR_REQUIRED = 'errors.required';

export class ValidationUtils {

    public static validatePasswordConfirmation(
        password: string,
        confirmation: string,
    ): IResponse<null> {
        if (RegexUtils.isNullOrWhitespace(confirmation)) {
            return {
                isSuccess: false,
                errors: [{ message: ERROR_REQUIRED }],
                data: null,
            };
        }

        if (password !== confirmation) {
            return {
                isSuccess: false,
                errors: [{ message: 'errors.passwordConfirmation.notMatch' }],
                data: null,
            };
        }

        return {
            isSuccess: true,
            errors: [],
            data: null,
        };
    }
}
