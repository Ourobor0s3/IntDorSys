import { Component, OnDestroy, OnInit } from '@angular/core';
import { NavigationEnd, Router } from '@angular/router';
import { AuthService } from 'src/app/shared/services/auth.service';
import { overviewRoute } from "../../constants/routes";
import { BaseComponent } from '../base/base.component';
import { Language } from '../../enums/language';
import { LanguageInfo, languages } from '../../constants/languages';
import { TranslateService } from '@ngx-translate/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { EventService } from '../../services/event.service';
import { UserInfoModel } from "../../model/userInfo.model";
import { UserService } from "../../services/user.service";
import { DataReloadService } from "../../services/dataReload.service";
import { NavService, Page } from "../../services/nav.service";

@Component({
    selector: 'app-header',
    templateUrl: './header.component.html',
    styleUrls: ['./header.component.scss'],
})
export class HeaderComponent extends BaseComponent implements OnInit, OnDestroy {
    currentLang: string = languages[Language.EN].shortName;
    availableLanguages: LanguageInfo[] = Object.values(languages);
    languages = languages;
    currentLanguageInfo: LanguageInfo = languages[Language.EN];
    getUser: () => UserInfoModel;
    user: UserInfoModel = new UserInfoModel();
    isMobileMenuOpen = false;
    protected menuItems: Page[] | undefined;
    private destroy$ = new Subject<void>();

    constructor(
        private authService: AuthService,
        protected router: Router,
        private translate: TranslateService,
        private dataReloadService: DataReloadService,
        private modalService: NgbModal,
        private userService: UserService,
        private navService: NavService,
        private eventService: EventService,
    ) {
        super(translate, modalService);
        const t = this;
        t.initializeLanguage();
        t.getUser = () => this.userService.get() ?? new UserInfoModel();

        let items = t.navService.mainItems;
        items.subscribe(menuItems => {
            this.menuItems = menuItems;
            this.router.events.subscribe((event) => {
                if (event instanceof NavigationEnd) {
                    t.updateActiveTabs(event.url);
                    if (event.url != t.navService.currentUrl) {
                        t.navService.previousUrl = t.navService.currentUrl;
                        t.navService.currentUrl = event.url;
                    }
                }
            })
        });
    }

    ngOnInit(): void {
        // Subscribe to language changes
        this.translate.onLangChange
            .pipe(takeUntil(this.destroy$))
            .subscribe(() => {
                this.currentLang = this.translate.currentLang;
                this.updateCurrentLanguageInfo();
            });

        this.loadData();
        this.dataReloadService.dataReload$
            .pipe(takeUntil(this.destroy$))
            .subscribe(() => {
                this.loadData();
            });
    }

    ngOnDestroy() {
        this.destroy$.next();
        this.destroy$.complete();
    }

    needHidden(navRoles: string[] | undefined) {
        var t = this;
        return false;
    }

    toggletNavActive(item: any) {
        let t = this;
        if (item.path == t.router.url) {
            return;
        }

        t.menuItems!.forEach((a): any => {
            if (t.menuItems!.includes(item))
                a.active = false
            if (!a.children) return false
            a.children.forEach(b => {
                if (a.children!.includes(item)) {
                    b.active = false
                }
            })
        });
        item.active = !item.active;
    }

    updateActiveTabs(url: string) {
        let t = this;
        //ищем выбранный элемент среди menuItems
        let curItem = t.menuItems!.find(x => x.path == url && x.type == 'link');
        if (!curItem) {
            //если выбранного элемента нет среди menuItems - смотрим детей
            t.menuItems!.forEach(items => {
                if (!!items?.children && !curItem) {
                    curItem = items.children.find(x => x.path === url);
                }
            })
        }
        //возврат, если выбранного эл-та нет среди menuItems и детей
        if (!curItem)
            return;

        curItem.active = true;
        //проходимся по всем эл-там, которые не являются выбранным
        t.menuItems!.filter(x => x.path != curItem!.path).forEach(menuItem => {
            //делаем эл-т активным, если у среди его детей есть выбранный
            menuItem.active = !!menuItem.children?.find(x => x.path === url);
            if (!!menuItem.children) {
                //проходимся по всем дочерним не выбранным эл-там и проставляем им false
                menuItem.children.filter(x => x.path != curItem!.path).forEach(child => {
                    child.active = false;
                });
            }
        });
    }

    loadData() {
        let t = this;
        t.setLoading(true);
        Promise.all([t.refreshUser()])
            .finally(() => {
                t.setLoading(false);
            });
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

    changeLanguage(langCode: string): void {
        if (this.currentLang !== langCode) {
            this.currentLang = langCode;
            this.translate.use(langCode);
            localStorage.setItem('localization', langCode);
            // Emit language change event
            this.eventService.Langchanged(langCode);
        }
    }

    toggleMobileMenu() {
        this.isMobileMenuOpen = !this.isMobileMenuOpen;
        document.body.style.overflow = this.isMobileMenuOpen ? 'hidden' : '';
    }

    private initializeLanguage(): void {
        // Set default language
        this.translate.setDefaultLang(languages[Language.EN].shortName);

        // Get stored language or use default
        const storedLang = localStorage.getItem('localization');
        this.currentLang = storedLang || languages[Language.EN].shortName;

        // Validate stored language
        if (!Object.values(languages).some(lang => lang.shortName === this.currentLang)) {
            this.currentLang = languages[Language.EN].shortName;
        }

        // Set current language
        this.translate.use(this.currentLang);
        localStorage.setItem('localization', this.currentLang);

        // Update current language info
        this.updateCurrentLanguageInfo();

        // Emit initial language
        this.eventService.Langchanged(this.currentLang);
    }

    private updateCurrentLanguageInfo(): void {
        const langInfo = Object.values(languages).find(lang => lang.shortName === this.currentLang);
        if (langInfo) {
            this.currentLanguageInfo = langInfo;
        }
    }

    private async refreshUser() {
        let t = this;
        t.user = await t.userService.refreshUser();
    }
}
