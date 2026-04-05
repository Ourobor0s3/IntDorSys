import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { LaundressRoutingModule } from './laundress-routing.module';
import { LaundressComponent } from './laundress.component';
import { SharedModule } from "../../shared/shared.component";
import { BsDatepickerModule } from "ngx-bootstrap/datepicker";


@NgModule({
    declarations: [
        LaundressComponent,
    ],
    imports: [
        CommonModule,
        LaundressRoutingModule,
        SharedModule,
        BsDatepickerModule,
    ],
    exports: [
        LaundressComponent,
    ],
})
export class LaundressModule {
}
