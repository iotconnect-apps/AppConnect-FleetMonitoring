import { Component, OnInit, ViewChild, AfterViewInit, OnDestroy, ElementRef, ChangeDetectorRef, Output, EventEmitter, Input } from '@angular/core';
import { Router, NavigationEnd, ActivatedRoute } from '@angular/router';
import { FormBuilder, Validators, FormGroup, FormControl } from '@angular/forms';
import { PaymentModel, GlobalPaaymentDetail } from './payment-model';
import { MatDialog, MatSlideToggleChange } from '@angular/material';
import { RxFormBuilder } from '@rxweb/reactive-form-validators';
import { AppConstant, IotConnectInstancesType } from 'app/app.constants';
import { Notification, NotificationService, SubscriptionService, LookupService } from 'app/services';
import { NgxSpinnerService } from 'ngx-spinner';



declare var Stripe: any;

@Component({
  selector: 'app-payment',
  templateUrl: './payment.component.html'
})
export class PaymentComponent implements AfterViewInit, OnDestroy, OnInit {
  //@Input('isAutoRenew') isAutoRenew: boolean = true;
  //  @Input('isDisableType') isDisableType: boolean = true;
  @Input('IsOpenPage') IsOpenPage: boolean;
  @Input('email') email: string;
  @ViewChild('cardInfo', { static: false }) cardInfo: ElementRef;
  userCardMaster:any;
  myForm: FormGroup;
  stripe: any;
  elements: any;
  card: any;
  cardHandler = this.onChange.bind(this);
  error: string;
  message: string = '';
  module: string = "Card";
  IsAuto: boolean = false;
  IsAutoCHK: boolean = true;
  IsAutoStatus: string = "Off";
  IsInvoice: boolean = false;
  isChangeCard: boolean = false;
  dialogRef: any;
  cardIdChecked: boolean = false;

  constructor(
    public appConstant: AppConstant,
    private router: Router,
    private route: ActivatedRoute,
    private cd: ChangeDetectorRef,
    private fb: FormBuilder,
    private formBuilder: RxFormBuilder,
    public dialog: MatDialog,
    public _paymentService: SubscriptionService,
    public lookupService: LookupService,
    private spinner: NgxSpinnerService,
    private _notificationService: NotificationService) {

  }

  ngOnInit() {
    this._paymentService.paymentData = new GlobalPaaymentDetail();
    let paymentModel = new PaymentModel()
    this.myForm = this.formBuilder.formGroup(paymentModel);
    this.ChangeCard();
  }

  ngAfterViewInit() {
    this.getStripeKey();

  }

  ngOnDestroy() {
    this.card.removeEventListener('change', this.cardHandler);
    this.card.destroy();
  }


  getStripeKey(search?: string) {
    this.spinner.show();
    this.lookupService.getNoAuthSripeKey().subscribe(response => {
      this.spinner.hide();
      if (response.isSuccess === true) {
        this.cardLayout(response.data);
      }
      else {
        this._notificationService.add(new Notification('error', response.message));
      }
    }, error => {
      this.spinner.hide();
      this._notificationService.add(new Notification('error', error));
    });

  }

  cardLayout(instanceType) {
    this.stripe = Stripe(instanceType);
    this.elements = this.stripe.elements();
    const style = {
      base: {
        color: '#32325d',
        fontFamily: '"Helvetica Neue", Helvetica, sans-serif',
        fontSmoothing: 'antialiased',
        fontSize: '16px',
        '::placeholder': {
          color: '#aab7c4'
        }
      },
      invalid: {
        color: '#fa755a',
        iconColor: '#fa755a'
      }
    };
    this.card = this.elements.create('card', { style });
    this.card.mount(this.cardInfo.nativeElement);
    this.card.addEventListener('change', this.cardHandler);
    this._paymentService.paymentData.stripe = this.stripe;
    this._paymentService.paymentData.card = this.card;
  }

  ChangeCard() {
    this.isChangeCard = true;
    this.cardIdChecked = false;
    this.setSelectedValue('');
  }

  onChange({ error }) {
    if (error) {
      this.error = error.message;
    } else {
      this.error = null;
    }
    this.cd.detectChanges();
  }

  setSelectedValue(cardId: string) {
    if (cardId == '') {
      this._paymentService.paymentData.cardId = '';
      this._paymentService.paymentData.token = '';
      this.cardIdChecked = false;

    }
    else {
      this._paymentService.paymentData.cardId = cardId;
      this._paymentService.paymentData.token = '';
      this.cardIdChecked = true;
      this.isChangeCard = false;
    }
  }

  setSelectedValuePayment(flag: boolean) {
    if (flag == true) {
      this.IsInvoice = false;
      this.myForm.patchValue({
        invoice: false
      });
      //this.cardLayout();
    }
    else {
      this.ngAfterViewInit();
      this.IsInvoice = true;
      this.myForm.patchValue({
        invoice: true
      });
      //this._paymentService.paymentData.cardId = cardId;
      //this._paymentService.paymentData.token = '';
    }
  }


  setAutoRenewCheck() {

    var isAuto = this.myForm.value.AutoRenewal == false ? null : this.myForm.value.AutoRenewal;
    if (isAuto != true) {
      this.IsAutoCHK = true;
    }
    else {
      this.IsAutoCHK = false;
    }

  }

  toggle(event: MatSlideToggleChange) {
    if (event.checked) {
      this.IsAutoStatus = "On";
    }
    else {
      this.IsAutoStatus = "Off";
    }
    //var isAuto = this.myForm.value.AutoRenewal == false ? null : this.myForm.value.AutoRenewal;
    var isAuto = event.checked == false ? null : true; // this.myForm.value.AutoRenewal == false ? null : this.myForm.value.AutoRenewal;

    if (isAuto != true) {
      //this.IsAutoCHK = true;
      this.IsAutoCHK = false;

    }
    else {
      //this.IsAutoCHK = false;
      this.IsAutoCHK = true;
    }

  }


  convertToLowercase(text: string): string {
    if (text) {
      return text.toLowerCase().replace(/[\s]/g, '');
    }
  }

  onBlurCardName() {
    this._paymentService.paymentData.cardName = this.myForm.value.cardName;
  }


}

