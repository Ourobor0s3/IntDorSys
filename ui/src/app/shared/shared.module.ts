import { NgModule } from "@angular/core";
import { FormsModule } from "@angular/forms";
import { LoaderComponent } from "./component/loader/loader.component";
import { ContentLayoutComponent } from "./component/content-layout/content-layout.component";
import { FooterComponent } from "./component/footer/footer.component";
import { HeaderComponent } from "./component/header/header.component";
import { RouterModule } from "@angular/router";
import { SidebarComponent } from "./component/sidebar/sidebar.component";
import { BreadcrumbComponent } from "./component/breadcrumb/breadcrumb.component";
import { SectionHeaderComponent } from "./component/section-header/section-header.component";
import { CommonModule } from "@angular/common";
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { BarChartComponent } from "./component/bar-chart/bar-chart.component";
import { TranslateReplacePipe } from "./pipes/translate-replace.pipe";
import { StatusStylePipe } from "./pipes/status-style.pipe";
import { ErrorMessagePipe } from "./pipes/error-message.pipe";
import { TranslateModule } from "@ngx-translate/core";
import { BsDatepickerModule } from "ngx-bootstrap/datepicker";
import { ConfirmModule } from "./component/modals/confirm/confirm.module";
import { ResultModule } from "./component/modals/result/result.module";

@NgModule({
    declarations: [
        HeaderComponent,
        FooterComponent,
        ContentLayoutComponent,
        LoaderComponent,
        SidebarComponent,
        BreadcrumbComponent,
        SectionHeaderComponent,
        BarChartComponent,
        TranslateReplacePipe,
        StatusStylePipe,
        ErrorMessagePipe,
    ],
    imports: [
        CommonModule,
        RouterModule,
        TranslateModule,
        FormsModule,
        BsDatepickerModule,
        NgbModule,
        ConfirmModule,
        ResultModule,
    ],
    exports: [
        BarChartComponent,
        ContentLayoutComponent,
        LoaderComponent,
        BreadcrumbComponent,
        SectionHeaderComponent,
        TranslateReplacePipe,
        StatusStylePipe,
        ErrorMessagePipe,
        BsDatepickerModule,
    ],
    providers: [],
})
export class SharedModule {
}
