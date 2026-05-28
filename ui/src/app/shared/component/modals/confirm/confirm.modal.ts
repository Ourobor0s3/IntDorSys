import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { Component, Input, OnDestroy, OnInit } from '@angular/core';
import { DomSanitizer, SafeHtml } from '@angular/platform-browser';

@Component({
    selector: 'app-confirm-modal',
    templateUrl: 'confirm.modal.html',
    styleUrls: ['./confirm.modal.scss'],
})
export class ConfirmModal implements OnInit, OnDestroy {
    @Input() title: string = '';
    @Input() description: string = '';
    @Input() showDescription: boolean = true;
    @Input() showTitle: boolean = true;
    @Input() buttonConfirm: string = '';
    @Input() showConfirmButton: boolean = true;
    @Input() buttonDecline: string = '';
    @Input() showDeclineButton: boolean = true;
    @Input() buttonError: string = '';
    @Input() showErrorButton: boolean = false;
    public descriptionHTML: SafeHtml | undefined;

    constructor(
        public activeModal: NgbActiveModal,
        private sanitizer: DomSanitizer,
    ) {
    }

    ngOnInit() {
        this.descriptionHTML = this.sanitizer.bypassSecurityTrustHtml(this.description);
        document.body.classList.add('modal-open-scroll-lock');
    }

    ngOnDestroy(): void {
        document.body.classList.remove('modal-open-scroll-lock');
    }

    public isConfirm(resp: boolean) {
        this.activeModal.close(resp);
    }
}
