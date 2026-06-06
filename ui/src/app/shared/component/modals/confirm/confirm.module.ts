import { NgModule } from '@angular/core';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { ConfirmModal } from './confirm.modal';

@NgModule({
    declarations: [ConfirmModal],
    imports: [NgbModule, FormsModule, ReactiveFormsModule],
})
export class ConfirmModule {
}
