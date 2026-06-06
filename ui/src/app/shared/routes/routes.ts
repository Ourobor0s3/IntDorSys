import { Routes } from "@angular/router";
import { auditRoute, laundressRoute, overviewRoute, reportsRoute, settingsRoute, userInfoRoute } from "../constants/routes";

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
        path: auditRoute,
        loadChildren: () =>
            import('../../components/audit/audit.module')
                .then(m => m.AuditModule),
    },
    {
        path: settingsRoute,
        loadChildren: () =>
            import('../../components/settings/settings.module')
                .then(m => m.SettingsModule),
    },
];
