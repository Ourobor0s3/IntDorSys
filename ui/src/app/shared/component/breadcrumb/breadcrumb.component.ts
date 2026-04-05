import { Component, OnDestroy, OnInit } from '@angular/core';
import { NavigationEnd, Router } from '@angular/router';
import { Subscription } from 'rxjs';
import { HeaderButtonModel } from "../../model/headerButton.model";
import { NavService } from "../../services/nav.service";
import { EventService } from "../../services/event.service";


@Component({
    selector: 'app-breadcrumb',
    templateUrl: './breadcrumb.component.html',
    styleUrls: ['./breadcrumb.component.scss'],
})

export class BreadcrumbComponent implements OnInit, OnDestroy {
    title: string | undefined;
    sectionHeaderIsHidden: boolean | undefined;
    button1: HeaderButtonModel = new HeaderButtonModel();
    button2: HeaderButtonModel = new HeaderButtonModel();
    showInput: boolean = false;
    buttonsSubscription: Subscription | undefined;
    inputSubscription: Subscription | undefined;

    constructor(
        private router: Router,
        private navService: NavService,
        private eventService: EventService,
    ) {
        var t = this;
        t.buttonsSubscription = t.eventService.SubpageEvent.subscribe(
            (headerButton: HeaderButtonModel) => {
                switch (headerButton.buttonNumber) {
                    case 0:
                        t.setButtonOptions(t.button1, headerButton);
                        break;
                    case 1:
                        t.setButtonOptions(t.button2, headerButton);
                        break;
                    default:
                        break;
                }
            },
        );

        t.inputSubscription = t.eventService.ShowUploadButtonEvent.subscribe(
            (showUploadInput: boolean) => {
                t.showInput = showUploadInput;
            },
        );

        t.router.events.subscribe((event) => {
            //t.title = '';
            t.button1 = new HeaderButtonModel();
            t.button2 = new HeaderButtonModel();
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

                    //isFullPathMatch - полное соответствие урла, т.е. такой раздел прописан в PAGES и имеет собственный хедер
                    var isFullPathMatch = item.path!.includes(event.urlAfterRedirects);
                    var isPartPathMatch = firstItemPath.includes(firstEventPath);
                    //если в руте есть двоеточие, значит нужно проверить сходимость рутов без айдишника(или другого параметра)
                    if (item.path!.includes(':')) {
                        isFullPathMatch = (firstItemPath + '/' + splitItemPath[2]).includes(
                            firstEventPath + '/' + splitEventPath[2],
                        );
                    }
                    if (isFullPathMatch || isPartPathMatch) {
                        t.title = item.header ? item.header : item.title;
                        t.sectionHeaderIsHidden = item.sectionHeaderIsHidden;
                        if (isFullPathMatch) break;
                    }
                }

                navService.HEADERBUTTONS.filter((item: any) => {
                    if (
                        item.itemTitle == t.title ||
                        item.headerTitle == t.title ||
                        (!!item.url && event.urlAfterRedirects.includes(item.url))
                    ) {
                        if (item.buttonNumber == 0) {
                            t.button1 = item;
                        }
                        if (item.buttonNumber == 1) {
                            t.button2 = item;
                        }
                    }
                });
            }
        });
    }

    ngOnInit(): void {
    }

    ngOnDestroy() {
        var t = this;
        if (!!t.buttonsSubscription) {
            t.buttonsSubscription.unsubscribe();
        }
        if (!!t.inputSubscription) {
            t.inputSubscription.unsubscribe();
        }
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
