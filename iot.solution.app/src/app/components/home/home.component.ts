import { Component, OnInit } from '@angular/core';
import { AdminAuthGuard } from './../../services/index'
import { Router } from '@angular/router'

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {
  loginStatus = false;
  constructor(private authService: AdminAuthGuard, private router: Router) { }

  ngOnInit() {
    if (localStorage.getItem('currentUser')) {
      let currentUser = JSON.parse(localStorage.getItem('currentUser'))
      if (!currentUser.userDetail.isAdmin) {
        this.router.navigate(['dashboard']);
      } else {
        this.router.navigate(['/admin/dashboard']);
      }
    }
  }

}
