import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { ConfirmModal } from './confirm.modal';

@NgModule({
    declarations: [ConfirmModal],
    imports: [CommonModule, NgbModule, FormsModule, ReactiveFormsModule],
})
export class ConfirmModule {
}
