import { Component, OnDestroy, Renderer2 } from '@angular/core';
import { ChildrenOutletContexts, NavigationEnd, Router } from "@angular/router";
import { NgbModal, NgbModalRef } from "@ng-bootstrap/ng-bootstrap";
import { filter, Subscription } from "rxjs";
import { BaseComponent } from "./shared/component/base/base.component";
import { TranslateService } from '@ngx-translate/core';
import { languages } from './shared/constants/languages';
import { Language } from './shared/enums/language';
import { animate, style, transition, trigger } from '@angular/animations';
import { AuthService } from "./shared/services/auth.service";
import { UserService } from "./shared/services/user.service";
import { EventService } from "./shared/services/event.service";

interface StyleConfig {
    name: string;
    preload: boolean;
}

@Component({
    selector: 'app-root',
    templateUrl: './app.component.html',
    styleUrls: ['./app.component.scss'],
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
    title = "ui";
    modalRef: NgbModalRef;
    private eventSubscription: Subscription;
    private readonly styleConfigs: StyleConfig[] = [
        { name: 'main', preload: false },
        { name: 'table', preload: false },
    ];

    constructor(
        private modalService: NgbModal,
        private contexts: ChildrenOutletContexts,
        private renderer: Renderer2,
        private router: Router,
        private translate: TranslateService,
        private authService: AuthService,
        private eventService: EventService,
        private userService: UserService,
    ) {
        super(translate, modalService);
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
        // Initialize language
        this.translate.use(languages[Language.EN].shortName);

        // Load styles
        this.styleConfigs.forEach(config => this.createStyle(config.name, config.preload));

        // Setup router events
        this.setupRouterEvents();
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

    // Create style sheet append in head
    private createStyle(styleName: string, preload = false): void {
        const link = document.createElement('link');
        link.type = 'text/css';
        link.rel = preload ? 'preload' : 'stylesheet';
        link.href = `${window.location.origin}/assets/css/${styleName}.css`;

        if (preload) {
            link.as = 'style';
            link.setAttribute('onload', "this.rel='stylesheet'");
        }

        document.head.appendChild(link);
    }
}
