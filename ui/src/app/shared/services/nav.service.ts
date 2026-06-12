import { HostListener, Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { menuitems } from '../constants/menuitems';
import { auditRoute, laundressRoute, overviewRoute, reportsRoute, settingsRoute, userInfoRoute, userProfileRoute } from "../constants/routes";
import { HeaderButtonModel } from "../model/headerButton.model";

// Menu
export interface Page {
    path?: string;
    title?: string;
    icon?: string;
    image?: string;
    type?: string;
    badgeType?: string;
    badgeValue?: string;
    active?: boolean;
    bookmark?: boolean;
    children?: Page[];
    roles?: string[];
    header?: string;
    notShowInSidebar?: boolean;
    sectionHeaderIsHidden?: boolean;
    translationKey?: string;
}

@Injectable({
    providedIn: 'root',
})
export class NavService {
    public screenWidth: number;
    public collapseSidebar: boolean = true;
    public collapseHeaderInfo: boolean = false;
    menuitems = menuitems;
    public currentUrl: string;
    public previousUrl: string;
    MainPages: Page[] = [
        {
            path: '/' + overviewRoute,
            title: 'menu.home',
            header: 'Overview',
            type: 'link',
            image: menuitems[overviewRoute],
            sectionHeaderIsHidden: true,
        },
        {
            path: '/' + laundressRoute,
            title: 'menu.laundress',
            type: 'link',
            image: menuitems[laundressRoute],
            roles: ['admin', 'user'],
        },
        {
            path: '/' + reportsRoute,
            title: 'menu.reports',
            type: 'link',
            image: menuitems[reportsRoute],
        },
        {
            path: '/' + userInfoRoute,
            title: 'menu.users',
            type: 'link',
            image: menuitems[userInfoRoute],
            roles: ['admin'],
        },
        {
            path: '/' + auditRoute,
            title: 'menu.audit',
            type: 'link',
            image: menuitems[auditRoute],
            roles: ['admin'],
        },
        {
            path: '/' + settingsRoute,
            title: 'menu.settings',
            type: 'link',
            image: menuitems[settingsRoute],
            roles: ['admin'],
        },
        {
            path: '/' + userProfileRoute,
            title: 'menu.profile',
            type: 'link',
            notShowInSidebar: true,
            roles: ['admin'],
        },
    ];
    HEADERBUTTONS: HeaderButtonModel[] = [];
    // Array
    mainItems = new BehaviorSubject<Page[]>(this.MainPages);

    constructor() {
        this.onResize();
        if (this.screenWidth < 991) {
            this.collapseSidebar = false;
            this.collapseHeaderInfo = true;
        }
    }

    @HostListener('window:resize', ['$event'])
    onResize() {
        this.screenWidth = window.innerWidth;
    }
}
