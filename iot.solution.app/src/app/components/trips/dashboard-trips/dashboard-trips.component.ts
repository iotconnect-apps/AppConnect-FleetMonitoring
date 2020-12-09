/// <reference types="@types/googlemaps" />
import { Component, OnInit } from '@angular/core';
import { TripService, Notification, NotificationService, AlertsService, DeviceService } from '../../../services';
import { ActivatedRoute } from '@angular/router';
import { NgxSpinnerService } from 'ngx-spinner';
import { Subscription, Observable } from 'rxjs';
import { Message } from '@stomp/stompjs';
import { StompRService } from '@stomp/ng2-stompjs';
import * as moment from 'moment-timezone';
import { Http } from '@angular/http';
import { DeleteAlertDataModel } from '../../../app.constants';
import { DeleteDialogComponent } from '../..';
import { MatDialog } from '@angular/material';


declare const google: any

@Component({
  selector: 'app-dashboard-trips',
  templateUrl: './dashboard-trips.component.html',
  styleUrls: ['./dashboard-trips.component.css'],
  providers: [StompRService]
})
export class DashboardTripsComponent implements OnInit {

  rightDirections: any = ['N', 'NE', 'E', 'SE'];
  leftDirections: any = ['S', 'SW', 'W', 'NW'];
  truckDirection: any = 'N';
  isStarted: any = false;
  deleteAlertDataModel: DeleteAlertDataModel;
  APIkey = 'b3923e496c69b4992f015198e59947d2';
  URL = 'http://api.openweathermap.org/data/2.5/onecall?';
  lat: number = 0;
  lng: number = 0;
  temperature: number;
  description: any;
  windSpeed: number;
  humidity: number;
  precipitation: number;
  weatherReport: any = {};
  weatherIcon: any = 'weather-scattered-clouds-icon';
  arrivalTime: any = '-';
  tripGuid: any;
  tripDetails: any;
  mediaUrl: any;
  alerts: any = [];
  searchParameters = {
    pageNo: 0,
    pageSize: 10,
    searchText: '',
    orderBy: 'eventDate desc',
    fleetGuid: ''
  }
  totalMiles: number = 0;
  remailingMiles: number = 0;
  coveredMiles: number = 0;
  progressMilesPerc: number = 0;
  totalRecords = 0;
  reportingData: any = {};
  uniqueId: any;
  subscription: Subscription;
  messages: Observable<Message>;

  deviceIsConnected = false;
  isConnected = false;
  flag = false;
  tripflag = false;
  cpId = '';
  isinfo = false;
  subscribed;
  stompConfiguration = {
    url: '',
    headers: {
      login: '',
      passcode: '',
      host: ''
    },
    heartbeat_in: 0,
    heartbeat_out: 2000,
    reconnect_delay: 5000,
    debug: true
  }

  public origin: any;
  public destination: any;
  public renderOptions = {
    suppressMarkers: false,
  }
  public markerOptions = {
    waypoints: {
      icon: 'https://www.shareicon.net/data/32x32/2016/04/28/756617_face_512x512.png',
      draggable: true,
    }

  }
  public icon = {
    url: './assets/images/truck-location-green.png',
    scaledSize: {
      height: 60,
      width: 60
    }
  };

  public waypoints = [
    {
      location: { lat: 24.0784, lng: 85.8307 },
      stopover: true
    },
    {
      location: { lat: 22.5688, lng: 71.8019 },
      stopover: true
    }
  ];
  fleetId: any;
  materialTypeName: any;
  driverGuid: any;
  driverId: any;
  tripStatus: any;
  can_odometer: any;

  constructor(
    public _service: TripService,
    public alertService: AlertsService,
    private activatedRoute: ActivatedRoute,
    private spinner: NgxSpinnerService,
    private _notificationService: NotificationService,
    public alertsService: AlertsService,
    public deviceService: DeviceService,
    private stompService: StompRService,
    public dialog: MatDialog,
    private http: Http
  ) {
    this.activatedRoute.params.subscribe(params => {
      if (params.tripGuid != null) {
        this.tripGuid = params.tripGuid;
        this.getTripDetails(params.tripGuid);
        this.getAlertList();
      }
    });
  }

  ngOnInit() {
    window.scrollTo(0, 0)
    this.mediaUrl = this._notificationService.apiBaseUrl;
  }

  /**
   * Get trip details by tripGuid
   * @param tripGuid
   */
  getTripDetails(tripGuid) {
    this.spinner.show();
    this._service.getTripDashboardDetail(tripGuid).subscribe(response => {
      if (response.isSuccess === true) {
        this.isinfo = true;
        this.tripDetails = response.data;
        this.tripDetails.tripStatus = '(' + this.tripDetails.tripStatus + ')';
        this.fleetId = this.tripDetails.fleetId
        this.tripStatus = this.tripDetails.tripStatus
        this.materialTypeName = this.tripDetails.materialTypeName
        this.driverGuid = this.tripDetails.driverGuid
        this.driverId = this.tripDetails.driverId
        this.origin = { lat: +this.tripDetails.sourceLatitude, lng: +this.tripDetails.sourceLongitude };
        this.destination = { lat: +this.tripDetails.destinationLatitude, lng: +this.tripDetails.destinationLongitude };
        this.lat = +this.tripDetails.sourceLatitude;
        this.lng = +this.tripDetails.sourceLongitude;
        this.uniqueId = this.tripDetails.uniqueId;

        this.truckDirection = this._service.vehicleBearing(this.origin, this.destination);
        if (this.tripStatus == '(Completed)') {
          this.lat = +this.tripDetails.destinationLatitude;
          this.lng = +this.tripDetails.destinationLongitude;
        } else {
          this.lat = +this.tripDetails.sourceLatitude;
          this.lng = +this.tripDetails.sourceLongitude;
        }
        this.totalMiles = this._service.calculateTotalMiles(this.tripDetails.sourceLatitude, this.tripDetails.sourceLongitude, this.tripDetails.destinationLatitude, this.tripDetails.destinationLongitude);
        if (this.tripDetails.tripStatus == '(Completed)') {
          this.progressMilesPerc = 100;
          this.coveredMiles = this.totalMiles;
          this.destination = { lat: this.lat, lng: this.lng };
        }
        if (this.uniqueId && (this.tripStatus != '(Completed)' && this.tripStatus != '(Overdue)')) {
          this.getStompConfig();
        }
        this.getWeatherReport();
      } else {
        this._notificationService.add(new Notification('error', response.message));
      }
      this.spinner.hide();
    }, error => {
      this.spinner.hide();
      this._notificationService.add(new Notification('error', error));
    });
  }

  /**
   * Get weather report by latLng
   * */
  getWeatherReport() {

    this.http.get(this.URL + 'lat=' + this.lat + '&lon=' + this.lng + '&exclude=hourly,daily&APPID=' + this.APIkey).subscribe(data => {
      if (data) {
        this.weatherReport = data.json();
        this.humidity = this.weatherReport.current.humidity;
        this.temperature = this.kelvinToFahrenheit(this.weatherReport.current.temp);
        this.description = this.weatherReport.current.weather[0].description;
        this.precipitation = this.weatherReport.minutely ? this.weatherReport.minutely[0].precipitation : 0;
        this.windSpeed = this.weatherReport.current.wind_speed;
        switch (this.description) {
          case 'overcast clouds':
            this.weatherIcon = 'weather-scattered-clouds-icon';
            break;
          case 'light rain':
            this.weatherIcon = 'weather-shower-rain-icon';
            break;
          case 'mist':
            this.weatherIcon = 'weather-mist-icon';
            break;
          case 'smoke':
            this.weatherIcon = 'weather-smoke-icon';
            break;
          case 'clear sky':
            this.weatherIcon = 'weather-clear-sky-icon';
            break;
          case 'few clouds':
            this.weatherIcon = 'weather-fewcloud-icon';
            break;
          case 'scattered clouds':
            this.weatherIcon = 'weather-scattered-clouds-icon';
            break;
          case 'broken clouds':
            this.weatherIcon = 'weather-broken-clouds-icon';
            break;
          case 'shower rain':
            this.weatherIcon = 'weather-shower-rain-icon';
            break;
          case 'rain':
            this.weatherIcon = 'weather-rain-icon';
            break;
          case 'thunderstorm':
            this.weatherIcon = 'weather-thunderstorm-icon';
            break;
          case 'snow':
            this.weatherIcon = 'weather-snow-icon';
            break;
          default:
            this.weatherIcon = 'weather-scattered-clouds-icon';
            break;
        }
      }
    });
  }

  /**
   * Get stomp config
   * */
  getStompConfig() {

    this.deviceService.getStompConfig('LiveData').subscribe(response => {
      if (response.isSuccess) {
        this.stompConfiguration.url = response.data.url;
        this.stompConfiguration.headers.login = response.data.user;
        this.stompConfiguration.headers.passcode = response.data.password;
        this.stompConfiguration.headers.host = response.data.vhost;
        this.cpId = response.data.cpId;
        this.initStomp();
      }
    });
  }

  initStomp() {
    let config = this.stompConfiguration;
    this.stompService.config = config;
    this.stompService.initAndConnect();
    this.stompSubscribe();
  }

  /**
  * get alerts list
  */
  getAlertList() {
    this.spinner.show();
    this.alertService.getAlerts(this.searchParameters).subscribe(response => {
      this.spinner.hide();
      if (response.isSuccess === true) {
        if (response.data.count && response.data.count > 0) {
          this.alerts = response.data.items;
          this.totalRecords = response.data.count;
        } else {
          this.alerts = [];
          this.totalRecords = 0;
        }
      }
      else {
        this.alerts = [];
        this._notificationService.add(new Notification('error', response.message));
        this.totalRecords = 0;
      }
    }, error => {
      this.spinner.hide();
      this.alerts = [];
      this.totalRecords = 0;
      this._notificationService.add(new Notification('error', error));
    });
  }

  /**
   * Stomp subscribe
   * */
  public stompSubscribe() {

    if (this.subscribed) {
      return;
    }

    this.messages = this.stompService.subscribe('/topic/' + this.cpId + '-' + this.uniqueId);
    this.subscription = this.messages.subscribe(this.on_next);
    this.subscribed = true;
  }

  public on_next = (message: Message) => {
    let uniqeId = (message.headers.destination).split("-");
    if (this.uniqueId == uniqeId[1]) {
      let obj: any = JSON.parse(message.body);
      let reporting_data = obj.data.data.reporting;
      if (reporting_data) {
        this.tripDetails.fuelLevel = reporting_data.can_fuel_level;
        this.tripDetails.currentSpeed = reporting_data.can_vehicle_speed;
        this.tripDetails.engineTemp = reporting_data.can_enginetemp;
        this.tripDetails.tyrePressure = reporting_data.can_tyrepressure;
        this.tripDetails.oil = reporting_data.can_fuel_level;
        if (reporting_data.can_engine_rpm <= 999 || reporting_data.can_engine_rpm == 0.00) {
          this.isStarted = false;
        }
        else {
          this.isStarted = true;
        }
      }
      this.isConnected = true;
      this.reportingData = reporting_data;
      if (this.reportingData) {
        this.lat = obj.data.data.reporting.gps_lat;
        this.lng = obj.data.data.reporting.gps_lng;

        this.origin = { lat: this.lat, lng: this.lng };
        this.truckDirection = this._service.vehicleBearing(this.origin, this.destination);
        this.can_odometer = this.reportingData.can_odometer;
        this.remailingMiles = this._service.calculateTotalMiles(this.lat, this.lng, this.tripDetails.destinationLatitude, this.tripDetails.destinationLongitude);
        this.progressMilesPerc = this.progressMilesPer(this.totalMiles, this.remailingMiles);
        let distanceInKm = this.remailingMiles / 0.62137;

        if (distanceInKm < 1 && this.flag != true) {
          let currentdatetime = moment().format('YYYY-MM-DD[T]HH:mm:ss');
          let timezone = moment().utcOffset();
          let data = {
            "tripGuid": this.tripGuid,
            "currentDate": currentdatetime,
            "timeZone": timezone, "coveredMiles": Math.floor(this.progressMilesPerc)
          }
          this._service.endTrip(data).subscribe(response => {
            if (response.isSuccess === true) {
              this.flag = true;
              this.tripDetails.tripStatus = '(Completed)';
              this.tripStatus = '(Completed)';
              this.lat = this.tripDetails.destinationLatitude;
              this.lng = this.tripDetails.destinationLongitude;
              this.progressMilesPerc = 100;
              this.coveredMiles = this.totalMiles;
              this._notificationService.add(new Notification('success', "Trip has been complated successfully."));
            } else {
              this.flag = true;
            }
          })
        }
        let arrivalResponse = this._service.calculateArrivalTime(this.reportingData.can_vehicle_speed, distanceInKm);
        if (arrivalResponse != '')
          this.arrivalTime = this._service.hoursToDhms(arrivalResponse);
        else if (parseFloat(arrivalResponse) == 0)
          this.arrivalTime = '0 second';
        else { }
        var start = new google.maps.LatLng(this.lat, this.lng);
        var end = new google.maps.LatLng(this.tripDetails.destinationLatitude, this.tripDetails.destinationLongitude);
        var objsign = this;
        var directionsService = new google.maps.DirectionsService();
        var request = {
          origin: start,
          destination: end,
          travelMode: google.maps.DirectionsTravelMode.DRIVING

        };

        directionsService.route(request, function (response, status) {
          if (status == google.maps.DirectionsStatus.OK) {
            let point = response.routes[0].legs[0];
            if (point.duration.value > 0) {
              var arrival_time = new Date();
              arrival_time.setSeconds(arrival_time.getSeconds() + point.duration.value);
              let datetime = objsign.getFormattedDateTime(arrival_time)
              if (objsign.tripflag != true) {
                let currentdatetime = moment().format('YYYY-MM-DD[T]HH:mm:ss');
                let timezone = moment().utcOffset();
                let data = {
                  "tripGuid": objsign.tripGuid,
                  "etaEndDateTime": datetime,
                  "odometer": objsign.can_odometer,
                  "currentDate": currentdatetime,
                  "timeZone": timezone
                }
                objsign._service.startTrip(data).subscribe(response => {
                  if (response.isSuccess === true) {
                    objsign.tripDetails.tripStatus = '(In Transit)';
                    objsign.tripStatus = '(In Transit)';
                    objsign.tripflag = true;
                  } else {
                    objsign.tripflag = true;
                  }
                })
              }
            }
          }
        })
      }

      let dates = obj.data.data.time;
      let now = moment();
      if (obj.data.data.status == undefined && obj.data.msgType == 'telemetry' && obj.data.msgType != 'device' && obj.data.msgType != 'simulator') {
        this.deviceIsConnected = true;

      } else if (obj.data.msgType === 'simulator' || obj.data.msgType === 'device') {
        if (obj.data.data.status === 'off') {

          this.deviceIsConnected = false;
        } else {
          this.deviceIsConnected = true;
        }
      }
      obj.data.data.time = now;
    }
  }

  /**
   * Get local date
   * @param lDate
   */
  getLocalDate(lDate) {
    var utcDate = moment.utc(lDate, 'YYYY-MM-DDTHH:mm:ss.SSS');
    // Get the local version of that date
    var localDate = moment(utcDate).local();
    let res = moment(localDate).format('MMM DD, YYYY hh:mm:ss A');
    return res;

  }

  /**
   * Total miles covered
   * @param total
   * @param remain
   */
  progressMilesPer(total, remain) {

    let r = (100 * remain) / total;
    this.coveredMiles = total - remain;
    return 100 - r;
  }

  /**
   * Convert kelvin to fahrenheit
   * @param temp
   */
  kelvinToFahrenheit(temp) {

    return (temp - 273.15) * 9 / 5 + 32;
  }

  checkZero(data) {
    if (data.length == 1) {
      data = "0" + data;
    }
    return data;
  }

  getFormattedDateTime(date) {
    var day = date.getDate() + "";
    var month = (date.getMonth() + 1) + "";
    var year = date.getFullYear() + "";
    var hour = date.getHours() + "";
    var minutes = date.getMinutes() + "";
    var seconds = date.getSeconds() + "";

    day = this.checkZero(day);
    month = this.checkZero(month);
    year = this.checkZero(year);
    hour = this.checkZero(hour);
    minutes = this.checkZero(minutes);
    seconds = this.checkZero(seconds);

    return (year + "-" + month + "-" + day + "T" + hour + ":" + minutes + ":" + seconds);
  }

  /**
   * End trip comfirmation popup
   * @param tripModel
   */
  endModel(tripModel: any) {
    this.deleteAlertDataModel = {
      title: "End Trip",
      message: "Are you sure you want to end this trip?",
      okButtonName: "Confirm",
      cancelButtonName: "Cancel",
    };
    const dialogRef = this.dialog.open(DeleteDialogComponent, {
      width: '400px',
      height: 'auto',
      data: this.deleteAlertDataModel,
      disableClose: false
    });
    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.endTrip(tripModel);
      }
    });
  }

  /**
   * End trip
   * @param tripModel
   */
  endTrip(tripModel) {
    this.progressMilesPerc = tripModel.totalMiles - this.remailingMiles;
    let currentdatetime = moment().format('YYYY-MM-DD[T]HH:mm:ss');
    let timezone = moment().utcOffset();
    let data = {
      "tripGuid": tripModel.guid,
      "currentDate": currentdatetime,
      "timeZone": timezone,
      "coveredMiles": Math.floor(this.progressMilesPerc)
    }
    this._service.endTrip(data).subscribe(response => {
      if (response.isSuccess === true) {
        this.getTripDetails(this.tripGuid);
      }
    })
  }

  truckIconAsPerDirection(direction, isStarted) {
    if (isStarted) {
      switch (direction) {

        case "N":
          this.icon.url = './assets/images/truck-location-green.png'
          break;

        case "NE":
          this.icon.url = './assets/images/truck-location-green.png'
          break;

        case "E":
          this.icon.url = './assets/images/truck-location-green.png'
          break;

        case "SE":
          this.icon.url = './assets/images/truck-location-green.png'
          break;


        case "S":
          this.icon.url = './assets/images/truck-location-green-i.png'
          break;


        case "SW":
          this.icon.url = './assets/images/truck-location-green-i.png'
          break;


        case "W":
          this.icon.url = './assets/images/truck-location-green-i.png'
          break;


        case "WN":
          this.icon.url = './assets/images/truck-location-green-i.png'
          break;

      }
    }
    else {
      switch (direction) {
        case "N":
          this.icon.url = './assets/images/truck-location-yellow.png'
          break;

        case "NE":
          this.icon.url = './assets/images/truck-location-yellow.png'
          break;

        case "E":
          this.icon.url = './assets/images/truck-location-yellow.png'
          break;

        case "SE":
          this.icon.url = './assets/images/truck-location-yellow.png'
          break;

        case "S":
          this.icon.url = './assets/images/truck-location-yellow-i.png'
          break;

        case "SW":
          this.icon.url = './assets/images/truck-location-yellow-i.png'
          break;

        case "W":
          this.icon.url = './assets/images/truck-location-yellow-i.png'
          break;

        case "WN":
          this.icon.url = './assets/images/truck-location-yellow-i.png'
          break;

      }

    }

  }

}
