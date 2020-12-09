import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router'
import { FormControl, FormGroup, Validators, FormBuilder, AbstractControl } from '@angular/forms'
import { NgxSpinnerService } from 'ngx-spinner'
import { Notification, NotificationService, UserService } from 'app/services';
import { CustomValidators } from 'app/helpers/custom.validators';
import { IDSAuthService } from 'app/services/auth/idsauth.service';

@Component({
  selector: 'app-change-password',
  templateUrl: './change-password.component.html',
  styleUrls: ['./change-password.component.css']
})
export class ChangePasswordComponent implements OnInit {

  userForm: FormGroup;
  moduleName = "Change Password";
  buttonname = "Update";
  checkSubmitStatus = false;
  currentUser: any = {};
  userObject: any = {};
  isAdmin = false;

  constructor (
    private IdsService: IDSAuthService,
    private formBuilder: FormBuilder,
    private router: Router,
    private _notificationService: NotificationService,
    private activatedRoute: ActivatedRoute,
    private spinner: NgxSpinnerService,
    public userService: UserService
  ) {
    this.currentUser = JSON.parse(localStorage.getItem('currentUser'));
    this.createFormGroup();
  }

  ngOnInit() {
    let currentUser = JSON.parse(localStorage.getItem('currentUser'));
    this.isAdmin = currentUser.userDetail.isAdmin;
    this.getCurrentUserInfo();
  }


    /**
   * Get current user info
   */
  getCurrentUserInfo() {
    this.spinner.show();
    let userGuid = this.currentUser.userDetail.id;
    this.userService.getUserDetails(userGuid).subscribe(response => {
      this.spinner.hide();
      if (response.isSuccess === true) {
        this.userObject = response.data;
      } else {
        this._notificationService.add(new Notification('error', response.message));
        this.userObject = {};
      }
    }, error => {
      this.spinner.hide();
      this._notificationService.add(new Notification('error', error));
    });
  }

  /**
   * Create reactive form group
   */
  createFormGroup() {
    this.userForm = this.formBuilder.group({
      oldPassword: ['', Validators.required],
      newPassword: ['', [
          Validators.required, 
          Validators.pattern('(?!.* )(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9])(?=.*[!@#$%^&*])[A-Za-z\d$@$!@#$%^&*].{7,20}')]
      ],
      confirmPassword: [null, Validators.required]
    }, {
      validator: this.passwordConfirming
    });
  }

  passwordConfirming(c: AbstractControl): { invalid: boolean } {
    if (c.get('newPassword').value !== c.get('confirmPassword').value) {
        return {invalid: true};
    }
  }

   /**
   * change user password
   */
  changePassword() {
    this.checkSubmitStatus = true;
    if (this.userForm.status === "VALID") {
      this.spinner.show();
      if(!this.userObject){
        this.spinner.hide();
         this._notificationService.add(new Notification('error', "User not found"));
         return false;
      }
      this.userForm.registerControl("email", new FormControl(''));
      this.userForm.patchValue({"email" : this.userObject.email});
      if(this.currentUser.userDetail.isAdmin){
        let model={
          "email":this.userForm.value.email,
          "oldPassword": this.userForm.value.oldPassword,
          "newPassword": this.userForm.value.newPassword
        }
        this.adminUserChangePassword(model);
      } else {
        this.companyUserChangePassword(this.userForm.value);
      }
    }
  }

  companyUserChangePassword(sendModel){
     this.userService.changePassword(sendModel).subscribe(response => {
        this.spinner.hide();
        if (response.isSuccess === true) {
          this._notificationService.add(new Notification('success', "Password has been changed successfully."));
          localStorage.clear();
          this.IdsService.endSignoutMainWindow();
          this.router.navigate(['/login']);
        } else {
          this.userForm.reset();
          this._notificationService.add(new Notification('error', response.message));
        }
      }, error => {
        this.spinner.hide();
        this.userForm.reset();
        this._notificationService.add(new Notification('error', error));
      })
  }

  adminUserChangePassword(sendModel){
    this.userService.changeAdminPassword(sendModel).subscribe(response => {
       this.spinner.hide();
       if (response.isSuccess === true) {
         this._notificationService.add(new Notification('success', "Password has been changed successfully."));
         localStorage.clear();
         this.router.navigate(['/admin']);
       } else {
         this.userForm.reset();
         this._notificationService.add(new Notification('error', response.message));
       }
     }, error => {
       this.spinner.hide();
       this.userForm.reset();
       this._notificationService.add(new Notification('error', error));
     })
 }

}
