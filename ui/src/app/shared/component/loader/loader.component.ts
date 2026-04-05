import { Component } from '@angular/core';

@Component({
    selector: 'app-loader',
    templateUrl: './loader.component.html',
    styleUrls: ['./loader.component.scss'],
})
export class LoaderComponent {
    constructor() {
    }

    public static setLoading(isLoading: boolean) {
        const preloader = document.getElementById('preloader');

        if (preloader) {
            preloader.style.display = isLoading ? 'flex' : 'none';
        }
    }
}
