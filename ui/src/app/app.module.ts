import { NgModule, ErrorHandler } from '@angular/core';

import { AppComponent } from './app.component';
import { SharedModule } from "./shared/shared.component";
import { AppRoutingModule } from "./app-routing.module";
import { LoginModule } from "./auth/login/login.module";
import { RegisterModule } from "./auth/register/register.module";
import { AuthComponent } from "./auth/auth.component";
import { BrowserModule } from "@angular/platform-browser";
import { FormsModule, ReactiveFormsModule } from "@angular/forms";
import { HTTP_INTERCEPTORS, HttpClient, HttpClientModule } from "@angular/common/http";
import { BrowserAnimationsModule } from "@angular/platform-browser/animations";
import { TranslateLoader, TranslateModule, TranslateService } from '@ngx-translate/core';
import { TranslateHttpLoader } from '@ngx-translate/http-loader';
import { BsDatepickerModule, BsLocaleService } from 'ngx-bootstrap/datepicker';
import { languages } from './shared/constants/languages';
import { Language } from './shared/enums/language';
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
            defaultLanguage: languages[Language.EN].shortName,
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
export class AppModule {
    constructor(
        private bsLocaleService: BsLocaleService,
        private translate: TranslateService,
    ) {
        // Set available languages
        translate.addLangs([languages[Language.EN].shortName, languages[Language.RU].shortName]);

        // Set default language
        translate.setDefaultLang(languages[Language.EN].shortName);

        // Try to get language from localStorage or use browser language
        const savedLang = localStorage.getItem('localization');
        if (savedLang) {
            translate.use(savedLang);
        } else {
            const browserLang = translate.getBrowserLang();
            translate.use(browserLang?.match(/en|ru/) ? browserLang : languages[Language.EN].shortName);
        }
    }
}
