import { Component, OnInit } from '@angular/core';
import { BaseComponent } from 'src/app/shared/component/base/base.component';
import { ActivatedRoute, Router } from '@angular/router';
import { authSubpages, Subpages } from './auth.subpages';
import { transition, trigger, useAnimation } from '@angular/animations';
import { fadeIn } from 'ng-animate';
import { authRoute } from '../shared/constants/routes';
import { NgbModal } from "@ng-bootstrap/ng-bootstrap";
import { TranslateService } from '@ngx-translate/core';
import { Language } from '../shared/enums/language';
import { languages } from '../shared/constants/languages';
import { ThemeService } from '../shared/services/theme.service';

@Component({
    selector: 'app-auth',
    templateUrl: './auth.component.html',
    styleUrls: ['./auth.component.scss'],
    animations: [
        trigger('animateRoute', [
            transition('* <=> *', useAnimation(fadeIn)),
        ]),
    ],
})
export class AuthComponent extends BaseComponent implements OnInit {
    subpagesMenu: Subpages[] = authSubpages;
    currentActiveTab: Subpages | undefined;
    currentYear: number = new Date().getFullYear();
    isDarkMode = false;

    constructor(
        private activateRoute: ActivatedRoute,
        private router: Router,
        private modal: NgbModal,
        private translate: TranslateService,
        private themeService: ThemeService,
    ) {
        super(translate, modal);
        this.router.routeReuseStrategy.shouldReuseRoute = () => false;
        this.translate.setDefaultLang(languages[Language.EN].shortName);
    }

    ngOnInit(): void {
        this.isDarkMode = this.themeService.getTheme() === 'dark';
        const subpageRoute = this.activateRoute.snapshot.paramMap.get('subpageRoute');
        const found = this.subpagesMenu.find((tab) => tab.route == subpageRoute);

        if (!subpageRoute || !found) {
            this.navigateTab(this.subpagesMenu[0].route);
            return;
        }

        this.currentActiveTab = found;
        found.isActive = true;
    }

    toggleTheme(): void {
        this.themeService.toggle();
        this.isDarkMode = this.themeService.getTheme() === 'dark';
    }

    ngOnDestroy(): void {
        if (this.currentActiveTab) {
            this.currentActiveTab.isActive = false;
        }
    }

    navigateTab(route: string) {
        this.router.navigateByUrl('/' + authRoute + '/' + route);
    }

    getCurrentState(): number {
        return this.subpagesMenu.findIndex(page => page.isActive);
    }
}
