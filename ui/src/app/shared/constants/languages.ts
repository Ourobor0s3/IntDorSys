import { Language } from '../enums/language';

export interface LanguageInfo {
    shortName: string;
    fullName: string;
    flag: string;
}

export const languages: Record<Language, LanguageInfo> = {
    [Language.EN]: {
        shortName: 'en',
        fullName: 'English',
        flag: 'EN',
    },
    [Language.RU]: {
        shortName: 'ru',
        fullName: 'Русский',
        flag: 'RU',
    },
};
