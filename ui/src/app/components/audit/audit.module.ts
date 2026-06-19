import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from "@angular/forms";

import { AuditRoutingModule } from './audit-routing.module';
import { AuditComponent } from './audit.component';
import { SharedModule } from "../../shared/shared.module";

@NgModule({
    declarations: [
        AuditComponent,
    ],
    imports: [
        CommonModule,
        FormsModule,
        AuditRoutingModule,
        SharedModule,
    ],
    exports: [
        AuditComponent,
    ],
})
export class AuditModule {
}
