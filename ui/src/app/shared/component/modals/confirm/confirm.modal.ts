import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { Component, Input, OnDestroy, OnInit, Renderer2, SecurityContext } from '@angular/core';
import { DomSanitizer } from '@angular/platform-browser';

export interface ConfirmModalConfig {
    title: string;
    description: string;
    buttonConfirm?: string;
    buttonDecline?: string;
    buttonError?: string;
    showConfirmButton?: boolean;
    showErrorButton?: boolean;
    showTitle?: boolean;
    showDescription?: boolean;
    showDeclineButton?: boolean;
}

@Component({
    selector: 'app-confirm-modal',
    templateUrl: 'confirm.modal.html',

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

    @Input() set config(value: ConfirmModalConfig | undefined) {
        if (!value) return;
        this.title = value.title;
        this.description = value.description;
        if (value.buttonConfirm !== undefined) this.buttonConfirm = value.buttonConfirm;
        if (value.buttonDecline !== undefined) this.buttonDecline = value.buttonDecline;
        if (value.buttonError !== undefined) this.buttonError = value.buttonError;
        if (value.showConfirmButton !== undefined) this.showConfirmButton = value.showConfirmButton;
        if (value.showErrorButton !== undefined) this.showErrorButton = value.showErrorButton;
        if (value.showTitle !== undefined) this.showTitle = value.showTitle;
        if (value.showDescription !== undefined) this.showDescription = value.showDescription;
        if (value.showDeclineButton !== undefined) this.showDeclineButton = value.showDeclineButton;
    }

    public descriptionHTML: string | undefined;

    constructor(
        public activeModal: NgbActiveModal,
        private sanitizer: DomSanitizer,
        private renderer: Renderer2,
    ) {
    }

    ngOnInit() {
        this.descriptionHTML = this.sanitizer.sanitize(SecurityContext.HTML, this.description) ?? '';
        this.renderer.addClass(document.body, 'modal-open-scroll-lock');
    }

    ngOnDestroy(): void {
        this.renderer.removeClass(document.body, 'modal-open-scroll-lock');
    }

    public isConfirm(resp: boolean) {
        this.activeModal.close(resp);
    }
}
