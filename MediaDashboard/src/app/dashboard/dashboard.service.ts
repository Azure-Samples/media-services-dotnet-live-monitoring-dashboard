import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs/Observable';
import { CustomerGroup } from './customer-group';
import { DashboardAlert } from './dashboard-alert';
import { AdalService } from '../adal.service';

@Injectable()
export class DashboardService {

  constructor(private http: HttpClient) { }

  public getCustomerGroups(): Observable<CustomerGroup[]> {
    return this.http.get<CustomerGroup[]>('api/CustomerGroups');
  }

  public getDashboardAlerts(): Observable<DashboardAlert[]> {
    return this.http.get<DashboardAlert[]>("api/DashboardAlerts");
  }
}
