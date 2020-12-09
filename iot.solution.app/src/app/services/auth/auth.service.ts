import { Injectable } from '@angular/core'
import { CanActivate, Router } from '@angular/router'
import { BehaviorSubject } from 'rxjs';


@Injectable({
  providedIn: 'root'
})

export class AuthService implements CanActivate {
  constructor(private router: Router) { }

  canActivate() {
    if (this.isCheckLogin()) {
      return true;
    } else {
      this.removeAllStorage();
      this.router.navigate(['/admin']);
      return false;
    }
  }

  isCheckLogin() {
    // check if the user is logged in
    if (localStorage.getItem('currentUser')) {
      let currentUser = JSON.parse(localStorage.getItem('currentUser'))
      if (currentUser.userDetail.isAdmin) {
        return true;
      }
      return false;
    }
  }

  logout() {
    this.removeAllStorage();
  }

  removeAllStorage() {
    localStorage.clear();
  }

  public userFullName = new BehaviorSubject('');
  updateUserNameData = this.userFullName.asObservable();

  changeUserNameData(data: any) {
    this.userFullName.next(data);
  }
}

export class AdminAuthGuard implements CanActivate {
  constructor(private router: Router) { }

  canActivate() {
    if (this.isCheckLogin()) {
      return true;
    } else {
      this.removeAllStorage();
      this.router.navigate(['/login']);
      return false;
    }
  }

  isCheckLogin() {
    // check if the user is logged in
    if (localStorage.getItem('currentUser')) {
      let currentUser = JSON.parse(localStorage.getItem('currentUser'))
      if (!currentUser.userDetail.isAdmin) {
        return true;
      }
      return false;
    }

  }

  logout() {
    this.removeAllStorage();
  }

  removeAllStorage() {
    localStorage.clear();
  }

  public userFullName = new BehaviorSubject('');
  updateUserNameData = this.userFullName.asObservable();

  changeUserNameData(data: any) {
    this.userFullName.next(data);
  }
}
