import { Injectable } from '@angular/core';
import { environment } from '../environments/environment';

@Injectable()
export class ConfigService {

    constructor() { }

    public get getAdalConfig(): any {
        return {
            tenant: environment.tenant,
            clientId: environment.clientId,
            redirectUri: window.location.origin + '/',
            postLogoutRedirectUri: window.location.origin + '/'
        };
    }
} 
