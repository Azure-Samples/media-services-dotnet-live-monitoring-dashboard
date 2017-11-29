import { Component, OnInit } from '@angular/core';
import { RouterLink } from '@angular/router';
import { AdalService } from '../adal.service';
import { DashboardService } from '../dashboard/dashboard.service';

@Component({
  selector: 'app-navbar',
  templateUrl: './navbar.component.html',
  styleUrls: ['./navbar.component.css']
})
export class NavbarComponent implements OnInit {

  public get userInfo(): adal.User {
    return this.adalService.userInfo;
  }

  public get operations() {
    return [];
  }
  
  constructor(private adalService: AdalService, private dashboard: DashboardService) { }

  ngOnInit() {
  }

}
