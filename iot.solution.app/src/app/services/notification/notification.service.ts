import { Injectable } from '@angular/core'
import { Subject } from 'rxjs/Subject'
import { FormControl } from '@angular/forms';

export class Notification {
  constructor(
    public type: string = '',
    public message: string = ''
  ) { }
}

@Injectable({
  providedIn: 'root'
})

export class NotificationService {
  refreshTokenInProgress = false;
  apiBaseUrl = '';
  constructor() { }

  private _notifications = new Subject<Notification>();
  public noteAdded = this._notifications.asObservable();

  public add(notification: Notification) {
    if (notification.message !== "Unauthorized") {
      this._notifications.next(notification);
    }
  }
  public handleResponse(response,type){
		let dataError:any=[];
		if(response.data && response.data != "00000000-0000-0000-0000-000000000000") {
			if(response.data.length && response.data.length > 0){
				response.data.forEach(element => {
					dataError.push(element.message)
				  });
			}
		} else {
		  dataError[0]=response.message
		}
		if(dataError.length > 0){
			this.add(new Notification(type, dataError));
		}
	}
  public ValidatorFn(control: FormControl) {
    if (control.value) {
      const isWhitespace = (control.value || '').trim().length === 0;
      const isValid = !isWhitespace;
      return isValid ? null : { 'whitespace': true };
    }
  };
}

export class ResponseData {
  success: boolean;
  message: any;
  isflash: boolean;
  imageUrlPath: string;
  img: string;
  image: string;
  id: string;
  cms_id: string;
}
