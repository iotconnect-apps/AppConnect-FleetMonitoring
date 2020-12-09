import 'rxjs/add/operator/map'

import { Injectable } from '@angular/core'
import { HttpClient } from '@angular/common/http'
import { CookieService } from 'ngx-cookie-service'
import { ApiConfigService, NotificationService } from 'app/services';
import * as moment from 'moment'
import { isNullOrUndefined } from 'util';


@Injectable({
  providedIn: 'root'
})
export class DriverService {

  cookieName = 'FM';
  protected apiServer = ApiConfigService.settings.apiServer;
  constructor(
    private cookieService: CookieService,
    private httpClient: HttpClient,
    private _notificationService: NotificationService
  ) {
    this._notificationService.apiBaseUrl = this.apiServer.baseUrl;
  }

  /**
   * Get driver list
   * @param parameters
   */
  getDriverList(parameters) {

    return this.httpClient.post<any>(this.apiServer.baseUrl + 'api/driver/search', parameters).map(response => {
      return response;
    });
  }

  /**
   * Driver add update 
   * @param data
   */
  manageDriver(data: any) {

    const formData = new FormData();
    for (const key of Object.keys(data)) {
      const value = data[key];
      if (data[key])
        formData.append(key, value);
    }

    return this.httpClient.post<any>(this.apiServer.baseUrl + "api/driver/manage", formData).map(response => {
      return response;
    });
  }

  /**
   * Get driver details by driverGuid
   * @param driverGuid
   */
  getDriverDetails(driverGuid) {
    return this.httpClient.get<any>(this.apiServer.baseUrl + 'api/driver/' + driverGuid).map(response => {
      return response;
    });
  }

  /**
   * Delete driver by driverGuid
   * @param driverGuid
   */
  deleteDriver(driverGuid) {

    return this.httpClient.put<any>(this.apiServer.baseUrl + 'api/driver/delete?id=' + driverGuid, "").map(response => {
      return response;
    });
  }

  /**
  * Delete driver image by driverGuid
  * @param driverGuid
  */
  deleteDriverImage(driverGuid) {

    return this.httpClient.put<any>(this.apiServer.baseUrl + 'api/driver/deleteimage?id=' + driverGuid, "").map(response => {
      return response;
    });
  }

  /**
 * Delete licence image by driverGuid
 * @param driverGuid
 */
  deleteLicenceImage(driverGuid) {

    return this.httpClient.put<any>(this.apiServer.baseUrl + 'api/driver/deletelicenceimage?id=' + driverGuid, "").map(response => {
      return response;
    });
  }

  /**
   * Get driver dashboard details by driverGuid
   * @param driverGuid
   */
  getDriverDashboardDetail(driverGuid) {
    let currentdatetime = moment().format('YYYY-MM-DD[T]HH:mm:ss');
    let timezone = moment().utcOffset();

    const parameter = {
      'driverGuid': driverGuid,
      'currentDate': currentdatetime.toString(),
      'timeZone': timezone.toString()
    };

    return this.httpClient.post<any>(this.apiServer.baseUrl + 'api/dashboard/getdriverdetail', parameter).map(response => {
      return response;
    });
  }

  /**
   * Get trips by driver
   * @param data
   */
  getTripsByDriver(data) {

    return this.httpClient.post<any>(this.apiServer.baseUrl + 'api/chart/gettripsbydriver', data).map(response => {
      return response;
    });
  }

  /**
   * Change Driver status
   * @param driverId
   * @param isActive
   */
  changeStatus(driverId, isActive) {
    let status = isActive == true ? false : true;
    return this.httpClient.put<any>(this.apiServer.baseUrl + 'api/driver/updatestatus?id=' + driverId + '&status=' + status, {}).map(response => {
      return response;
    });
  }
}
