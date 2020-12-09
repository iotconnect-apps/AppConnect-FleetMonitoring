import { Component, OnInit, ViewEncapsulation } from '@angular/core'
import { FormControl, FormGroup, Validators } from '@angular/forms'
import { Router } from '@angular/router'
import { NgxSpinnerService } from 'ngx-spinner'
import { UserService, NotificationService, Notification, AuthService } from '../../../services/index'


@Component({
  selector: 'app-admin-login',
  templateUrl: './admin-login.component.html',
  styleUrls: ['./admin-login.component.css']
})
export class AdminLoginComponent implements OnInit {
  loginform: FormGroup;
  checkSubmitStatus = false;
  loginObject = {};
  currentUser:any
  constructor(
    private spinner: NgxSpinnerService,
    private router: Router,
    private _notificationService: NotificationService,
    public UserService: UserService,
    public authService: AuthService,
  ) { }

  ngOnInit() {
    this.createFormGroup();
    if(localStorage.getItem("currentUser")){
      this.logout();
    }
    // logout the person when he opens the app for the first time
  }

	logout() {
    this.currentUser = JSON.parse(localStorage.getItem("currentUser"));
		this.authService.logout();
			if(this.currentUser.userDetail.isAdmin){
				this.router.navigate(['/admin'])
			} else {
				this.router.navigate(['/login'])
			}
	  }
  /**
   * Create Form
   * */
  createFormGroup() {
    this.loginform = new FormGroup({
      username: new FormControl('', [Validators.required, Validators.email]),
      password: new FormControl('', [Validators.required]),
    });

    this.loginObject = {
      username: '',
      password: '',
    };
  }

  /**
   * Login with loginObject
   * */
  login() {
    this.checkSubmitStatus = true;
    if (this.loginform.status === "VALID" && this.checkSubmitStatus) {
      this.spinner.show();
      this.UserService.loginadmin(this.loginObject).subscribe(response => {
        this.spinner.hide();
        if (response.isSuccess === true && response.data.access_token) {
          this._notificationService.add(new Notification('success', 'Logged in successfully'));
          this.router.navigate(['admin/dashboard']);
        }
        else {
          this._notificationService.add(new Notification('error', response.message));
          this.router.navigate(['/admin']);
        }
      }, error => {
        this.spinner.hide();
        this._notificationService.add(new Notification('error', error));
      });
    }
  }

  /**
   * Forgot password div show/hide
   * @param $event
   */
  forgotPassword($event) {
    $("#divLoginSection").hide();
    $("#divForgotPwdSection").show();
  }

  /**
   * Forgot password div show/hide
   * @param $event
   */
  forgotPasswordCancel($event) {
    $("#divForgotPwdSection").hide();
    $("#divLoginSection").show();

  }
}
