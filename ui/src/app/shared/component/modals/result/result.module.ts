import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { ResultModal } from './result.modal';

@NgModule({
    declarations: [ResultModal],
    imports: [CommonModule, NgbModule, FormsModule, ReactiveFormsModule],
})
export class ResultModule {
}
