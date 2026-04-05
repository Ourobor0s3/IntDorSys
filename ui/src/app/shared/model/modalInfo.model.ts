export class ModalInfoModel {
    public title: string = '';
    public description: string = '';
    public showDescription: boolean = true;
    public showTitle: boolean = true;
    public buttonConfirm: string = 'OK';
    public buttonDecline: string = 'Cancel';
    public buttonDeclineFontSize: string = '15px';
    public buttonError: string = 'OK';
    public showConfirmButton: boolean = true;
    public showDeclineButton: boolean = true;
    public showErrorButton: boolean = false;
    public showInput: boolean = false;
    public inputLabel: string = '';
    public inputPlaceholder: string = '';
    public inputValue: string = '';
    public agreementText: string = '';
    public showAgreement: boolean = false;
}
