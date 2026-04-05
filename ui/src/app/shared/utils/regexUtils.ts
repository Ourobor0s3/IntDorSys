export class RegexUtils {
    /**
     * Проверка является ли строка адресом электронной почты
     */
    public static isValidEmail(val: string): boolean {
        return /^[a-zA-Z0-9.!#$%&'*+/=?^_`{|}~-]+@[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?(?:\.[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?)*$/.test(
            val,
        );
    }

    /**
     * Проверка является ли строка корректным телефоном
     */
    public static isValidPhone(val: string): boolean {
        return /^[0-9]+$/.test(val);
    }

    /**
     * Проверка, содержит ли строка только кириллические символы
     */
    public static isCyrillic(val: string): boolean {
        return /^[а-яА-ЯёЁ\- ]*$/.test(val);
    }

    /**
     * Проверка является ли строка только из пробельных символов
     */
    public static isNullOrWhitespace(val: string): boolean {
        return val == null || /^\s*$/.test(val);
    }

    /** Проверка является ли строка валидным кодом подтверждения (состоит из 4 цифр) */
    public static isValidConfirmationCode(val: string): boolean {
        return /^\d{4}$/.test(val);
    }

    /**
     * Проверка, содержит ли строка только кириллические символы
     */
    public static isLetters(val: string): boolean {
        return /^[a-zA-Z0-9_]*$/.test(val);
    }

    /**
     * Dates dd.mm.yyyy
     * @param val - date
     */
    public static isDate(val: string): boolean {
        return /^\s*(3[01]|[12][0-9]|0?[1-9])\.(1[012]|0?[1-9])\.((?:19|20)\d{2})\s*$/.test(val);
    }

    /**
     * Проверка, содержит ли строка только число
     */
    public static isNumber(val: string): boolean {
        return /^[0-9,.*]+$/.test(val);
    }
}
