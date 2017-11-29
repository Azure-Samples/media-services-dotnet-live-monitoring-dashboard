import { Routes } from '@angular/router';

import { HomeComponent } from './home/home.component';
import { OAuthCallbackComponent } from './oauth-callback/oauth-callback.component'
import { LoginComponent } from './login/login.component';

import { EnsureAuthenticatedGuard } from './ensure-authenticated.guard';
import { OAuthCallbackGuard } from './oauth-callback.guard';

export const rootRouterConfig: Routes = [
    { path: 'id_token', component: OAuthCallbackComponent, canActivate: [OAuthCallbackGuard] },
    { path: 'action_token', component: OAuthCallbackComponent, canActivate: [OAuthCallbackGuard] },
    { path: '', redirectTo: "home", pathMatch: "prefix" },
    { path: 'home', component: HomeComponent, canActivate: [EnsureAuthenticatedGuard] },
    { path: 'login', component: LoginComponent}
];
