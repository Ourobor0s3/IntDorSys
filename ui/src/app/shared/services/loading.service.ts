import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

@Injectable({
    providedIn: 'root',
})
export class LoadingService {
    private loadingSubject = new BehaviorSubject<boolean>(false);
    private counter = 0;
    loading$ = this.loadingSubject.asObservable();

    setLoading(isLoading: boolean): void {
        if (isLoading) {
            this.counter++;
        } else {
            this.counter = Math.max(0, this.counter - 1);
        }
        this.loadingSubject.next(this.counter > 0);
    }
}
