import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ContentLayoutComponent } from './shared/component/content-layout/content-layout.component';
import { AuthGuard } from './shared/guards/auth.guard';
import { mainContent } from "./shared/routes/routes";
import { AuthComponent } from "./auth/auth.component";
import { authRoute } from "./shared/constants/routes";

const mainRoutes: Routes = [
    {
        path: authRoute + '/:subpageRoute',
        component: AuthComponent,
    },
    {
        path: '',
        component: ContentLayoutComponent,
        canActivate: [AuthGuard],
        children: mainContent,
    },
    {
        path: '**',
        redirectTo: '',
    },
];

@NgModule({
    imports: [RouterModule.forRoot(mainRoutes, {
        scrollPositionRestoration: 'enabled',
    })],
    exports: [RouterModule],
})
export class AppRoutingModule {
}
