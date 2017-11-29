import { Injectable } from '@angular/core';
import { HttpEvent, HttpInterceptor, HttpHandler, HttpRequest, HttpHeaders } from '@angular/common/http';
import { AdalService } from '../adal.service';
import { Observable } from 'rxjs/Observable';

@Injectable()
export class AuthInterceptor implements HttpInterceptor {
    constructor(private adalService: AdalService) {}

    intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
        const resource = this.adalService.getResourceForEndpoint(request.url);
        return this.adalService.getAccessToken(resource).switchMap(token => {
            var authReq = request.clone({setHeaders: { Authroization: `Bearer ${token}`}});
            console.log(`Auth header is ${token}`);
            return next.handle(authReq);
        })
    }
}