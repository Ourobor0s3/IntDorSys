import { Injectable } from '@angular/core';
import { Subject } from 'rxjs';
import { HeaderButtonModel } from "../model/headerButton.model";

@Injectable({
    providedIn: 'root',
})
export class EventService {
    readonly LangChangeEvent = new Subject<string>();
    readonly NotifCountEvent = new Subject<any>();
    readonly SubpageEvent = new Subject<HeaderButtonModel>();
    readonly ShowUploadButtonEvent = new Subject<boolean>();
    readonly SubscriptionUpgrade = new Subject<void>();
    private arrIntervals: any = [];
    private funcImpl: any[] = [];
    private arrForLogOut: any = [];

    Langchanged(msg: string) {
        this.LangChangeEvent.next(msg);
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
