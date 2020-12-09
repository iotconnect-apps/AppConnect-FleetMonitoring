import { Injectable } from '@angular/core'
import { HttpClient } from '@angular/common/http'
import { CookieService } from 'ngx-cookie-service'
import { ApiConfigService, NotificationService } from 'app/services';
import * as moment from 'moment'

@Injectable()

export class DashboardService {
  cookieName = 'FM';
  protected apiServer = ApiConfigService.settings.apiServer;
  constructor(
    private http: HttpClient,
    private cookieService: CookieService,
    private _notificationService: NotificationService
  ) {
    this._notificationService.apiBaseUrl = this.apiServer.baseUrl;
  }

  getsensorTelemetryData() {
    return this.http.get<any>(this.apiServer.baseUrl + 'api/lookup/attributes').map(response => {
      return response;
    });
  }
  connectionstatus(deviceid) {
    var configHeader = {
      headers: {
        'Content-Type': 'application/json',
        'Authorization': 'Bearer ' + this.cookieService.get(this.cookieName + 'access_token')
      }
    };

    return this.http.get<any>(this.apiServer.baseUrl + 'api/device/connectionstatus/' + deviceid, configHeader).map(response => {
      return response;
    });
  }
  getDashboardStatistics() {
    var configHeader = {
      headers: {
        'Content-Type': 'application/json',
        'Authorization': 'Bearer ' + this.cookieService.get(this.cookieName + 'access_token')
      }
    };

    return this.http.get(this.apiServer.baseUrl + 'api/dashboard/statistics', configHeader).map(response => {
      return response;
    });
  }

  getNotificationList() {
    var configHeader = {
      headers: {
        'Content-Type': 'application/json',
        'Authorization': 'Bearer ' + this.cookieService.get(this.cookieName + 'access_token')
      }
    };

    return this.http.get(this.apiServer.baseUrl + 'api/dashboard/notification', configHeader).map(response => {
      return response;
    });
  }

  getTruckUsage() {
    var configHeader = {
      headers: {
        'Content-Type': 'application/json',
        'Authorization': 'Bearer ' + this.cookieService.get(this.cookieName + 'access_token')
      }
    };

    return this.http.get(this.apiServer.baseUrl + 'api/dashboard/getTruckUsage', configHeader).map(response => {
      return response;
    });
  }

  getTruckActivity() {
    var configHeader = {
      headers: {
        'Content-Type': 'application/json',
        'Authorization': 'Bearer ' + this.cookieService.get(this.cookieName + 'access_token')
      }
    };

    return this.http.get(this.apiServer.baseUrl + 'api/dashboard/getTruckActivity', configHeader).map(response => {
      return response;
    });
  }

  getTruckGraph() {
    var configHeader = {
      headers: {
        'Content-Type': 'application/json',
        'Authorization': 'Bearer ' + this.cookieService.get(this.cookieName + 'access_token')
      }
    };

    return this.http.get(this.apiServer.baseUrl + 'api/dashboard/getTruckGraph', configHeader).map(response => {
      return response;
    });
  }

  getStompCon() {
    var configHeader = {
      headers: {
        'Content-Type': 'application/json',
        'Authorization': 'Bearer ' + this.cookieService.get(this.cookieName + 'access_token')
      }
    };

    return this.http.get(this.apiServer.baseUrl + 'api/dashboard/getStompConfiguration', configHeader).map(response => {
      return response;
    });
  }

  getDeviceAttributeHistoricalData(data) {
    var configHeader = {
      headers: {
        'Content-Type': 'application/json',
        'Authorization': 'Bearer ' + this.cookieService.get(this.cookieName + 'access_token')
      }
    };

    return this.http.post(this.apiServer.baseUrl + 'api/dashboard/getDeviceAttributeHistoricalData', data, configHeader).map(response => {
      return response;
    });
  }

  getSensors(data) {
    var configHeader = {
      headers: {
        'Content-Type': 'application/json',
        'Authorization': 'Bearer ' + this.cookieService.get(this.cookieName + 'access_token')
      }
    };

    return this.http.post(this.apiServer.baseUrl + 'api/dashboard/getDeviceAttributes', data, configHeader).map(response => {
      return response;
    });
  }

  tripStatus(id, data) {
    var configHeader = {
      headers: {
        'Content-Type': 'application/json',
        'Authorization': 'Bearer ' + this.cookieService.get(this.cookieName + 'access_token')
      }
    };

    return this.http.put<any>(this.apiServer.baseUrl + 'api/trip/' + id + '/status', data, configHeader).map(response => {
      return response;
    });
  }

  startSimulator(id, isSalesTemplate = true) {
    var configHeader = {
      headers: {
        'Content-Type': 'application/json',
        'Authorization': 'Bearer ' + this.cookieService.get(this.cookieName + 'access_token')
      }
    };

    return this.http.get<any>(this.apiServer.baseUrl + 'api/trip/startSimulator/' + id + '/' + isSalesTemplate, configHeader).map(response => {
      return response;
    });
  }

  getDashboardoverview() {
    var configHeader = {
      headers: {
        'Content-Type': 'application/json',
        'Authorization': 'Bearer ' + this.cookieService.get(this.cookieName + 'access_token')
      }
    };
    let currentDate = moment(new Date()).format('YYYY-MM-DDTHH:mm:ss');
    var timeZone = moment().utcOffset();
    var path = 'api/dashboard/overview' + '?currentDate=' + currentDate + '&timeZone=' + timeZone;

    return this.http.get<any>(this.apiServer.baseUrl + path, configHeader).map(response => {
      return response;
    });
  }
  getFleetStatusGraph(data) {

    return this.http.post<any>(this.apiServer.baseUrl + 'api/chart/getfleetstatus', data).map(response => {
      return response;
    });
  }
  getenergyusageGraph(data) {

    return this.http.post<any>(this.apiServer.baseUrl + 'api/chart/getenergyusage', data).map(response => {
      return response;
    });
  }
  getfleettypeusage(data) {

    return this.http.post<any>(this.apiServer.baseUrl + 'api/chart/getfleettypeusage', data).map(response => {
      return response;
    });
  }

  getEntityDetails(entityId) {
    var configHeader = {
      headers: {
        'Content-Type': 'application/json',
        'Authorization': 'Bearer ' + this.cookieService.get(this.cookieName + 'access_token')
      }
    };
    let currentDate = moment(new Date()).format('YYYY-MM-DDTHH:mm:ss');
    var timeZone = moment().utcOffset();
    var path = 'api/dashboard/getentitydetail' + '?entityId=' + entityId + '&currentDate=' + currentDate + '&timeZone=' + timeZone;

    return this.http.get<any>(this.apiServer.baseUrl + path, configHeader).map(response => {
      return response;
    });
  }

  getDashboardoverviewByEntity() {
    var configHeader = {
      headers: {
        'Content-Type': 'application/json',
        'Authorization': 'Bearer ' + this.cookieService.get(this.cookieName + 'access_token')
      }
    };
    let currentDate = moment(new Date()).format('YYYY-MM-DDTHH:mm:ss');
    var timeZone = moment().utcOffset();
    var path = 'api/dashboard/overviewbyentity' + '?currentDate=' + currentDate + '&timeZone=' + timeZone;

    return this.http.get<any>(this.apiServer.baseUrl + path, configHeader).map(response => {
      return response;
    });
  }

  getTimeZone() {
    return /\((.*)\)/.exec(new Date().toString())[1];
  }
  getUpcomingMaintenancedate(curdate, timezone, assetGuid?) {
    let payload = {
      deviceGuid: assetGuid,
      currentDate: curdate,
      timeZone: timezone
    };

    return this.http.post<any>(this.apiServer.baseUrl + 'api/devicemaintenance/getscheduledMaintenancedate', payload).map(response => {
      return response;
    });
  }
  getinventoryconsumptionbyproduct(data) {
    var configHeader = {
      headers: {
        'Content-Type': 'application/json',
        'Authorization': 'Bearer ' + this.cookieService.get(this.cookieName + 'access_token')
      }
    };

    return this.http.post<any>(this.apiServer.baseUrl + 'api/chart/getinventoryconsumptionbyproduct', data, configHeader).map(response => {
      return response;
    });
  }
  getinventorystatus(data) {
    var configHeader = {
      headers: {
        'Content-Type': 'application/json',
        'Authorization': 'Bearer ' + this.cookieService.get(this.cookieName + 'access_token')
      }
    };

    return this.http.post<any>(this.apiServer.baseUrl + 'api/chart/getinventorystatus', data, configHeader).map(response => {
      return response;
    });
  }
  getAlertsList(parameters) {
    var configHeader = {
      headers: {
        'Content-Type': 'application/json',
      }
    };

    const parameter = {
      params: {
        'pageNo': parameters.pageNumber + 1,
        'pageSize': parameters.pageSize,
        'searchText': parameters.searchText,
        'orderBy': parameters.sortBy,
        'deviceGuid': parameters.deviceGuid,
        'entityGuid': parameters.entityGuid,
      },
      timestamp: Date.now()
    };
    var reqParameter = Object.assign(parameter, configHeader);

    return this.http.get<any>(this.apiServer.baseUrl + 'api/alert', reqParameter).map(response => {
      return response;
    });
  }
  getAllAlerts() {
    var configHeader = {
      headers: {
        'Content-Type': 'application/json',
        'Authorization': 'Bearer ' + this.cookieService.get(this.cookieName + 'access_token')
      }
    };

    return this.http.get<any>(this.apiServer.baseUrl + 'api/alert', configHeader).map(response => {
      return response;
    });
  }

  geLocationlist(parameters) {
    let currentDate = moment(new Date()).format('YYYY-MM-DDTHH:mm:ss');
    var timeZone = moment().utcOffset().toString();
    const parameter = {
      params: {
        'pageNo': parameters.pageNo + 1,
        'pageSize': parameters.pageSize,
        'searchText': parameters.searchText,
        'currentDate': currentDate,
        'timeZone': timeZone
      },
      timestamp: Date.now()
    };
    var configHeader = {
      headers: {
        'Content-Type': 'application/json',
        'Authorization': 'Bearer ' + this.cookieService.get(this.cookieName + 'access_token')
      }
    };

    return this.http.get<any>(this.apiServer.baseUrl + 'api/entity/search', parameter).map(response => {
      return response;
    });
  }
  deletelocation(locationGuid) {


    return this.http.put<any>(this.apiServer.baseUrl + 'api/entity/delete/' + locationGuid, "").map(response => {
      return response;
    });
  }

  // Convert timestamp ti since time
  timeSince(date) {
    date = new Date(date)
    let minute = 60;
    let hour = minute * 60;
    let day = hour * 24;
    let month = day * 30;
    let year = day * 365;

    let suffix = ' ago';

    let elapsed = Math.floor((Date.now() - date) / 1000);

    if (elapsed < minute) {
      return 'just now';
    }

    // get an array in the form of [number, string]
    let a = elapsed < hour && [Math.floor(elapsed / minute), 'minute'] ||
      elapsed < day && [Math.floor(elapsed / hour), 'hour'] ||
      elapsed < month && [Math.floor(elapsed / day), 'day'] ||
      elapsed < year && [Math.floor(elapsed / month), 'month'] ||
      [Math.floor(elapsed / year), 'year'];

    // pluralise and append suffix
    return a[0] + ' ' + a[1] + (a[0] === 1 ? '' : 's') + suffix;
  }

  transform(number: number): any {
    if (isNaN(number)) return null; // will only work value is a number
    if (number === null) return null;
    if (number === 0) return null;
    let abs = Math.abs(number);
    const rounder = Math.pow(10, 1);
    const isNegative = number < 0; // will also work for Negetive numbers
    let key = '';

    const powers = [
      { key: 'Q', value: Math.pow(10, 15) },
      { key: 'T', value: Math.pow(10, 12) },
      { key: 'B', value: Math.pow(10, 9) },
      { key: 'M', value: Math.pow(10, 6) },
      { key: 'K', value: 1000 }
    ];

    for (let i = 0; i < powers.length; i++) {
      let reduced = abs / powers[i].value;
      reduced = Math.round(reduced * rounder) / rounder;
      if (reduced >= 1) {
        abs = reduced;
        key = powers[i].key;
        break;
      }
    }
    return (isNegative ? '-' : '') + abs + key;
  }

}
