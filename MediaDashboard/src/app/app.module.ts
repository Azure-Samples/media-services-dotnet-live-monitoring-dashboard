import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { HTTP_INTERCEPTORS } from '@angular/common/http';

import { rootRouterConfig } from './app.routes';
import { AppComponent } from './app.component';
import { AdalService } from './adal.service';
import { ConfigService } from './config.service';
import { HomeComponent } from './home/home.component';
import { OAuthCallbackComponent } from './oauth-callback/oauth-callback.component';
import { DetailsComponent } from './details/details.component';
import { AlertsComponent } from './alerts/alerts.component';
import { ConfigureComponent } from './configure/configure.component';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap'
import { NgbModal } from '@ng-bootstrap/ng-bootstrap/modal/modal';
import { PagingComponent } from './paging/paging.component';
import {  EnsureAuthenticatedGuard } from './ensure-authenticated.guard';
import { OAuthCallbackGuard } from './oauth-callback.guard';
import { LoginComponent } from './login/login.component';
import { NavbarComponent } from './navbar/navbar.component';
import { DashboardService } from './dashboard/dashboard.service';
import { HttpClientModule } from '@angular/common/http';
import { AuthInterceptor } from './dashboard/auth-interceptor';

@NgModule({
  declarations: [
    AppComponent,
    HomeComponent,
    OAuthCallbackComponent,
    DetailsComponent,
    AlertsComponent,
    ConfigureComponent,
    PagingComponent,
    LoginComponent,
    NavbarComponent
  ],
  imports: [
      BrowserModule,
      RouterModule.forRoot(rootRouterConfig, { useHash: true, enableTracing: true }),
      NgbModule.forRoot(),
      HttpClientModule
  ],
  providers: [
    {
      provide: HTTP_INTERCEPTORS,
      multi: true,
      useClass: AuthInterceptor
    },
    ConfigService,
    AdalService,
    DashboardService,
    EnsureAuthenticatedGuard,
    OAuthCallbackGuard
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
