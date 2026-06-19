import { Component, OnDestroy, OnInit } from '@angular/core';
import { BaseComponent } from 'src/app/shared/component/base/base.component';
import { ActivatedRoute, Router } from '@angular/router';
import { authSubpages, Subpages } from './auth.subpages';
import { transition, trigger, useAnimation } from '@angular/animations';
import { fadeIn } from 'ng-animate';
import { authRoute } from '../shared/constants/routes';
import { TranslateService } from '@ngx-translate/core';
import { Language } from '../shared/enums/language';
import { languages } from '../shared/constants/languages';
import { ThemeService } from '../shared/services/theme.service';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';

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
export class AuthComponent extends BaseComponent implements OnInit, OnDestroy {
    subpagesMenu: Subpages[] = authSubpages;
    currentActiveTab: Subpages | undefined;
    currentYear: number = new Date().getFullYear();
    isDarkMode = false;
    currentLang: string = languages[Language.EN].shortName;
    private destroy$ = new Subject<void>();

    constructor(
        private activateRoute: ActivatedRoute,
        private router: Router,
        private translate: TranslateService,
        private themeService: ThemeService,
    ) {
        super();
    }

    ngOnInit(): void {
        this.currentLang = this.translate.currentLang;
        this.isDarkMode = this.themeService.getTheme() === 'dark';

        this.activateRoute.params.pipe(takeUntil(this.destroy$)).subscribe(params => {
            const subpageRoute = params['subpageRoute'];
            const found = this.subpagesMenu.find((tab) => tab.route == subpageRoute);

            if (!subpageRoute || !found) {
                this.navigateTab(this.subpagesMenu[0].route);
                return;
            }

            if (this.currentActiveTab) {
                this.currentActiveTab.isActive = false;
            }

            this.currentActiveTab = found;
            found.isActive = true;
        });
    }

    changeLanguage(): void {
        this.currentLang = this.currentLang === languages[Language.EN].shortName
            ? languages[Language.RU].shortName
            : languages[Language.EN].shortName;
        this.translate.use(this.currentLang);
        localStorage.setItem('localization', this.currentLang);
    }

    toggleTheme(): void {
        this.themeService.toggle();
        this.isDarkMode = this.themeService.getTheme() === 'dark';
    }

    ngOnDestroy(): void {
        this.destroy$.next();
        this.destroy$.complete();

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
