import { NgModule } from '@angular/core';
import { FormsModule } from "@angular/forms";

import { SettingsRoutingModule } from './settings-routing.module';
import { SettingsComponent } from './settings.component';
import { SharedModule } from "../../shared/shared.component";

@NgModule({
    declarations: [
        SettingsComponent,
    ],
    imports: [
        FormsModule,
        SettingsRoutingModule,
        SharedModule,
    ],
})
export class SettingsModule {
}
