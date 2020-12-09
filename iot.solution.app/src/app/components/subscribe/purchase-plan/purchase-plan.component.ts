import { Component, OnInit, ViewChild } from '@angular/core';
import { MatDialog } from '@angular/material';
import {
  SolutionDetailsForm, PurchasePlanRequest, SubscriberDetails,
  PlanDetails, SubscriberDetail
} from './purchase-plan.model';

import { FormGroup, FormBuilder } from '@angular/forms';
import { RxFormBuilder } from '@rxweb/reactive-form-validators';
import { ActivatedRoute, Router } from '@angular/router';
import { NgScrollbar } from 'ngx-scrollbar';
import { NgxSpinnerService } from 'ngx-spinner'


declare var Stripe: any;

import 'rxjs/add/operator/finally';
import { AppConstant } from '../../../app.constants';
import { SubscriptionService, Notification, NotificationService, LookupService } from '../../../services';

@Component({
  selector: 'app-purchase-plan',
  templateUrl: './purchase-plan.component.html',
  styleUrls: []
})
export class PurchasePlanComponent implements OnInit {

  @ViewChild(NgScrollbar, { static: false }) scrollRef: NgScrollbar;

  error: string;
  myForm: FormGroup;
  myFormRenewal: FormGroup;
  message: string = "";
  moduleName: string = "Purchase-Plan";
  solutionPlans: PlanDetails[];
  selectedPlan: PlanDetails = new PlanDetails();
  solutionDetailsForm: SolutionDetailsForm = new SolutionDetailsForm();
  emailId: string = '';
  billingToken: string = "";
  subscriberId: string = "";
  saveData: PurchasePlanRequest = new PurchasePlanRequest();
  isPlanPurchased: boolean = false;
  other: number = 0;
  hide: boolean = false;
  solutionName: string = "";
  subfeaturedisplayIndex: number = -1;
  funCallsCount: number = 0;
  lastSyncDate: string = "";
  currentDate: any = new Date();
  selectedPlanDetail = {};

  constructor(
    public appConstant: AppConstant,
    public route: ActivatedRoute,
    private router: Router,
    public dialog: MatDialog,
    private formBuilder: RxFormBuilder,
    private spinner: NgxSpinnerService,
    public _paymentService: SubscriptionService,
    private _notificationService: NotificationService,
    public lookupService: LookupService
  ) {
  }

  ngOnInit() {
    try {
      this.getPlansForSolution();
      this.myForm = this.formBuilder.formGroup(this.solutionDetailsForm);
    } catch (ex) {
      throw ex;
    }
  }

  /**
   * Get subsccription plans list
   * @param search 
   */
  getPlansForSolution(search?: string) {
    //  let pagedData: any = { "data": [{ "planId": "16E445F8-03E3-43EB-0E80-08D77952FCC2", "planName": "ronakp48plan1", "instance": "", "subscriber": 1, "consumerPlanPrice": 100.00, "status": true, "isPlanSelected": false, "isExpired": false, "expiryDate": "0001-01-01T00:00:00", "isAutoRenew": false, "planCode": "SBPKG00195", "solutionMasterId": "C6621B52-A0E3-485A-23C7-08D7794B720C", "stopSubscirptionAfterMonth": 0, "isPlanInactiveAndNotExpired": false, "isSolutionActive": false, "instanceName": "IOTConnect QA", "solutionName": "ronakp48p1 name1", "envName": "ronakp48e1", "solutionCode": "ronakp48p1", "instanceType": "1", "features": [{ "solutionFeaturesId": "071c0c81-743b-4a89-89cf-a68cbeb0a49b", "featureName": "Messages", "featureValue": "10000", "isCustomeFeature": false, "isInternal": true, "displayName": "Messages", "subFeatures": [] }, { "solutionFeaturesId": "aa4f0694-d7f5-41e4-a2f2-f73b32b721d0", "featureName": "Devices", "featureValue": "10", "isCustomeFeature": false, "isInternal": true, "displayName": "Devices", "subFeatures": [] }], "instanceId": "9316A34C-E149-437E-E3C5-08D74726F669", "totalAvailableMessages": 3650000 }, { "planId": "4AE2B882-5F80-40A2-0E81-08D77952FCC2", "planName": "ronakp48plan2", "instance": "", "subscriber": 3, "consumerPlanPrice": 110.00, "status": true, "isPlanSelected": false, "isExpired": false, "expiryDate": "0001-01-01T00:00:00", "isAutoRenew": true, "planCode": "SBPKG00196", "solutionMasterId": "C6621B52-A0E3-485A-23C7-08D7794B720C", "stopSubscirptionAfterMonth": 0, "isPlanInactiveAndNotExpired": true, "isSolutionActive": false, "instanceName": "IOTConnect QA", "solutionName": "ronakp48p1 name1", "envName": "ronakp48e1", "solutionCode": "ronakp48p1", "instanceType": "1", "features": [{ "solutionFeaturesId": "071c0c81-743b-4a89-89cf-a68cbeb0a49b", "featureName": "Messages", "featureValue": "11000", "isCustomeFeature": false, "isInternal": true, "displayName": "Messages", "subFeatures": [] }, { "solutionFeaturesId": "aa4f0694-d7f5-41e4-a2f2-f73b32b721d0", "featureName": "Devices", "featureValue": "11", "isCustomeFeature": false, "isInternal": true, "displayName": "Devices", "subFeatures": [] }], "instanceId": "9316A34C-E149-437E-E3C5-08D74726F669", "totalAvailableMessages": 4015000 }, { "planId": "99B825A2-207D-4169-15A2-08D77D5568E5", "planName": "plan 1", "instance": "", "subscriber": 0, "consumerPlanPrice": 100.00, "status": true, "isPlanSelected": false, "isExpired": false, "expiryDate": "0001-01-01T00:00:00", "isAutoRenew": false, "planCode": "SBPKG00206", "solutionMasterId": "C6621B52-A0E3-485A-23C7-08D7794B720C", "stopSubscirptionAfterMonth": 0, "isPlanInactiveAndNotExpired": false, "isSolutionActive": false, "instanceName": "IOTConnect QA", "solutionName": "ronakp48p1 name1", "envName": "ronakp48e1", "solutionCode": "ronakp48p1", "instanceType": "1", "features": [{ "solutionFeaturesId": "071c0c81-743b-4a89-89cf-a68cbeb0a49b", "featureName": "Messages", "featureValue": "10000", "isCustomeFeature": false, "isInternal": true, "displayName": "Messages", "subFeatures": [] }, { "solutionFeaturesId": "aa4f0694-d7f5-41e4-a2f2-f73b32b721d0", "featureName": "Devices", "featureValue": "10", "isCustomeFeature": false, "isInternal": true, "displayName": "Devices", "subFeatures": [] }], "instanceId": "9316A34C-E149-437E-E3C5-08D74726F669", "totalAvailableMessages": 3650000 }], "params": { "count": 3 } };
    this.spinner.show();
    this.lookupService.getNoAuthplanslist().subscribe(response => {
      this.spinner.hide();
      if (response.isSuccess === true) {

        // this.solutionPlans = response.data.filter(x => x.status);
        this.solutionPlans = response.data.data;
        this.solutionPlans.forEach(element => {
          if (element.isPlanSelected) {
            this.selectedPlanDetail = element;
            this.myForm.controls.selectedPlanName.patchValue(element.planName);
            this.myForm.controls.selectedPlanPrice.patchValue(element.consumerPlanPrice);
            this.myForm.controls.selectedPlanCode.patchValue(element.planCode);
          }
        });

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
   * confirm payment after a plan and card details is captured 
   */
  async onSubmitPayment() {
    if (this.myForm.valid) {
      this.spinner.show();
      //if (this._paymentService.paymentData.cardId == null || this._paymentService.paymentData.cardId == '') {
      //if (this.appConstant.isEmptyString(this._paymentService.paymentData.cardName)) {
      //  this._notificationService.add(new Notification('error', "Please enter card name."));
      //  this.spinner.hide();
      //  return;
      //} else if (this.appConstant.isEmptyString(this._paymentService.paymentData.cardName.trim())) {
      //  this._notificationService.add(new Notification('error', "Please enter valid card name."));
      //  this.spinner.hide();
      //  return;
      //}


      //const { paymentMethod, error } =
      //  await this._paymentService.paymentData.stripe.createPaymentMethod(
      //    {
      //      type: 'card',
      //      card: this._paymentService.paymentData.card,
      //      billing_details: {
      //        name: this._paymentService.paymentData.cardName,
      //      }

      //    });
      //if (error) {
      //  this.message = error.message;
      //  this._notificationService.add(new Notification('error', this.message));
      //  this.spinner.hide();
      //  return;
      //}
      //else {
      // await this._paymentService.paymentData.stripe.createPaymentMethod(
      //   {
      //     type: 'card',
      //     card: this._paymentService.paymentData.card,
      //     billing_details: {
      //       name: 'Jenny Rosen',
      //     }

      //   });
      let res = {};
      res['subscriptionToken'] = '';
      res['solutionCode'] = this.selectedPlanDetail['solutionCode'];
      res['solutionPlanCode'] = this.myForm.value.selectedPlanCode;
      res['isAutoRenewal'] = this.myForm.value.isAutoRenewal;
      //res['stripeToken'] = paymentMethod.id;
      res['stripeToken'] = '';
      res['stripeCardId'] = this._paymentService.paymentData.cardId;
      res['subscriberId'] = '';
      //res['packageName'] = this.myForm.value.selectedPlanName;
      //res['packageCode'] =this.myForm.value.selectedPlanCode;

      this.spinner.hide();
      return res;
      //}

      //}
      //else {
      //  this.saveData.stripeToken = "";
      //  this.saveData.stripeCardId = this._paymentService.paymentData.cardId;
      //}

    }
    else {
      this._notificationService.add(new Notification('error', "Please select plan"));
      this.spinner.hide();
      return;
    }

  }


  SelectPlan(data) {
    if (data) {
      let selectedPlan = this.solutionPlans.filter(x => x.isPlanSelected);
      if (selectedPlan != null && selectedPlan.length > 0)
        selectedPlan[0].isPlanSelected = false;
      data.isPlanSelected = true;
      this.selectedPlanDetail = data;
      this.myForm.controls.selectedPlanName.patchValue(data.planName);
      this.myForm.controls.selectedPlanPrice.patchValue(data.consumerPlanPrice);
      this.myForm.controls.selectedPlanCode.patchValue(data.planCode);
    }
  }

  onSolutionChange(event) {
    this.router.navigate(["/"]);
  }

  // gridSearch() {
  //   this.getPlansForSolution(this.myForm.value.gridSearchValue);
  // }

  subfeatureList(subfeature: number, data1: any) {

    if (subfeature != this.subfeaturedisplayIndex) {
      this.subfeaturedisplayIndex = subfeature;
    }
    else {
      this.subfeaturedisplayIndex = -1;

    }
  }

}
