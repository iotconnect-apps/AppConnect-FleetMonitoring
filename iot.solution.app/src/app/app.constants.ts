import { Injectable } from '@angular/core';
import { FormGroup, FormArray, FormControl } from '@angular/forms';

@Injectable()
export class AppConstant {

  //#common constant
  public activeStatus = "Active";
  public inactiveStatus = "Inactive";
  public Status = "Acquired";
  public inStatus = "Released";
  //# "Common message"
  public tokenInValidMessage = 'Invalid Token.';
  public unauthorizedMessage = 'Unauthorized access';
  public serverErrorMessage = 'Server Error';
    //# "Maintenance date validation range in min"
    public minGap=15; 


  public msgAdded = 'modulename added successfully.';
  public msgCreated = 'modulename created successfully.';
  public msgUpdated = 'modulename updated successfully.';
  public msgStatusChange = 'modulename status changed successfully.';
  public msgCreatedError = ' created with error.';
  public msgUpdatedError = ' updated  with error.';
  public msgDeleted = 'modulename deleted successfully.';
  public msgRestore = ' restore successfully.';
  public errorMessage = 'Please try again.';
  public msgConfirm = 'Are you sure you want to delete this modulename?';
  public msgStatusConfirm = 'Are you sure you want to statusname fieldname?';
  public username = "";
  public noImg = "../../../../assets/images/noimage.svg";

  /**
   * Check if string is empty or not
   * @param obj
   */
  isEmptyString(obj: string) {
    if (obj !== undefined && obj != null && obj != '')
      return false;
    return true;
  }

  
  /**
   * Check model is empty or not
   * @param obj
   */
  isEmptyObject<T>(obj: T): boolean {
    if (obj !== undefined && obj != null)
      return false;
    return true;
  }

  
  /**
   * Check if string has empty guid or not
   * @param obj
   */
  isEmptyGuid(obj: string) {
    if (obj !== undefined && obj != null && obj != '' && obj != '00000000-0000-0000-0000-000000000000')
      return false;
    return true;
  }

  
  /**
   * Check if number is not undefined, null and 0
   * @param obj
   */
  isEmptyNumber(obj: number) {
    if (obj !== undefined && obj != null && obj != 0)
      return false;
    return true;
  }


  /**
   * Check array contains data ot not
   * @param obj
   */
  isEmptyList(obj: any[]) {
    if (obj !== undefined && obj != null && obj.length > 0)
      return false;
    return true;
  }

  /**
   * Check form array is empty or not
   * @param obj
   */
  isEmptyFormArray(obj: FormArray) {
    if (obj !== undefined && obj != null && obj.length > 0)
      return false;
    return true;
  }

  /**
   * Check form string is valid json or not
   * @param obj json string
   */
  isJsonStringValid(obj: string): boolean {
    try {
      JSON.parse(obj);
    } catch (ex) {
      return false;
    }

    return true;
  }

}

/**
 * Interface of Confirm alert model
 */
export interface DeleteAlertDataModel {
	title: string;
	message: string;
	okButtonName: string;
	cancelButtonName: string ;
}

/**
 * Interface of Confirm alert model
 */
export interface MessageAlertDataModel {
  title: string;
  message: string;
  message2: string;
  okButtonName: string;
}

/**
 * Status list (Like active and inactive)
 */
export interface StatusList {
	id: boolean;
	status: string;
}

/**
 * Instance type (Will use for stripe as well)
 */
export enum IotConnectInstancesType {
  /// <summary>
  /// Development / Sandbox
  /// </summary>
  Development = 1,

  /// <summary>
  /// Production or live environemnt
  /// </summary>
  Production = 2,
}
