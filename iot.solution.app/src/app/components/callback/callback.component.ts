import { Component, OnInit, ViewEncapsulation } from '@angular/core'
import { FormControl, FormGroup, Validators } from '@angular/forms'
import { Router } from '@angular/router'
import { NgxSpinnerService } from 'ngx-spinner'
import { UserService, AuthService, Notification, NotificationService } from '../../services/index'
import { IDSAuthService } from '../../services/auth/idsauth.service'


@Component({
  selector: 'app-callback',
  templateUrl: './callback.component.html',
  styleUrls: ['./callback.component.css']
})
export class CallbackComponent implements OnInit {
  loginform: FormGroup;
  checkSubmitStatus = false;
  loginObject = {};
  loginStatus = false;
  loggedIn: Boolean;
  constructor(
    private spinner: NgxSpinnerService,
    private router: Router,
    private _notificationService: NotificationService,
    public UserService: UserService,
    private IdsService: IDSAuthService
  ) {

  }

  ngOnInit() {
    let isLoggedIn = this.IdsService.endSigninMainWindow();
    isLoggedIn
      .subscribe((loggedin) => {
        if (loggedin.access_token) {
          this.UserService.getUserDetailByToken(loggedin.access_token).subscribe(response => {
            if (response.isSuccess === true) {
              let data = {
                "access_token": loggedin.access_token,
                "userDetail": {
                  "id": response.data.data.userGuid,
                  "companyId": response.data.data.companyGuid,
                  "roleId": response.data.data.roleGuid,
                  "roleName": response.data.data.roleName,
                  "cpId": response.data.data.cpId,
                  "entityGuid": response.data.data.entityGuid,
                  "solutionGuid": response.data.data.solutionGuid,
                  "fullName": response.data.data.firstName + ' ' + response.data.data.lastName
                }
              }
              localStorage.setItem('currentUser', JSON.stringify(data));
              this._notificationService.add(new Notification('success', 'Logged in successfully'));
              this.router.navigate(['/dashboard']);
            }
            else {
              this.IdsService.endSignoutMainWindow();
              this._notificationService.add(new Notification('error', response.message));
              this.router.navigate(['/']);
            }
          }, error => {
            this.spinner.hide();
            this._notificationService.add(new Notification('error', error));
          });
        }
      })
    //this.router.navigate(['/dashboard']);
  }
}
