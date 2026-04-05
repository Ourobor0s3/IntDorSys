import { AfterViewChecked, Component, OnDestroy, OnInit } from '@angular/core';
import { BaseComponent } from "../base/base.component";
import { NavigationEnd, NavigationStart, Router } from "@angular/router";
import { NavService, Page } from "../../services/nav.service";
import { Subject, takeUntil } from "rxjs";
import { DataReloadService } from "../../services/dataReload.service";
import { NgbModal } from "@ng-bootstrap/ng-bootstrap";
import { UserInfoModel } from "../../model/userInfo.model";
import { UserService } from "../../services/user.service";
import { TranslateService } from '@ngx-translate/core';

@Component({
    selector: 'app-sidebar',
    templateUrl: './sidebar.component.html',
    styleUrls: ['./sidebar.component.scss'],
})
export class SidebarComponent extends BaseComponent implements OnInit, AfterViewChecked, OnDestroy {
    public getUser: () => UserInfoModel;
    isMobileMenuOpen = false;
    protected menuItems: Page[] | undefined;
    private destroy$ = new Subject<void>();

    constructor(
        private router: Router,
        private modalService: NgbModal,
        private userService: UserService,
        protected navService: NavService,
        private dataReloadService: DataReloadService,
        private translate: TranslateService,
    ) {
        super(translate, modalService);
        let t = this;
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

        this.router.events.subscribe((ev) => {
            if (ev instanceof NavigationStart) {
                let sidebar = document.getElementById('sidebar');
                if (sidebar) sessionStorage.setItem('sidebarScrollY', sidebar.scrollTop.toString());
            }
        });

        t.getUser = () => t.userService.get() ?? new UserInfoModel();
    }

    ngAfterViewChecked(): void {
        var scrollPosition = sessionStorage.getItem('sidebarScrollY');
        if (!!scrollPosition) {
            let sidebar = document.getElementById('sidebar');
            sidebar!.scrollTop = +scrollPosition;
        }
    }

    ngOnInit(): void {
        this.loadData();
        this.dataReloadService.dataReload$
            .pipe(takeUntil(this.destroy$))
            .subscribe(() => {
                this.loadData();
            });
    }

    ngOnDestroy() {
        let t = this;
        t.destroy$.next();
        t.destroy$.complete();
    }

    loadData() {
    }

    needHidden(navRoles: string[] | undefined) {
        var t = this;
        return false;
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

    toggleMobileMenu() {
        this.isMobileMenuOpen = !this.isMobileMenuOpen;
        document.body.style.overflow = this.isMobileMenuOpen ? 'hidden' : '';
    }

    // Click Toggle menu
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
        //Закрытие навбара при переходе на другие страницы в мобилке
        if (window.innerWidth < 569 && t.navService.collapseSidebar)
            t.collapseSidebar();
        item.active = !item.active;
    }

    collapseSidebar() {
        this.navService.collapseSidebar = !this.navService.collapseSidebar;
    }
}
