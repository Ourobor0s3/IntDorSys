import { NgModule } from '@angular/core';
import { CommonModule, NgOptimizedImage } from '@angular/common';
import { FormsModule } from "@angular/forms";

import { UserInfoRoutingModule } from './users-info-routing.module';
import { UserInfoComponent } from './users-info.component';
import { SharedModule } from "../../shared/shared.module";


@NgModule({
    declarations: [
        UserInfoComponent,
    ],
    imports: [
        CommonModule,
        FormsModule,
        UserInfoRoutingModule,
        NgOptimizedImage,
        SharedModule,
    ],
    exports: [
        UserInfoComponent,
    ],
})
export class UserInfoModule {
}
