import { Component, Input } from '@angular/core';
import { HeaderButtonModel } from "../../model/headerButton.model";

@Component({
    selector: 'app-section-header',
    templateUrl: './section-header.component.html',

})
export class SectionHeaderComponent {

    @Input()
    public title: string | undefined;

    @Input()
    public firstButton: HeaderButtonModel = new HeaderButtonModel();
    @Input()
    public secondButton: HeaderButtonModel = new HeaderButtonModel();
    @Input()
    public showInput: boolean = false;

}
