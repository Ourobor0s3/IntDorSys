export interface ModalInfoModel {
    title: string;
    description: string;
    showDescription: boolean;
    showTitle: boolean;
    buttonConfirm: string;
    buttonDecline: string;
    buttonDeclineFontSize: string;
    buttonError: string;
    showConfirmButton: boolean;
    showDeclineButton: boolean;
    showErrorButton: boolean;
    showInput: boolean;
    inputLabel: string;
    inputPlaceholder: string;
    inputValue: string;
    agreementText: string;
    showAgreement: boolean;
}

export const defaultModalInfo = (): ModalInfoModel => ({
    title: '',
    description: '',
    showDescription: true,
    showTitle: true,
    buttonConfirm: 'OK',
    buttonDecline: 'Cancel',
    buttonDeclineFontSize: '15px',
    buttonError: 'OK',
    showConfirmButton: true,
    showDeclineButton: true,
    showErrorButton: false,
    showInput: false,
    inputLabel: '',
    inputPlaceholder: '',
    inputValue: '',
    agreementText: '',
    showAgreement: false,
});
