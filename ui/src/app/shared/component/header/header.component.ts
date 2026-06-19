import { Component, OnDestroy, OnInit, Renderer2 } from '@angular/core';
import { NavigationEnd, Router } from '@angular/router';
import { AuthService } from 'src/app/shared/services/auth.service';
import { overviewRoute } from "../../constants/routes";
import { BaseComponent } from '../base/base.component';
import { Language } from '../../enums/language';
import { LanguageInfo, languages } from '../../constants/languages';
import { TranslateService } from '@ngx-translate/core';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { EventService } from '../../services/event.service';
import { ThemeService } from '../../services/theme.service';
import { UserInfoModel } from "../../interface/userInfo.model";
import { UserService } from "../../services/user.service";
import { DataReloadService } from "../../services/dataReload.service";
import { NavService, Page } from "../../services/nav.service";

@Component({
    selector: 'app-header',
    templateUrl: './header.component.html',

})
export class HeaderComponent extends BaseComponent implements OnInit, OnDestroy {
    currentLang: string = languages[Language.EN].shortName;
    currentLanguageInfo: LanguageInfo = languages[Language.EN];
    user: UserInfoModel = {} as UserInfoModel;
    isMobileMenuOpen = false;
    isDarkMode = false;
    protected menuItems: Page[] | undefined;
    private destroy$ = new Subject<void>();

    constructor(
        private authService: AuthService,
        protected router: Router,
        private translate: TranslateService,
        private dataReloadService: DataReloadService,
        private userService: UserService,
        private navService: NavService,
        private eventService: EventService,
        private themeService: ThemeService,
        private renderer: Renderer2,
    ) {
        super(renderer);
        this.user = this.userService.get() ?? ({} as UserInfoModel);
    }

    ngOnInit(): void {
        this.currentLang = this.translate.currentLang;
        this.updateCurrentLanguageInfo();
        this.isDarkMode = this.themeService.getTheme() === 'dark';

        this.navService.mainItems
            .pipe(takeUntil(this.destroy$))
            .subscribe(menuItems => {
                this.menuItems = menuItems;
            });

        this.router.events
            .pipe(takeUntil(this.destroy$))
            .subscribe((event) => {
                if (event instanceof NavigationEnd) {
                    this.updateActiveTabs(event.url);
                    if (event.url != this.navService.currentUrl) {
                        this.navService.previousUrl = this.navService.currentUrl;
                        this.navService.currentUrl = event.url;
                    }
                }
            });

        this.translate.onLangChange
            .pipe(takeUntil(this.destroy$))
            .subscribe(() => {
                this.currentLang = this.translate.currentLang;
                this.updateCurrentLanguageInfo();
            });

        this.dataReloadService.dataReload$
            .pipe(takeUntil(this.destroy$))
            .subscribe(() => {
                this.loadData();
            });

        this.loadData();
    }

    ngOnDestroy() {
        this.destroy$.next();
        this.destroy$.complete();
    }

    needHidden(navRoles: string[] | undefined): boolean {
        if (!navRoles || navRoles.length === 0) return false;
        const userRole = this.authService.authData?.role;
        if (!userRole) return true;
        return !navRoles.includes(userRole);
    }

    toggletNavActive(item: Page): void {
        if (item.path === this.router.url) {
            return;
        }

        this.menuItems?.forEach(a => {
            if (this.menuItems?.includes(item)) {
                a.active = false;
            }
            if (a.children) {
                a.children.forEach(b => {
                    if (a.children?.includes(item)) {
                        b.active = false;
                    }
                });
            }
        });
        item.active = !item.active;
    }

    updateActiveTabs(url: string) {
        if (!this.menuItems) return;
        this.navService.updateActiveTabs(this.menuItems, url);
    }

    async loadData() {
        this.setLoading(true);
        try {
            this.user = (await this.userService.refreshUser()) ?? ({} as UserInfoModel);
        } finally {
            this.setLoading(false);
        }
    }

    onLogout(): void {
        this.authService.logout();
    }

    goOverview() {
        this.router.navigateByUrl(overviewRoute);
    }

    isLoggedIn(): boolean {
        return this.authService.isLoggedIn();
    }

    toggleLanguage(): void {
        const langCodes = Object.values(languages).map(l => l.shortName);
        const currentIndex = langCodes.indexOf(this.currentLang);
        const nextLang = langCodes[(currentIndex + 1) % langCodes.length];
        this.currentLang = nextLang;
        this.translate.use(nextLang);
        localStorage.setItem('localization', nextLang);
        this.eventService.langChanged(nextLang);
    }

    toggleMobileMenu() {
        this.isMobileMenuOpen = !this.isMobileMenuOpen;
        if (this.isMobileMenuOpen) {
            this.renderer.setStyle(document.body, 'overflow', 'hidden');
        } else {
            this.renderer.removeStyle(document.body, 'overflow');
        }
    }

    private updateCurrentLanguageInfo(): void {
        const langInfo = Object.values(languages).find(lang => lang.shortName === this.currentLang);
        if (langInfo) {
            this.currentLanguageInfo = langInfo;
        }
    }

    toggleTheme() {
        this.themeService.toggle();
        this.isDarkMode = !this.isDarkMode;
    }

    trackByMenuItem(index: number, item: Page): string {
        return item.path || String(index);
    }

}
