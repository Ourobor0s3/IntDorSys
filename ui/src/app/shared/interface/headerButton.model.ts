export interface HeaderButtonModel {
    funcName?: string;
    itemTitle: string;
    buttonTitle: string;
    headerTitle?: string;
    needShow?: boolean;
    className?: string;
    buttonNumber?: number;
    url?: string;
}

export const defaultHeaderButton = (): HeaderButtonModel => ({
    funcName: '',
    itemTitle: '',
    buttonTitle: '',
    headerTitle: '',
    needShow: true,
    className: '',
    buttonNumber: 0,
});
