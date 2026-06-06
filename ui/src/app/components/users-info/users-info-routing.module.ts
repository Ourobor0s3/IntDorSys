import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { UserInfoComponent } from './users-info.component';
import { AuthGuard } from 'src/app/shared/guards/auth.guard';

const routes: Routes = [
    {
        path: '',
        component: UserInfoComponent,
        canActivate: [AuthGuard],
    },
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule],
})
export class UserInfoRoutingModule {
}
