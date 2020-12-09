import { Injectable } from '@angular/core';
import { GlobalPaaymentDetail } from 'app/components/subscribe/payment/payment-model';

@Injectable({
  providedIn: 'root'
})
export class SubscriptionService {
  public paymentData = new GlobalPaaymentDetail();

  constructor() { }
}
