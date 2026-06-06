import { Component } from '@angular/core';

@Component({
    selector: 'app-footer',
    templateUrl: './footer.component.html',
    styleUrls: ['./footer.component.scss'],
})
export class FooterComponent {
    currentYear: number;

    constructor() {
        let t = this;
        t.currentYear = new Date().getFullYear();
    }
}
