import { Component, OnDestroy, Renderer2 } from '@angular/core';
import { ChildrenOutletContexts, NavigationEnd, Router } from "@angular/router";
import { NgbModalRef } from "@ng-bootstrap/ng-bootstrap";
import { filter, Subscription } from "rxjs";
import { BaseComponent } from "./shared/component/base/base.component";
import { TranslateService } from '@ngx-translate/core';
import { languages } from './shared/constants/languages';
import { Language } from './shared/enums/language';
import { animate, style, transition, trigger } from '@angular/animations';
import { AuthService } from "./shared/services/auth.service";
import { UserService } from "./shared/services/user.service";
import { EventService } from "./shared/services/event.service";
import { TimezoneService } from "./shared/services/timezone.service";
import { environment } from '../environments/environment';

interface StyleConfig {
    name: string;
    preload: boolean;
}

@Component({
    selector: 'app-root',
    templateUrl: './app.component.html',

    animations: [
        trigger('mainAnimation', [
            transition('* <=> *', [
                style({ opacity: 0 }),
                animate('300ms ease-in', style({ opacity: 1 })),
            ]),
        ]),
    ],
})
export class AppComponent extends BaseComponent implements OnDestroy {
    modalRef: NgbModalRef;
    private eventSubscription: Subscription | undefined;
    private readonly styleConfigs: StyleConfig[] = [
        { name: 'main', preload: false },
        { name: 'table', preload: false },
    ];

    constructor(
        private contexts: ChildrenOutletContexts,
        private router: Router,
        private translate: TranslateService,
        private authService: AuthService,
        private eventService: EventService,
        private userService: UserService,
        private renderer: Renderer2,
    ) {
        super(renderer);
        this.initializeApp();
    }

    ngOnDestroy(): void {
        if (this.eventSubscription) {
            this.eventSubscription.unsubscribe();
        }
    }

    getRouterOutletState(): string | undefined {
        return this.contexts.getContext('primary')?.route?.snapshot?.data?.['animation'];
    }

    private initializeApp(): void {
        this.initializeLanguage();
        this.timezoneService.init();

        this.styleConfigs.forEach(config => this.createStyle(config.name, config.preload));

        this.setupRouterEvents();
    }

    private initializeLanguage(): void {
        this.translate.addLangs([languages[Language.EN].shortName, languages[Language.RU].shortName]);
        this.translate.setDefaultLang(languages[Language.EN].shortName);

        const storedLang = localStorage.getItem('localization');
        const lang = storedLang && Object.values(languages).some(l => l.shortName === storedLang)
            ? storedLang
            : languages[Language.EN].shortName;

        this.translate.use(lang);
        localStorage.setItem('localization', lang);
        this.eventService.langChanged(lang);
    }

    private setupRouterEvents(): void {
        this.eventSubscription = this.router.events.pipe(
            filter(event => event instanceof NavigationEnd),
        ).subscribe(() => {
            if (this.authService.isLoggedIn()) {
                if (!this.userService.get() && !this.eventService.isFuncArrIncludes(this.userService.refreshUser)) {
                    this.userService.init();
                    this.eventService.addFuncToArrayOfIntervals(this.userService.refreshUser, 1000 * 60 * 5);
                }
            }
        });
    }

    private createStyle(styleName: string, preload = false): void {
        const link = this.renderer.createElement('link');
        this.renderer.setAttribute(link, 'type', 'text/css');
        this.renderer.setAttribute(link, 'rel', preload ? 'preload' : 'stylesheet');
        this.renderer.setAttribute(link, 'href', `${window.location.origin}/assets/css/${styleName}.css?v=${environment.appVersion}`);

        if (preload) {
            this.renderer.setAttribute(link, 'as', 'style');
            link.onload = () => { this.renderer.setAttribute(link, 'rel', 'stylesheet'); };
        }

        link.onerror = () => {};

        this.renderer.appendChild(document.head, link);
    }
}
