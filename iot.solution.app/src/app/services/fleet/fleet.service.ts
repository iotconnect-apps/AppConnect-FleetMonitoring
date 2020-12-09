import 'rxjs/add/operator/map'

import { Injectable } from '@angular/core'
import { HttpClient } from '@angular/common/http'
import { CookieService } from 'ngx-cookie-service'
import * as moment from 'moment'
import { NotificationService, ApiConfigService } from '..';

@Injectable({
  providedIn: 'root'
})

export class FleetService {

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
   * Remove fleet image by fleetId
   * @param fleetId
   */
  removeFleetImage(fleetId) {
    return this.httpClient.put<any>(this.apiServer.baseUrl + 'api/fleet/deleteimage?id=' + fleetId, {}).map(response => {
      return response;
    });
  }

  addFleet(data) {

    const formData = new FormData();
    for (const key of Object.keys(data)) {
      const value = data[key];
      if (data[key]) {
        if (key === 'permissionFiles') {
          for (let i = 0; i < value.length; i++) {
            formData.append(key, value[i]);
          }
        }
        else if (key == 'imageFile') {
          formData.append(key, value);
        }
        else if (key === 'devices') {
          formData.append(key, JSON.stringify(value));
        }
        else {
          formData.append(key, value);
        }
      }
    }

    return this.httpClient.post<any>(this.apiServer.baseUrl + 'api/fleet/manage', formData).map(response => {
      return response;
    });
  }

  /**
   * Delete permission file by fleetId and fileId
   * @param fleetId
   * @param fileId
   */
  removePermissionFile(fleetId, fileId) {
    return this.httpClient.put<any>(this.apiServer.baseUrl + 'api/fleet/deletepermissionfile/' + fleetId + '/' + fileId, {}).map(response => {
      return response;
    });
  }

  getFleet(parameter) {

    return this.httpClient.post<any>(this.apiServer.baseUrl + 'api/fleet/search', parameter).map(response => {
      return response;
    });
  }
  getFleetBytrip(parameter) {

    return this.httpClient.post<any>(this.apiServer.baseUrl + 'api/trip/search', parameter).map(response => {
      return response;
    });
  }

  getFleetTripgraph(parameter) {

    return this.httpClient.post<any>(this.apiServer.baseUrl + 'api/chart/getenergyusagebyfleet', parameter).map(response => {
      return response;
    });
  }

  getOdometerreadingbyfleet(parameter) {

    return this.httpClient.post<any>(this.apiServer.baseUrl + 'api/chart/getodometerreadingbyfleet', parameter).map(response => {
      return response;
    });
  }
  getMaintenancelist(parameters) {


    return this.httpClient.post<any>(this.apiServer.baseUrl + 'api/devicemaintenance/search', parameters).map(response => {
      return response;
    });
  }

  getFleetDetails(fleetGuid) {
    return this.httpClient.get<any>(this.apiServer.baseUrl + 'api/fleet/' + fleetGuid).map(response => {
      return response;
    });
  }

  deleteFleet(fleetGuid) {
    let currentdatetime = moment().format('YYYY-MM-DD[T]HH:mm:ss');
    let timezone = moment().utcOffset();

    const parameter = {
      'guid': fleetGuid,
      'currentDate': currentdatetime.toString(),
      'timeZone': timezone.toString()
    };
    return this.httpClient.put<any>(this.apiServer.baseUrl + 'api/fleet/delete', parameter, {}).map(response => {
      return response;
    });
  }

  getFleetdashdetail(fleetGuid) {
    let currentdatetime = moment().format('YYYY-MM-DD[T]HH:mm:ss');
    let timezone = moment().utcOffset();

    const parameter = {
      'fleetGuid': fleetGuid,
      'currentDate': currentdatetime.toString(),
      'timeZone': timezone.toString()
    };

    return this.httpClient.post<any>(this.apiServer.baseUrl + 'api/dashboard/getfleetdetail', parameter).map(response => {
      return response;
    });
  }

  getFleetlivedashdetail(fleetGuid) {
    let currentdatetime = moment().format('YYYY-MM-DD[T]HH:mm:ss');
    let timezone = moment().utcOffset();

    const parameter = {
      'fleetGuid': fleetGuid,
      'currentDate': currentdatetime.toString(),
      'timeZone': timezone.toString()
    };

    return this.httpClient.post<any>(this.apiServer.baseUrl + 'api/dashboard/getfleetdetail', parameter).map(response => {
      return response;
    });
  }
}
