import { FormGroup, ValidationErrors } from '@angular/forms';

export class CustomValidators {

  // custom validator to check that two fields match
  static matchPwds(controlName: string, matchingControlName: string) {
    return (formGroup: FormGroup) => {
      const control = formGroup.controls[controlName];
      const matchingControl = formGroup.controls[matchingControlName];

      if (matchingControl.errors && !matchingControl.errors.mustMatch) {
        // return if another validator has already found an error on the matchingControl
        return;
      }

      // set error on matchingControl if validation fails
      if (control.value !== matchingControl.value) {
        matchingControl.setErrors({ mustMatch: true });
      } else {
        matchingControl.setErrors(null);
      }
    }

  }

  // check for the valid phone no for text mask
  static checkPhoneValue(matchingControlName: string) {
    return (formGroup: FormGroup) => {
      const matchingControl = formGroup.controls[matchingControlName];
      if (matchingControl.errors && !matchingControl.errors.pattern) {
        // return if another validator has already found an error on the matchingControl
        return;
      }
      // set error on matchingControl if validation fails
      if (matchingControl.value.indexOf('_') !== -1 || matchingControl.value.length === 0) {
        matchingControl.setErrors({ pattern: true });
      } else {
        matchingControl.setErrors(null);
      }
    }
  }


}