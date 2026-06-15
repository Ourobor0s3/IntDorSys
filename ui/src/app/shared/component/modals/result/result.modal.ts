import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { Component, Input, OnDestroy, OnInit, Renderer2 } from '@angular/core';
import { DomSanitizer, SafeHtml } from '@angular/platform-browser';

@Component({
    selector: 'app-result-modal',
    templateUrl: 'result.modal.html',

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
        private renderer: Renderer2,
    ) {
    }

    ngOnInit() {
        this.descriptionHTML = this.sanitizer.bypassSecurityTrustHtml(this.description);
        this.renderer.addClass(document.body, 'modal-open-scroll-lock');
    }

    ngOnDestroy(): void {
        this.renderer.removeClass(document.body, 'modal-open-scroll-lock');
    }

    public closeModal(resp: boolean) {
        this.activeModal.close(resp);
    }
}
