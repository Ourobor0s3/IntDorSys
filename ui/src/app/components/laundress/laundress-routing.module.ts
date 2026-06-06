import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { LaundressComponent } from './laundress.component';
import { AuthGuard } from 'src/app/shared/guards/auth.guard';

const routes: Routes = [
    {
        path: '',
        component: LaundressComponent,
        canActivate: [AuthGuard],
    },
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule],
})
export class LaundressRoutingModule {
}
