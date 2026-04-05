import { NgModule } from '@angular/core';
import { CommonModule, NgOptimizedImage } from '@angular/common';

import { UserInfoRoutingModule } from './users-info-routing.module';
import { UserInfoComponent } from './users-info.component';
import { SharedModule } from "../../shared/shared.component";


@NgModule({
    declarations: [
        UserInfoComponent,
    ],
    imports: [
        CommonModule,
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
