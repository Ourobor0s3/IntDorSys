import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { Component, Input, OnDestroy, OnInit } from '@angular/core';
import { DomSanitizer, SafeHtml } from '@angular/platform-browser';

@Component({
    selector: 'app-result-modal',
    templateUrl: 'result.modal.html',
    styleUrls: ['./result.modal.scss'],
})
export class ResultModal implements OnInit, OnDestroy {

    @Input() title: string = 'System';
    @Input() description: string = '';
    @Input() showDescription: boolean = true;
    @Input() imgUrl: string = '';
    @Input() buttonClassList = 'btn-primary';
    @Input() maxWidth: string = '360px';
    public descriptionHTML: SafeHtml | undefined;

    constructor(
        public activeModal: NgbActiveModal,
        private sanitizer: DomSanitizer,
    ) {
    }

    ngOnInit() {
        this.descriptionHTML = this.sanitizer.bypassSecurityTrustHtml(this.description);
        document.addEventListener('touchmove', this.listener);
    }

    ngOnDestroy(): void {
        document.removeEventListener('touchmove', this.listener);
    }

    public closeModal(resp: boolean) {
        this.activeModal.close(resp);
    }

    private listener = (event: Event) => {
        event.preventDefault(), { passive: false }
    };
}
