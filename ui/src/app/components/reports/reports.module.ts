import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SharedModule } from "../../shared/shared.component";
import { BsDatepickerModule } from "ngx-bootstrap/datepicker";
import { ReportsComponent } from "./reports.component";
import { ReportsRoutingModule } from "./reports-routing.module";


@NgModule({
    declarations: [
        ReportsComponent,
    ],
    imports: [
        CommonModule,
        ReportsRoutingModule,
        SharedModule,
        BsDatepickerModule,
    ],
    exports: [
        ReportsComponent,
    ],
})
export class ReportsModule {
}
