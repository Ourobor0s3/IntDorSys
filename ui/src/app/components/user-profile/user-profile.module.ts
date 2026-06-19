import { NgModule } from '@angular/core';
import { CommonModule, NgOptimizedImage } from '@angular/common';
import { UserProfileRoutingModule } from './user-profile-routing.module';
import { UserProfileComponent } from './user-profile.component';
import { SharedModule } from "../../shared/shared.module";

@NgModule({
    declarations: [ UserProfileComponent ],
    imports: [
        CommonModule, UserProfileRoutingModule, NgOptimizedImage, SharedModule,
    ],
})
export class UserProfileModule {}
