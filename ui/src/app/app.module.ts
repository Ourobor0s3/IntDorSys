import { NgModule, ErrorHandler } from '@angular/core';

import { AppComponent } from './app.component';
import { SharedModule } from "./shared/shared.module";
import { AppRoutingModule } from "./app-routing.module";
import { LoginModule } from "./auth/login/login.module";
import { RegisterModule } from "./auth/register/register.module";
import { AuthComponent } from "./auth/auth.component";
import { BrowserModule } from "@angular/platform-browser";
import { FormsModule, ReactiveFormsModule } from "@angular/forms";
import { HTTP_INTERCEPTORS, HttpClient, HttpClientModule } from "@angular/common/http";
import { BrowserAnimationsModule } from "@angular/platform-browser/animations";
import { TranslateLoader, TranslateModule } from '@ngx-translate/core';
import { TranslateHttpLoader } from '@ngx-translate/http-loader';
import { BsDatepickerModule } from 'ngx-bootstrap/datepicker';
import { AuthInterceptor } from "./shared/services/auth.interceptor";
import { GlobalErrorHandler } from "./shared/services/global-error-handler";

export function HttpLoaderFactory(http: HttpClient) {
    return new TranslateHttpLoader(http, './assets/i18n/', '.json?ver=' + new Date().getTime());
}


@NgModule({
    declarations: [
        AppComponent, AuthComponent,
    ],
    imports: [
        BrowserModule,
        FormsModule,
        ReactiveFormsModule,
        SharedModule,
        HttpClientModule,
        BrowserAnimationsModule,
        AppRoutingModule,
        LoginModule,
        RegisterModule,
        BsDatepickerModule.forRoot(),
        TranslateModule.forRoot({
            loader: {
                provide: TranslateLoader,
                useFactory: HttpLoaderFactory,
                deps: [HttpClient],
            },
            defaultLanguage: 'en',
            useDefaultLang: true,
        }),
    ],
    providers: [
        {
            provide: HTTP_INTERCEPTORS,
            useClass: AuthInterceptor,
            multi: true,
        },
        {
            provide: ErrorHandler,
            useClass: GlobalErrorHandler,
        },
    ],
    bootstrap: [AppComponent],
})
export class AppModule { }
