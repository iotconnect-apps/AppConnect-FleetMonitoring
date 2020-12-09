import { Component, ViewChild, OnInit, AfterViewInit } from '@angular/core';
import { Router } from '@angular/router'
import { NgxSpinnerService } from 'ngx-spinner'
import { RxFormGroup, RxFormBuilder } from '@rxweb/reactive-form-validators';
import { RequestSubscriberFormModel } from './subscriber.model';
import { PurchasePlanComponent } from '../purchase-plan/purchase-plan.component';
import { Notification, NotificationService, UserService, LookupService, AuthService } from '../../../services';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit, AfterViewInit {
  isFormSubmittedSuccessfully: any;
  @ViewChild(PurchasePlanComponent, { static: false }) child: PurchasePlanComponent;

  validateCompany: any = false;
  modulename = "Register";
  SubscriberFormGroup: RxFormGroup;
  SubscriberData: RequestSubscriberFormModel = new RequestSubscriberFormModel();
  checkSubmitStatus = false;
  subscribeObject = {};
  countryList = [];
  stateList = [];
  timezoneList = [];
  isSubmitted = false;
  isShowPayment = false;
  loginStatus = false;
  public mask = {
    guide: true,
    showMask: false,
    keepCharPositions: true,
    mask: ['(', /[0-9]/, /\d/, ')', '-', /\d/, /\d/, /\d/, /\d/, /\d/, /\d/, /\d/, /\d/, /\d/, /\d/]
  };
  constructor(
    private spinner: NgxSpinnerService,
    private router: Router,
    private _notificationService: NotificationService,
    public userService: UserService,
    public lookupService: LookupService,
    private formBuilder: RxFormBuilder,
    private authService: AuthService
  ) { }

  ngOnInit() {
    this.loginStatus = this.authService.isCheckLogin();
    if (this.loginStatus === true) {
      this.router.navigate(['dashboard']);
    }
    this.getTimezoneList();
    this.getcountryList();
    this.createFormGroup();
  }

  ngAfterViewInit() {
  }


  /**
   * Create reactive form group
   */
  createFormGroup() {
    this.SubscriberFormGroup = <RxFormGroup>this.formBuilder.formGroup(this.SubscriberData);
    this.SubscriberFormGroup.patchValue({
    })
  }

  /**
   * Get timezone list
   */
  getTimezoneList() {
    this.spinner.show();
    this.lookupService.getNoAuthTimezoneList().subscribe(response => {
      this.spinner.hide();
      if (response.isSuccess === true) {
        this.timezoneList = response.data.data;
      }
      else {
        this._notificationService.add(new Notification('error', response.message));
      }
    }, error => {
      this.spinner.hide();
      this._notificationService.add(new Notification('error', error));
    });
  }

  /**
   * Get country list
   */
  getcountryList() {
    this.spinner.show();
    this.lookupService.getNoAuthcountryList().subscribe(response => {
      this.spinner.hide();
      if (response.isSuccess === true) {
        this.countryList = response.data.data;
      }
      else {
        this._notificationService.add(new Notification('error', response.message));
      }
    }, error => {
      this.spinner.hide();
      this._notificationService.add(new Notification('error', error));
    });
  }

  /**
   * Get state list based on country id
   * 
   * @param event 
   */
  changeCountry(event) {
    this.stateList = [];
    this.SubscriberFormGroup.controls['stateName'].setValue(null, { emitEvent: true })
    if (event) {
      let id = event.countryId;
      if (id) {
        this.spinner.show();
        this.lookupService.getNoAuthStaetlist(id).subscribe(response => {
          this.spinner.hide();
          if (response.isSuccess === true) {
            this.stateList = response.data.data;
          }
          else {
            this._notificationService.add(new Notification('error', response.message));
          }
        }, error => {
          this.spinner.hide();
          this._notificationService.add(new Notification('error', error));
        });
      }
    }
  }
  /**
    * Do next step payment once subscription form is completed
    */
  doPayment() {
    this.isSubmitted = true;
    if ((this.SubscriberFormGroup.value['companyName'] || this.SubscriberFormGroup.value['email']) && !this.validateCompany) {
      this.validate(this.SubscriberFormGroup.value['companyName'], this.SubscriberFormGroup.value['email']);
    }

    if (!this.SubscriberFormGroup.valid) {
      return;
    }
    if (this.isShowPayment) {
      this.child.onSubmitPayment()
        .catch(err => console.log(err))
        .then((result: any) => (typeof result !== "undefined") ? this.registerUser(result) : false);
    }
  }

  goBack() {
    this.isShowPayment = !this.isShowPayment;
    this.validateCompany = false;
  }

  registerUser(data) {
    let postData = {
      "subscriptionToken": data.subscriptionToken,
      "solutionCode": data.solutionCode,
      "solutionPlanCode": data.solutionPlanCode,
      "isAutoRenew": data.isAutoRenewal,
      "stripeToken": data.stripeToken,
      "stripeCardId": data.stripeCardId,
      "subscriberId": data.subscriberId,
      "user": {
        "firstName": this.SubscriberFormGroup.value['firstName'],
        "lastName": this.SubscriberFormGroup.value['lastName'],
        "email": this.SubscriberFormGroup.value['email'],
        "password": this.SubscriberFormGroup.value['password'],
        "phone": this.SubscriberFormGroup.value['phone'],
        "phoneCountryCode": this.SubscriberFormGroup.value['phoneCountryCode'],
        "companyName": this.SubscriberFormGroup.value['companyName'],
        "address": this.SubscriberFormGroup.value['address'],
        "timezoneId": this.SubscriberFormGroup.value['timezoneId'],
        "countryId": this.SubscriberFormGroup.value['countryName'],
        "stateId": this.SubscriberFormGroup.value['stateName'],
        "cityName": this.SubscriberFormGroup.value['cityName'],
        "postalCode": this.SubscriberFormGroup.value['postalCode'],
        "cpId": ""
      }
    };
    this.spinner.show();
    this.lookupService.postNoAuthRegister(postData).subscribe(response => {
      if (response.isSuccess === true) {
        this._notificationService.add(new Notification('success', 'Registered successfully'));
        this.router.navigate(['/login']);
      }
      else {
        this._notificationService.add(new Notification('error', response.message));
      }
      this.spinner.hide();
    }, error => {
      this.spinner.hide();
      this._notificationService.add(new Notification('error', error));

    });
  }

  /**
   * Validate Company name and email
   * @param name
   * @param email
   */
  validate(name, email) {
    let data = {
      "companyName": name,
      "email": email,
    };
    this.spinner.show();
    this.lookupService.validate(data).subscribe(response => {
      if (response.isSuccess === true) {
        this.spinner.hide();
        if (this.SubscriberFormGroup.valid) {
          this.isShowPayment = true;
          this.validateCompany = true;
        }
      }
      else {
        this.spinner.hide();
        this._notificationService.add(new Notification('error', response.message));
        this.isShowPayment = false;
        this.validateCompany = false;
        return;
      }
    }, error => {
      this.spinner.hide();
      this._notificationService.add(new Notification('error', error));

    });
  }
}
