import { required, prop } from '@rxweb/reactive-form-validators';

import { Injectable } from '@angular/core';

@Injectable()
export class PaymentModel {

  @required()
  @prop()
  selectedCard: number = 0;
  @prop()
  isAutoRenew: boolean = false;
  @required()
  @prop()
  selectedType: number = 0;
  @prop()
  invoice: boolean = false;
  @prop()
  cardName: string = "";
}

export class GlobalPaaymentDetail {
  stripe: any;
  card: any;
  cardId: string = '';
  token: string = '';
  isAutoRenew: boolean;
  IsAuto: boolean = false;
  IsAutoCHK: boolean = false;
  invoice: boolean = false;
  cardName: string = "";
}

/**
 * saved card model
 * */
export class CardDataModel {
  id: string;
  last4: string;
  expMonth: number;
  expYear: number;
  brand: string;
  name: string;
}


/**
 * get credit card details based on email id
 * */
export class RequestGetCreditCards {
  email: string;
  instanceType: number;
}