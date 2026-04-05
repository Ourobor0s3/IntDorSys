import { Component, OnInit } from '@angular/core';
import { BaseComponent } from 'src/app/shared/component/base/base.component';
import { ActivatedRoute, Router } from '@angular/router';
import { authSubpages, Subpages } from './auth.subpages';
import { transition, trigger, useAnimation } from '@angular/animations';
import { fadeIn } from 'ng-animate';
import { authRoute } from '../shared/constants/routes';
import { NgbModal } from "@ng-bootstrap/ng-bootstrap";
import { UserService } from "../shared/services/user.service";
import { TranslateService } from '@ngx-translate/core';
import { Language } from '../shared/enums/language';
import { languages } from '../shared/constants/languages';

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
    currentActiveTab: any;
    currentYear: any;

    constructor(
        private activateRoute: ActivatedRoute,
        private router: Router,
        private userService: UserService,
        private modal: NgbModal,
        private translate: TranslateService,
    ) {
        super(translate, modal);
        let t = this;
        t.router.routeReuseStrategy.shouldReuseRoute = function () {
            return false;
        };
        t.currentYear = new Date().getFullYear();
        t.translate.setDefaultLang(languages[Language.EN].shortName);
    }

    ngOnInit(): void {
        let t = this;
        let subpageRoute = t.activateRoute.snapshot.paramMap.get('subpageRoute');
        t.currentActiveTab = t.subpagesMenu.find((tab) => tab.route == subpageRoute);
        if (!subpageRoute || !t.currentActiveTab) {
            t.navigateTab(t.subpagesMenu[0].route);
        }
        t.activateTab(t.currentActiveTab);
    }

    ngOnDestroy() {
        if (!!this.currentActiveTab)
            this.currentActiveTab.isActive = false;
    }

    navigateTab(route: string) {
        let t = this;
        t.router.navigateByUrl('/' + authRoute + '/' + route);
    }

    getCurrentState() {
        return this.subpagesMenu.findIndex(page => page.isActive);
    }

    public activateTab(item: any) {
        let t = this;
        let currentTab = t.subpagesMenu.find((val) => val.route == item.route);
        currentTab!.isActive = true;
        t.currentActiveTab = currentTab;
    }
}
