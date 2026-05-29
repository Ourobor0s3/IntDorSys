import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { LoginComponent } from './login.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { SharedModule } from "../../shared/shared.component";

@NgModule({
    declarations: [LoginComponent],
    imports: [CommonModule, ReactiveFormsModule, FormsModule, SharedModule],
    exports: [LoginComponent],
})
export class LoginModule {
}
