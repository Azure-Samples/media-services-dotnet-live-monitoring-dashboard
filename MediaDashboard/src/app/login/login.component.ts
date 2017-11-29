import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router'
import { AdalService } from '../adal.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent implements OnInit {

  constructor(private adalService: AdalService, private router: Router) {}

  ngOnInit() {
    if (!this.adalService.isAuthenticated) {
      this.adalService.login();
    } else {
      this.router.navigate(['home']);
    }
  }

}
