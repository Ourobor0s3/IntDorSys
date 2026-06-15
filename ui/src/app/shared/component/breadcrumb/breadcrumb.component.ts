import { Component, OnDestroy } from '@angular/core';
import { NavigationEnd, Router } from '@angular/router';
import { Subject, takeUntil } from 'rxjs';
import { HeaderButtonModel } from "../../model/headerButton.model";
import { NavService } from "../../services/nav.service";
import { EventService } from "../../services/event.service";


@Component({
    selector: 'app-breadcrumb',
    templateUrl: './breadcrumb.component.html',

})

export class BreadcrumbComponent implements OnDestroy {
    title: string | undefined;
    sectionHeaderIsHidden: boolean | undefined;
    button1: HeaderButtonModel = new HeaderButtonModel();
    button2: HeaderButtonModel = new HeaderButtonModel();
    showInput: boolean = false;
    private destroy$ = new Subject<void>();

    constructor(
        private router: Router,
        private navService: NavService,
        private eventService: EventService,
    ) {
        this.eventService.SubpageEvent.pipe(takeUntil(this.destroy$)).subscribe(
            (headerButton: HeaderButtonModel) => {
                switch (headerButton.buttonNumber) {
                    case 0:
                        this.setButtonOptions(this.button1, headerButton);
                        break;
                    case 1:
                        this.setButtonOptions(this.button2, headerButton);
                        break;
                    default:
                        break;
                }
            },
        );

        this.eventService.ShowUploadButtonEvent.pipe(takeUntil(this.destroy$)).subscribe(
            (showUploadInput: boolean) => {
                this.showInput = showUploadInput;
            },
        );

        this.router.events.pipe(takeUntil(this.destroy$)).subscribe((event) => {
            //this.title = '';
            this.button1 = new HeaderButtonModel();
            this.button2 = new HeaderButtonModel();
            if (event instanceof NavigationEnd) {
                let pages = navService.MainPages;
                for (let i = 0; i < pages.length; i++) {
                    let item = pages[i];
                    let splitItemPath = item.path!.split('/');
                    let splitEventPath = event.urlAfterRedirects.split('/');
                    let firstItemPath = 'itemDefault';
                    let firstEventPath = 'eventDefault';
                    if (splitItemPath.length > 1 && splitEventPath.length > 1) {
                        firstItemPath = splitItemPath[1];
                        firstEventPath = splitEventPath[1];
                    }

                    let isFullPathMatch = item.path!.includes(event.urlAfterRedirects);
                    let isPartPathMatch = firstItemPath.includes(firstEventPath);
                    //если в руте есть двоеточие, значит нужно проверить сходимость рутов без айдишника(или другого параметра)
                    if (item.path!.includes(':')) {
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
                        item.headerTitle == this.title ||
                        (!!item.url && event.urlAfterRedirects.includes(item.url))
                    ) {
                        if (item.buttonNumber == 0) {
                            this.button1 = item;
                        }
                        if (item.buttonNumber == 1) {
                            this.button2 = item;
                        }
                    }
                });
            }
        });
    }

    ngOnDestroy() {
        this.destroy$.next();
        this.destroy$.complete();
    }

    setButtonOptions(model: HeaderButtonModel, newModel: HeaderButtonModel) {
        if (!!newModel.buttonTitle) {
            model.buttonTitle = newModel.buttonTitle;
        }
        model.needShow = newModel.needShow;
        model.buttonNumber = newModel.buttonNumber;
        if (!!newModel.className) {
            model.className = newModel.className;
        }
    }
}
