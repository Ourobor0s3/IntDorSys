import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { LaundressComponent } from './laundress.component';

const routes: Routes = [
    {
        path: '',
        component: LaundressComponent,
        canActivate: [],
    },
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule],
})
export class LaundressRoutingModule {
}
