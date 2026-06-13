import { Injectable } from '@angular/core';
import { Subject } from 'rxjs';
import { HeaderButtonModel } from "../model/headerButton.model";

@Injectable({
    providedIn: 'root',
})
export class EventService {
    readonly LangChangeEvent = new Subject<string>();
    readonly NotifCountEvent = new Subject<number>();
    readonly SubpageEvent = new Subject<HeaderButtonModel>();
    readonly ShowUploadButtonEvent = new Subject<boolean>();
    readonly SubscriptionUpgrade = new Subject<void>();
    private arrIntervals: ReturnType<typeof setInterval>[] = [];
    private funcImpl: (() => void)[] = [];
    private arrForLogOut: { clear: () => void }[] = [];

    langChanged(msg: string): void {
        this.LangChangeEvent.next(msg);
    }

    public addFuncToArrayOfIntervals(func: () => void, interval: number) {
        this.funcImpl.push(func);
        this.arrIntervals.push(setInterval(func, interval));
    }

    public isFuncArrIncludes(func: () => void) {
        return this.funcImpl.includes(func);
    }

    public addFuncToArrayForLogout(func: { clear: () => void }) {
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
        this.arrIntervals.forEach((element: ReturnType<typeof setInterval>) => {
            clearInterval(element);
        });
    }

    public clearModels() {
        this.arrForLogOut.forEach((element: { clear: () => void }) => {
            try {
                element.clear();
            } catch {
                // ignore — element may not implement clear()
            }
        });
    }
}
