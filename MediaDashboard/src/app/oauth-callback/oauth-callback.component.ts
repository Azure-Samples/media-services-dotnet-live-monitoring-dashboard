import { Component, OnInit } from '@angular/core';
import { AdalService } from '../adal.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-oauth-callback',
  template: '<div>Please wait while logging in...</div>'
})
export class OAuthCallbackComponent implements OnInit {

  constructor(private adalService: AdalService, private router: Router) { }

  ngOnInit() {
    if (!this.adalService.isAuthenticated) {
      this.router.navigate(['login']);
    } else {
      this.router.navigate(['home']);
    }
  }
}
