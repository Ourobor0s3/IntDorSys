import { AfterViewInit, ChangeDetectorRef, Component, ElementRef, HostListener, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { ChildrenOutletContexts, Router } from "@angular/router";
import { NavService } from "../../services/nav.service";
import { transition, trigger, useAnimation } from "@angular/animations";
import { fadeIn } from "ng-animate";
import { EventService } from "../../services/event.service";
import { Subject, takeUntil } from "rxjs";

@Component({
    selector: 'app-content-layout',
    templateUrl: './content-layout.component.html',
    styleUrls: ['./content-layout.component.scss'],
    animations: [
        trigger('animateRoute', [
            transition('* => *', useAnimation(fadeIn)),
        ]),
    ],
})
export class ContentLayoutComponent implements OnInit, AfterViewInit, OnDestroy {
    @ViewChild('sidebarBlock', { static: false }) sidebarBlockEl!: ElementRef;
    @ViewChild('pageBodyWrapper', { static: false }) pageBodyWrapperEl!: ElementRef;
    pageBodyWidth: number = 0;
    private destroy$ = new Subject<void>();

    constructor(
        public navServices: NavService,
        private router: Router,
        private contexts: ChildrenOutletContexts,
        private cdr: ChangeDetectorRef,
        private eventService: EventService,
    ) {
    }

    ngOnInit(): void {
        this.eventService.LangChangeEvent
            .pipe(takeUntil(this.destroy$))
            .subscribe(() => setTimeout(() => this.calculatePageBodyWrapperWidth(), 0));
    }

    ngOnDestroy(): void {
        this.destroy$.next();
        this.destroy$.complete();
    }

    ngAfterViewInit(): void {
        this.calculatePageBodyWrapperWidth();
        this.cdr.detectChanges();
    }

    public getRouterOutletState() {
        return this.contexts.getContext('primary')?.route?.snapshot?.data?.['animation'];
    }

    @HostListener('window:resize', ['$event'])
    onResize(event?: Event) {
        this.calculatePageBodyWrapperWidth();
    }

    // пересчет размеров pageBodyWrapper
    calculatePageBodyWrapperWidth() {
        if (this.sidebarBlockEl && this.pageBodyWrapperEl) {
            this.pageBodyWidth = this.pageBodyWrapperEl.nativeElement.offsetWidth - this.sidebarBlockEl.nativeElement.offsetWidth;
            this.cdr.detectChanges();
        }
    }

    collapseSidebar() {
        this.navServices.collapseSidebar = !this.navServices.collapseSidebar;
        setTimeout(() => this.calculatePageBodyWrapperWidth(), 0);
    }
}
