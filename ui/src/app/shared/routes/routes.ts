import { Routes } from "@angular/router";
import { authRoute, laundressRoute, overviewRoute, reportsRoute, userInfoRoute } from "../constants/routes";

export const mainContent: Routes = [
    {
        path: '',
        redirectTo: overviewRoute,
        pathMatch: 'full',
        data: { animation: 'Overview' },
    },
    {
        path: overviewRoute,
        loadChildren: () =>
            import('../../components/home/home.module')
                .then(m => m.HomeModule),
    },
    {
        path: userInfoRoute,
        loadChildren: () =>
            import('../../components/users-info/users-info.module')
                .then(m => m.UserInfoModule),
    },
    {
        path: laundressRoute,
        loadChildren: () =>
            import('../../components/laundress/laundress.module')
                .then(m => m.LaundressModule),
    },
    {
        path: reportsRoute,
        loadChildren: () =>
            import('../../components/reports/reports.module')
                .then(m => m.ReportsModule),
    },
    {
        path: authRoute + '/:subpageRoute',
        loadChildren: () => import('../../app.module')
            .then((m) => m.AppModule),
        data: { animation: 'Auth' },
    },
];
