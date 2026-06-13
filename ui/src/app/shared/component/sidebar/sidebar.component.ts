import { AfterViewChecked, Component, ElementRef, OnDestroy, OnInit, Renderer2, ViewChild } from '@angular/core';
import { BaseComponent } from "../base/base.component";
import { NavigationEnd, NavigationStart, Router } from "@angular/router";
import { NavService, Page } from "../../services/nav.service";
import { Subject, takeUntil } from "rxjs";
import { NgbModal } from "@ng-bootstrap/ng-bootstrap";
import { UserInfoModel } from "../../model/userInfo.model";
import { UserService } from "../../services/user.service";
import { AuthService } from '../../services/auth.service';
import { TranslateService } from '@ngx-translate/core';

@Component({
    selector: 'app-sidebar',
    templateUrl: './sidebar.component.html',
    styleUrls: ['./sidebar.component.scss'],
})
export class SidebarComponent extends BaseComponent implements OnInit, AfterViewChecked, OnDestroy {
    @ViewChild('sidebar', { static: false }) sidebarEl!: ElementRef;
    public getUser: () => UserInfoModel;
    isMobileMenuOpen = false;
    protected menuItems: Page[] | undefined;
    private destroy$ = new Subject<void>();

    constructor(
        private router: Router,
        private modalService: NgbModal,
        private userService: UserService,
        protected navService: NavService,
        private authService: AuthService,
        private translate: TranslateService,
        private renderer: Renderer2,
    ) {
        super(translate, modalService);
        let items = this.navService.mainItems;
        items.pipe(takeUntil(this.destroy$)).subscribe(menuItems => {
            this.menuItems = menuItems;
        });

        this.router.events.pipe(takeUntil(this.destroy$)).subscribe((event) => {
            if (event instanceof NavigationEnd) {
                if (this.menuItems) {
                    this.updateActiveTabs(event.url);
                }
                if (event.url != this.navService.currentUrl) {
                    this.navService.previousUrl = this.navService.currentUrl;
                    this.navService.currentUrl = event.url;
                }
            }
            if (event instanceof NavigationStart) {
                if (this.sidebarEl) sessionStorage.setItem('sidebarScrollY', this.sidebarEl.nativeElement.scrollTop.toString());
            }
        });

        this.getUser = () => this.userService.get() ?? new UserInfoModel();
    }

    ngAfterViewChecked(): void {
        const scrollPosition = sessionStorage.getItem('sidebarScrollY');
        if (!!scrollPosition && this.sidebarEl) {
            this.sidebarEl.nativeElement.scrollTop = +scrollPosition;
        }
    }

    ngOnInit(): void {
        if (this.menuItems) {
            this.updateActiveTabs(this.router.url);
        }
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

    updateActiveTabs(url: string) {
        //ищем выбранный элемент среди menuItems
        let curItem = this.menuItems!.find(x => x.path == url && x.type == 'link');
        if (!curItem) {
            //если выбранного элемента нет среди menuItems - смотрим детей
            this.menuItems!.forEach(items => {
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
        this.menuItems!.filter(x => x.path != curItem!.path).forEach(menuItem => {
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
        if (this.isMobileMenuOpen) {
            this.renderer.setStyle(document.body, 'overflow', 'hidden');
        } else {
            this.renderer.removeStyle(document.body, 'overflow');
        }
    }

    // Click Toggle menu
    toggletNavActive(item: Page) {
        if (item.path == this.router.url) {
            return;
        }

        this.menuItems!.forEach((a) => {
            if (this.menuItems!.includes(item))
                a.active = false
            if (!a.children) return false
            a.children.forEach(b => {
                if (a.children!.includes(item)) {
                    b.active = false
                }
            })
        });
        //Закрытие навбара при переходе на другие страницы в мобилке
        if (window.innerWidth < 569 && this.navService.collapseSidebar)
            this.collapseSidebar();
        item.active = !item.active;
    }

    collapseSidebar() {
        this.navService.collapseSidebar = !this.navService.collapseSidebar;
    }

    trackByMenuItem(index: number, item: Page): string {
        return item.path || String(index);
    }
}
