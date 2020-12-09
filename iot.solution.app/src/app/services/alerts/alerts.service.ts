import { Injectable } from '@angular/core';
import { CookieService } from 'ngx-cookie-service';
import { HttpClient } from '@angular/common/http';
import * as moment from 'moment'
import { ApiConfigService, NotificationService } from '..';

@Injectable({
  providedIn: 'root'
})
export class AlertsService {
  protected apiServer = ApiConfigService.settings.apiServer;
  cookieName = 'FM';
  constructor(
    private cookieService: CookieService,
    private httpClient: HttpClient,
    private _notificationService: NotificationService) {
    this._notificationService.apiBaseUrl = this.apiServer.baseUrl;
  }

  removeImage(entityId) {
    return this.httpClient.put<any>(this.apiServer.baseUrl + 'api/entity/deleteimage?id='+entityId,{}).map(response => {
      return response;
    });
  }

  getAlerts(parameters) {
   
    var param = {
      "pageNo": parameters.pageNo + 1,
      "pageSize": parameters.pageSize,
      "searchText": parameters.searchText,
      "orderBy": parameters.orderBy,
      "fleetGuid": parameters.fleetGuid,
      "currentDate": parameters.currentDate,
      "timeZone": parameters.timeZone,
    }

    return this.httpClient.post<any>(this.apiServer.baseUrl + 'api/alert', param).map(response => {
      return response;
    });
  }
  getMaplist(parameters) {
   
    return this.httpClient.post<any>(this.apiServer.baseUrl + 'api/fleet/maplist', parameters).map(response => {
      return response;
    });
  }
 
 
}
