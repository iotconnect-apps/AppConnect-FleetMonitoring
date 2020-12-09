import 'rxjs/add/operator/map'
import * as moment from 'moment'
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http'
import { CookieService } from 'ngx-cookie-service'
import { ApiConfigService, NotificationService } from '..';

@Injectable({
  providedIn: 'root'
})
export class TripService {
  protected apiServer = ApiConfigService.settings.apiServer;
  cookieName = 'FM';
  constructor(private cookieService: CookieService,
    private httpClient: HttpClient,
    private _notificationService: NotificationService) {
    this._notificationService.apiBaseUrl = this.apiServer.baseUrl;
  }

  /**
   * Delete shipment file by fleetId and fileId
   * @param tripId
   * @param fileId
   */
  removeShipmentFile(tripId, fileId) {
    return this.httpClient.put<any>(this.apiServer.baseUrl + 'api/fleet/deletepermissionfile/' + tripId + '/' + fileId, {}).map(response => {
      return response;
    });
  }

  /**
   * Manage trip
   * @param data
   */
  addTrip(data) {
    const formData = new FormData();
    for (const key of Object.keys(data)) {
      const value = data[key];
      if (data[key]) {
        if (key === 'shipmentFiles') {
          for (let i = 0; i < value.length; i++) {
            formData.append(key, value[i]);
          }

        }
        else if (key === 'tripStop') {
          formData.append(key, JSON.stringify(value));
        }
        else if (key === 'startDateTime') {
          var val = moment(value).format('YYYY-MM-DDTHH:mm:ss');
          formData.append(key, val);
        }
        else {
          formData.append(key, value);
        }
      }
    }
    let currentdatetime = moment().format('YYYY-MM-DD[T]HH:mm:ss');
    let timezone = moment().utcOffset();
    formData.append('currrentDate', currentdatetime);
    formData.append('timeZone', timezone.toString());
    return this.httpClient.post<any>(this.apiServer.baseUrl + 'api/trip/manage', formData).map(response => {
      return response;
    });
  }

  /**
   * Get trip list 
   * @param parameters
   */
  getTrip(parameters) {
    let currentdatetime = moment().format('YYYY-MM-DD[T]HH:mm:ss');
    let timezone = moment().utcOffset();
    const parameter = {
      'startDate': parameters.startDate ? moment(parameters.startDate).format('YYYY-MM-DD[T]HH:mm:ss') : parameters.startDate,
      'endDate': parameters.endDate ? moment(parameters.endDate).format('YYYY-MM-DD[T]HH:mm:ss') : parameters.endDate,
      'status': parameters.status,
      'pageNo': parameters.pageNumber + 1,
      'pageSize': parameters.pageSize,
      'searchText': parameters.searchText,
      'orderBy': parameters.sortBy,
      'currentDate': currentdatetime.toString(),
      'timeZone': timezone.toString()

    };

    return this.httpClient.post<any>(this.apiServer.baseUrl + 'api/trip/search', parameter).map(response => {
      return response;
    });
  }

  /**
   * Delete trip by tripGuid
   * @param tripGuid
   */
  deleteTrip(tripGuid) {
    return this.httpClient.put<any>(this.apiServer.baseUrl + 'api/trip/delete?id=' + tripGuid, {}).map(response => {
      return response;
    });
  }

  /**
   * Get trip details by tripGuid
   * @param tripGuid
   */
  getTripDetails(tripGuid) {
    return this.httpClient.get<any>(this.apiServer.baseUrl + 'api/trip/' + tripGuid).map(response => {
      return response;
    });
  }

  getTripDashboardDetail(tripGuid) {
    let currentdatetime = moment().format('YYYY-MM-DD[T]HH:mm:ss');
    let timezone = moment().utcOffset();

    const parameter = {
      'tripGuid': tripGuid,
      'currentDate': currentdatetime.toString(),
      'timeZone': timezone.toString()
    };

    return this.httpClient.post<any>(this.apiServer.baseUrl + 'api/dashboard/gettripdetail', parameter).map(response => {
      return response;
    });
  }

  /**
  * Radius calcualtion
  * @param x
  */
  rad(x) {
    return x * Math.PI / 180;
  }

  /**
  * Calculate totla miles from source to destination
  * @param slat
  * @param slng
  * @param dlat
  * @param dlng
    */
  calculateTotalMiles(sourceLat, sourcelng, destinationLat, destinationLng) {
    var R = 6378137; // Earth’s mean radius in meter
    var dLat = this.rad(sourceLat - destinationLat);
    var dLong = this.rad(sourcelng - destinationLng);
    var a = Math.sin(dLat / 2) * Math.sin(dLat / 2) +
      Math.cos(this.rad(sourceLat)) * Math.cos(this.rad(destinationLat)) *
      Math.sin(dLong / 2) * Math.sin(dLong / 2);
    var c = 2 * Math.atan2(Math.sqrt(a), Math.sqrt(1 - a));
    var d = R * c; // returns the distance in meter
    return d * 0.00062137;
  }

  hoursToDhms(hours) {
    let seconds = Number(hours) * 3600;
    let d = Math.floor(seconds / (3600 * 24));
    let h = Math.floor(seconds % (3600 * 24) / 3600);
    let m = Math.floor(seconds % 3600 / 60);
    let s = Math.floor(seconds % 60);
    m = (s >= 60) ? (m + 1) : m;
    let dDisplay = d > 0 ? d + (d == 1 ? " day, " : " days, ") : "";
    let hDisplay = h > 0 ? h + (h == 1 ? " hour, " : " hours, ") : "";
    let mDisplay = m > 0 ? m + (m == 1 ? " minute " : " minutes ") : "";
    //let sDisplay = s > 0 ? s + (s == 1 ? " second" : " seconds") : "";
    return dDisplay + hDisplay + mDisplay;
  }

  /**
   * Get arrival time based on speed
   * @param speed in km/hr
   * @param distance in km
   * @return arrival time in hour
   */
  calculateArrivalTime(speed, distance) {
    speed = parseFloat(speed);
    distance = parseFloat(distance);
    let distInMeters = (distance * 1000);
    if (distInMeters > 0) {
      if (speed > 0) {
        return distance / speed;
      }
      else {
        //Break or onHolt
        return '';
      }
    }
    else {
      //reached on destination
      return 0;
    }
  }

  endTrip(parameters) {

    return this.httpClient.post<any>(this.apiServer.baseUrl + 'api/trip/updatetripstatus', parameters).map(response => {
      return response;
    });
  }

  startTrip(parameters) {

    return this.httpClient.post<any>(this.apiServer.baseUrl + 'api/trip/starttrip', parameters).map(response => {
      return response;
    });
  }

  /**
   * vehicle direction
   * @param startpoint
   * @param endpoint
   */
  vehicleBearing(startpoint, endpoint) {
    let radians = this.getAtan2((endpoint.lng - startpoint.lng), (endpoint.lat - startpoint.lat));

    var compassReading = radians * (180 / Math.PI);

    var coordNames = ["N", "NE", "E", "SE", "S", "SW", "W", "NW", "N"];
    var coordIndex = Math.round(compassReading / 45);
    if (coordIndex < 0) {
      coordIndex = coordIndex + 8
    };

    return coordNames[coordIndex]; // returns the coordinate value
  }

  /**
   * get Atan2
   * @param y
   * @param x
   */
  getAtan2(y, x) {
    return Math.atan2(y, x);
  }
}
