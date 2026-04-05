import { EventEmitter, Injectable, Output } from '@angular/core';
import { HeaderButtonModel } from "../model/headerButton.model";

@Injectable({
    providedIn: 'root',
})
export class EventService {
    @Output() LangChangeEvent = new EventEmitter<string>();
    @Output() NotifCountEvent = new EventEmitter<any>();
    @Output() SubpageEvent = new EventEmitter<HeaderButtonModel>();
    @Output() ShowUploadButtonEvent = new EventEmitter<boolean>();
    @Output() SubscriptionUpgrade = new EventEmitter<void>();
    private arrIntervals: any = [];
    private funcImpl: any[] = [];
    private arrForLogOut: any = [];

    Langchanged(msg: string) {
        this.LangChangeEvent.emit(msg);
    }

    public addFuncToArrayOfIntervals(func: () => void, interval: number) {
        this.funcImpl.push(func);
        this.arrIntervals.push(setInterval(func, interval));
    }

    public isFuncArrIncludes(func: any) {
        return this.funcImpl.includes(func);
    }

    public addFuncToArrayForLogout(func: any) {
        this.arrForLogOut.push(func);
    }

    public logout() {
        this.clearIntervals();
        this.clearModels();
        this.arrIntervals = [];
        this.arrForLogOut = [];
        this.funcImpl = [];
    }

    public clearIntervals() {
        this.arrIntervals.forEach((element: any) => {
            clearInterval(element);
        });
    }

    public clearModels() {
        this.arrForLogOut.forEach((element: any) => {
            try {
                element.clear();
            } catch (ex) {
            }
        });
    }
}
