import { Component, OnDestroy } from '@angular/core';
import { NavigationEnd, Router } from '@angular/router';
import { Subject, takeUntil } from 'rxjs';
import { HeaderButtonModel, defaultHeaderButton } from "../../interface/headerButton.model";
import { NavService } from "../../services/nav.service";
import { EventService } from "../../services/event.service";


@Component({
    selector: 'app-breadcrumb',
    templateUrl: './breadcrumb.component.html',

})

export class BreadcrumbComponent implements OnDestroy {
    title: string | undefined;
    sectionHeaderIsHidden: boolean | undefined;
    button: HeaderButtonModel = defaultHeaderButton();
    showInput: boolean = false;
    private destroy$ = new Subject<void>();

    constructor(
        private router: Router,
        private navService: NavService,
        private eventService: EventService,
    ) {
        this.eventService.ShowUploadButtonEvent.pipe(takeUntil(this.destroy$)).subscribe(
            (showUploadInput: boolean) => {
                this.showInput = showUploadInput;
            },
        );

        this.router.events.pipe(takeUntil(this.destroy$)).subscribe((event) => {
            if (event instanceof NavigationEnd) {
                this.button = defaultHeaderButton();
                let pages = navService.MainPages;
                for (let i = 0; i < pages.length; i++) {
                    let item = pages[i];
                    let itemPath = item.path ?? '';
                    let splitItemPath = itemPath.split('/');
                    let splitEventPath = event.urlAfterRedirects.split('/');
                    let firstItemPath = 'itemDefault';
                    let firstEventPath = 'eventDefault';
                    if (splitItemPath.length > 1 && splitEventPath.length > 1) {
                        firstItemPath = splitItemPath[1];
                        firstEventPath = splitEventPath[1];
                    }

                    let isFullPathMatch = itemPath.includes(event.urlAfterRedirects);
                    let isPartPathMatch = firstItemPath.includes(firstEventPath);
                    //если в руте есть двоеточие, значит нужно проверить сходимость рутов без айдишника(или другого параметра)
                    if (itemPath.includes(':')) {
                        isFullPathMatch = (firstItemPath + '/' + splitItemPath[2]).includes(
                            firstEventPath + '/' + splitEventPath[2],
                        );
                    }
                    if (isFullPathMatch || isPartPathMatch) {
                        this.title = item.header ? item.header : item.title;
                        this.sectionHeaderIsHidden = item.sectionHeaderIsHidden;
                        if (isFullPathMatch) break;
                    }
                }

                navService.HEADERBUTTONS.filter((item: HeaderButtonModel) => {
                    if (
                        item.itemTitle == this.title ||
                        item.headerTitle == this.title
                    ) {
                        this.button = item;
                    }
                });
            }
        });
    }

    ngOnDestroy() {
        this.destroy$.next();
        this.destroy$.complete();
    }

    onButtonClick(): void {
        if (this.button.url) {
            this.router.navigate([this.button.url]);
        }
    }
}
