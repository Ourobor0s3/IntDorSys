import { Component, EventEmitter, Input, Output } from '@angular/core';
import { HeaderButtonModel, defaultHeaderButton } from "../../interface/headerButton.model";

@Component({
    selector: 'app-section-header',
    templateUrl: './section-header.component.html',

})
export class SectionHeaderComponent {

    @Input()
    public title: string | undefined;

    @Input()
    public button: HeaderButtonModel = defaultHeaderButton();

    @Input()
    public showInput: boolean = false;

    @Output()
    public buttonClick = new EventEmitter<void>();

}
