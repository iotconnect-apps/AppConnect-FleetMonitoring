import { required, prop, date, maxDate, minDate, propObject } from '@rxweb/reactive-form-validators';

/**
 * Set Grid total count
 */
export interface GridTotalCountDataModel {
  count: number;
}

export class GetSolution {
  data: SolutionDetails[];
  params: GridTotalCountDataModel;
}

export class SolutionDetails {
  id: string = "";
  productCode: string = "";
  productName: string = "";
  solutionTypeDescription: string = "";
}

export class SubscriptionTokenRequest {
  clientId: string = "";
  clientSecret: string = "";
  userName: string = "";
}

export class SubscriberDetails {

  isLinkExpire: boolean = false;
  id: string = "";
  solutionId: string = "";
  solutionCode: string = "";
  solutionName: string = "";
  solutionApiKey: string = "";
  solutionApiSecret: string = "";
  firstName: string = "";
  lastName: string = "";
  fullName: string = "";
  email: string = "";
  solutionAdminEmail: string = "";
  phone: string = "";
  companyName: string = "";
  address: string = "";
  timezoneId: string = "";
  country: string = "";
  state: string = "";
  cityName: string = "";
  postalCode: string = "";

}


export class PurchasePlanRequest {
  solutionCode: string;
  solutionPlanCode: string;
  emailId: string;
  subscriberId: string;
  isAutoRenew: boolean;
  stripeToken: string;
  stripeCardId: string;
  packageName: string;
  packageCode: string;
  oldPackagaeCode: string;
}

export class ResponseChangeRequest {
  packageCode: string = "";
}

export class SolutionDetailsForm {


  @prop()
  selectedSolution: string = "";

  @prop()
  selectedCard: string = '0';

  @required()
  selectedPlanName: string = "";

  @required()
  selectedPlanPrice: string = "";

  @required()
  selectedPlanCode: string = "";

  @prop()
  gridSearchValue?: string;

  @prop()
  isAutoRenewal: boolean = true;

}

/**
 * saved card model
 * */
export class CardDataModel {
  id: string;
  last4: string;
  expMonth: string;
  expYear: string;
  brand: string;
}


/**
 * get all cards
 * */
export class GetCrditCardsListDataModel {
  data: CardDataModel[];
  params: GridTotalCountDataModel;
}

export class SubscriberDetail {
  @prop()
  companyName: string;

  @prop()
  email: string;
  @prop()
  firstName: string;
  @prop()
  lastName: string;
  @prop()
  country: string;
  @prop()
  state: string;
  @prop()
  city: string;
  @prop()
  noOfMessages: number;
  @prop()
  noOfUsers: number;
  @prop()
  noOfDevices: number;
  @prop()
  asa: number;
  @prop()
  mqtt: number;
  @prop()
  timeseries: number;
  @prop()
  ruleEngine: number;
  @prop()
  solutionSync: number;
  @prop()
  connectionStatus: number;
  @prop()
  diagnostic: number;
  @prop()
  command: number;
  @prop()
  twin: number;
  @prop()
  notification: number;
  @prop()
  renewalDate: Date;
  @prop()
  solutionDetails: SolutionDetail[] = new Array<SolutionDetail>();
  @prop()
  planName: string;
  @prop()
  totalMessage: number;
  @prop()
  price: number;
  @prop()
  subscriptionDate: Date;
  @prop()
  planNoOfMessages: number;
  @prop()
  solutionId: string;
  @prop()
  subFeaturesConsumptionDatas: SubFeaturesConsumptionData[] = new Array<SubFeaturesConsumptionData>();

  @prop()
  planPerDayMessages: number;
}

export class SolutionDetail {
  @prop()
  solutionId: string;
  @prop()
  solutionName: string;
  @prop()
  solutionCode: string;
}

export class SubFeaturesConsumptionData {
  @prop()
  subFeatureName: string;
  @prop()
  value: number;

}

export class SubscriberRenewal {
  @prop()
  transactionId: string;

  @date({ message: 'Invalid date', conditionalExpression: x => x.fromDate != null })
  @maxDate({ fieldName: 'toDate', message: "The selected date should have smaller (prior) value than that of the to date.", conditionalExpression: x => x.toDate != null })
  fromDate: Date;

  @date({ message: 'Invalid date', conditionalExpression: x => x.toDate != null })
  @minDate({ fieldName: 'fromDate', message: "The selected date should have larger (later) value than that of the from date", conditionalExpression: x => x.fromDate != null })
  toDate: Date;
  @prop()
  subscriptionPlanName: string;
  @prop()
  status: number;
}

export class GetPlanList {
  data: PlanDetails[];
  params: GridTotalCountDataModel;
}

export class PlanDetails {
  planId: string = "";
  planName: string = "";
  planCode: string = "";
  Subscriber: number = 0;
  planPrice: number = 0;
  consumerPlanPrice: number = 0
  status: boolean = false;
  isPlanSelected: boolean = false;
  isAutoRenew: boolean = false;
  isExpired: boolean = false;
  planFeatures: Features[] = new Array<Features>();
  message: string = "";
  devices: string = "";
  users: string = "";
  connectors: string = "";
  customFeatureCount: number = 0;
  solutionName: string = "";
  isPlanInactiveAndNotExpired: boolean = false;
  instanceType: number;  //0-Sandbox, 1-Production
}

export class Features {
  featureId: string;
  featureName: string = "";
  featureValue: string = "";
  isCustomeFeature: boolean;
  solutionFeaturesId: string;
  displayName: string;
}



export class PlanRenewalHistroy {
  data: PlanRenewalHistroyData[] = new Array<PlanRenewalHistroyData>();
}

export class PlanRenewalHistroyData {
  @prop()
  transactionId: string;
  @prop()
  transactionDateTime: string;
  @prop()
  subscriptionPlanName: string;
  @prop()
  price: number;
  @prop()
  status: number;
}

export interface StatusRenewal {
  id: number
  statusName: string;
}

export class ResponseGetLastSyncSubscriber {
  displayName: string = "";
  displayTime: string = "";
}