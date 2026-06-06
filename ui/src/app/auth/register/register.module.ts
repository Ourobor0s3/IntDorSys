import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RegisterComponent } from './register.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { SharedModule } from "../../shared/shared.component";

@NgModule({
    declarations: [RegisterComponent],
    imports: [CommonModule, ReactiveFormsModule, FormsModule, SharedModule],
    exports: [RegisterComponent],
})
export class RegisterModule {
}
