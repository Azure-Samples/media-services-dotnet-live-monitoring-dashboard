/// <reference types="adal" />
import { ConfigService } from './config.service';
import { Injectable } from '@angular/core';
import * as adalLib from 'adal-angular';
import { Observable } from 'rxjs/Observable';

let createAuthContextFn: adal.AuthenticationContextStatic = adalLib;

@Injectable()
export class AdalService {

    private context: adal.AuthenticationContext;

    constructor(private configService: ConfigService) {
        this.context = new createAuthContextFn(configService.getAdalConfig);
    }

    login() {
        this.context.login();
    }

    logout() {
        this.context.logOut();
    }

    handleCallback() {
        this.context.handleWindowCallback();
    }

    public get userInfo(): adal.User {
        return this.context.getCachedUser();
    }

    public get accessToken(): string {
        return this.context.getCachedToken(this.configService.getAdalConfig.clientId);
    }

    public get isAuthenticated() {
        const user = this.context.getCachedUser();
        return !!user;
    }

    public getResourceForEndpoint(endpoint: string) : string {
        return this.context.getResourceForEndpoint(endpoint);
    }

    public getAccessToken(resource: string) : Observable<string> {
        var token = this.context.getCachedToken(resource);
        if (!token) {
            const callback = this.context.acquireToken.bind(this.context, resource);
            const acquireToken = Observable.bindCallback(callback, this.acquireToken);
            return acquireToken();
        }
        return Observable.of(token);
    }

    private acquireToken(message: string, token: string): string {
        if (message) throw message;
        return token;
    }
}
