import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from "@angular/forms";

import { SettingsRoutingModule } from './settings-routing.module';
import { SettingsComponent } from './settings.component';
import { SharedModule } from "../../shared/shared.component";

@NgModule({
    declarations: [
        SettingsComponent,
    ],
    imports: [
        CommonModule,
        FormsModule,
        SettingsRoutingModule,
        SharedModule,
    ],
})
export class SettingsModule {
}
