import { prop, minDate, maxDate, required, alphaNumeric, alpha, pattern, maxLength, minLength } from '@rxweb/reactive-form-validators';

export class RequestSubscriberFormModel {
  // @required({ message: "Please Select Environment" })
  // @prop()
  // solutionId: string;

  @maxLength({ value: 50, message: 'You have reached the maximum character limit for this field.' })
  @required({ message: 'Please enter First Name' })
  @pattern({ expression: { 'name': /^[ a-zA-Z0-9.]*$/ }, message: "Please enter valid First Name" })
  firstName: string;

  @maxLength({ value: 50, message: 'You have reached the maximum character limit for this field.' })
  @required({ message: 'Please enter Last Name' })
  @pattern({ expression: { 'name': /^[ a-zA-Z0-9.]*$/ }, message: "Please enter valid Last Name" })
  lastName: string;

  @required({ message: "Please enter Email Address" })
  @maxLength({ value: 50, message: 'You have reached the maximum character limit for this field.' })
  // /^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+.[a-zA-Z0-9-.]+$/
  @pattern({
    expression: {
      'email': /^(([^<>()\[\]\\.,;:\s@"]+(\.[^<>()\[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/
      
    }, message: "Invalid Email"
  })
  email: string;

  @maxLength({ value: 50, message: 'You have reached the maximum character limit for this field.' })
  @required({ message: "Please enter Password" })
  @pattern({
    expression: {
      'password': /^(?!.* )(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9])(?=.*[!@#$%^&*])[A-Za-z\d$@$!@#$%^&*].{7,20}$/
    }, message: "Password between 8 and 20 characters; must contain at least one lowercase letter, one uppercase letter, one numeric digit, and one special character, but cannot contain whitespace."
  })
  password: string;

  @required({ message: "Please enter Phone Number" })
  //@pattern({ expression: { 'companyPhone': /^[\+\d]?(?:[\d-.\s()]*)$/i } })
  @pattern({
    expression: {
      'phone': /^(?!0+$)\d{8,}$/i
    }, message: "Please enter valid Phone Number."
  })
  @minLength({ value: 8,message:'please enter minimum 8 digit' })
  @maxLength({ value: 10,message:'please enter maximum 10 digit' })
  phone: string = "";

  @required({ message: "Please select CountryCode" })
  //@pattern({ expression: { 'companyPhone': /^[\+\d]?(?:[\d-.\s()]*)$/i } })
  // @pattern({
  //   expression: {
  //     'phoneNumber': /^[0-9]*$/i
  //   }, message: "Please enter valid Phone Number."
  // })
  // @minLength({ value: 8,message:'please enter minimum 8 digit' })
  // @maxLength({ value: 10,message:'please enter maximum 10 digit' })
  phoneCountryCode: string = "+61";

  // @required({ message: "Please enter Country Code." })
  // //@pattern({ expression: { 'companyPhone': /^[\+\d]?(?:[\d-.\s()]*)$/i } })
  // @pattern({
  //   expression: {
  //     'ccode': /^[0-9]*$/i
  //   }, message: "Please enter Country Code."
  // })
  // @maxLength({ value: 3,message:'please enter maximum 3 digit' })
  // ccode: string = "";




  @maxLength({ value: 50, message: 'You have reached the maximum character limit for this field.' })
  @required({ message: 'Please enter City Name' })
  @pattern({ expression: { 'name': /^[ a-zA-Z0-9.]*$/ }, message: "Please enter valid City name" })
  cityName: string;
  // @prop()
  // @required({message: "Enter Postal Code" })
  // @numeric({ acceptValue:NumericValueType.PositiveNumber  ,allowDecimal:false ,message: "Invalid Postal Code"}) 
  // postalCode: string;

  @prop()
  @maxLength({ value: 7, message: 'You have reached the maximum character limit for this field.' })
  @required({ message: "Please enter Postal Code" })
  @pattern({
    expression: {
      'companyPostalCode': /^[A-Z0-9 _]/
    }, message: "Please enter valid Postal Code"
  })
  // @alphaNumeric({ allowWhiteSpace: true, message: "Please enter valid Postal code" })
  postalCode: string;

  @maxLength({ value: 50, message: 'You have reached the maximum character limit for this field.' })
  @required({ message: 'Please enter Company Name' })
  @pattern({ expression: { 'name': /^[ a-zA-Z0-9.]*$/ }, message: "Please enter valid Company Name" })
  companyName: string;

  @required({ message: "Please enter Address" })
  // @alphaNumeric({ allowWhiteSpace: true, message: "" })
  //@alpha({ allowWhiteSpace: true, message: "Invalid Address" })
  address: string;
  @required({ message: "Please Select Time Zone" })
  timezoneId: string;
  @prop()
  isSameSolutionInstance: boolean = false;

  // @required({ message: "Please Select Product" })
  // @prop()
  // productCode: string;
  @required({ message: "Please Select Country" })
  countryName: string;
  @required({ message: "Please Select State" })
  stateName: string;
}
