import { AfterViewChecked, Component, ElementRef, OnDestroy, OnInit, Renderer2, ViewChild } from '@angular/core';
import { BaseComponent } from "../base/base.component";
import { NavigationEnd, NavigationStart, Router } from "@angular/router";
import { NavService, Page } from "../../services/nav.service";
import { Subject, takeUntil } from "rxjs";
import { UserInfoModel } from "../../interface/userInfo.model";
import { UserService } from "../../services/user.service";
import { AuthService } from '../../services/auth.service';

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
        private userService: UserService,
        protected navService: NavService,
        private authService: AuthService,
        private renderer: Renderer2,
    ) {
        super(renderer);
        this.getUser = () => this.userService.get() ?? ({} as UserInfoModel);
    }

    ngAfterViewChecked(): void {
        const scrollPosition = sessionStorage.getItem('sidebarScrollY');
        if (!!scrollPosition && this.sidebarEl) {
            this.renderer.setProperty(this.sidebarEl.nativeElement, 'scrollTop', +scrollPosition);
        }
    }

    ngOnInit(): void {
        this.navService.mainItems.pipe(takeUntil(this.destroy$)).subscribe(menuItems => {
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
        if (!this.menuItems) return;
        this.navService.updateActiveTabs(this.menuItems, url);
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

        this.menuItems?.forEach((a) => {
            if (this.menuItems?.includes(item))
                a.active = false
            if (!a.children) return
            a.children.forEach(b => {
                if (a.children?.includes(item)) {
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
