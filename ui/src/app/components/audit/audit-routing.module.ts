import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AuditComponent } from './audit.component';
import { AuthGuard } from 'src/app/shared/guards/auth.guard';

const routes: Routes = [
    {
        path: '',
        component: AuditComponent,
        canActivate: [AuthGuard],
    },
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule],
})
export class AuditRoutingModule {
}
