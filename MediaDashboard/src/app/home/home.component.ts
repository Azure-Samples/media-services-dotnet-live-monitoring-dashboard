import { Component, OnInit } from '@angular/core';
import { DashboardService } from '../dashboard/dashboard.service';
import { CustomerGroup } from '../dashboard/customer-group';
import {} from 'rxjs/observable/TimerObservable'
import { Observable } from 'rxjs/Rx';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {

  private customerGroups: CustomerGroup[];
  
  constructor(private dashboard: DashboardService) { }

  ngOnInit() {
    Observable.interval(30000).flatMap(() => this.dashboard.getCustomerGroups()).subscribe(groups => this.customerGroups = groups);
  }

}
