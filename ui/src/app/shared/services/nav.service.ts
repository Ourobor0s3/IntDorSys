import { HostListener, Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { menuitems } from '../constants/menuitems';
import { laundressRoute, overviewRoute, reportsRoute, userInfoRoute } from "../constants/routes";
import { HeaderButtonModel } from "../model/headerButton.model";

// Menu
export interface Page {
    path?: string;
    title?: string;
    icon?: any;
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
    public screenWidth: any;
    public collapseSidebar: boolean = true;
    public collapseHeaderInfo: boolean = false;
    menuitems = menuitems;
    public currentUrl: any;
    public previousUrl: any;
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
