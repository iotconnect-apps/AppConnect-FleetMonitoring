import { Component, OnInit } from '@angular/core';
import { DriverService } from '../../../services/driver/driver.service';
import { ActivatedRoute } from '@angular/router';
import { NgxSpinnerService } from 'ngx-spinner';
import { Notification, NotificationService, AlertsService, FleetService, DeviceService, TripService } from '../../../services';
import * as moment from 'moment';
import { Subscription, Observable } from 'rxjs';
import { Message } from '@stomp/stompjs';
import { StompRService } from '@stomp/ng2-stompjs';
declare const google: any

export interface PeriodicElement {
  name: string;
  position: number;
  weight: string;
  symbol: string;
  material: string;
}

const ELEMENT_DATA: PeriodicElement[] = [
  { position: 1, name: 'AV124', weight: 'XY145', symbol: '425Miles', material: 'Solid' },
  { position: 2, name: 'AV125', weight: 'XY155', symbol: '405Miles', material: 'Liquid' },
  { position: 3, name: 'AV126', weight: 'XY165', symbol: '402Miles', material: 'Solid' },
];

@Component({
  selector: 'app-driver-dashboard',
  templateUrl: './driver-dashboard.component.html',
  styleUrls: ['./driver-dashboard.component.css'],
  providers: [StompRService]
})
export class DriverDashboardComponent implements OnInit {

  lat: number;
  lng: number;
  totalMiles: number = 0;
  remailingMiles: number = 0;
  coveredMiles: number = 0;
  progressMilesPerc: number = 0;
  displayedColumns: string[] = ['tripId', 'fleetName', 'weight', 'totalMiles', 'materialType'];
  dataSource = ELEMENT_DATA;
  pageSizeOptions: number[] = [5, 10, 25, 100];
  ongoingTripDetails: any = {};
  ongoingTripDetailsCount = 0;
  driverGuid: any;
  driverDashboardDetails: any = {};
  alerts: any = [];
  fleetGuid: any;
  datatrip: any = [{ text: 'on going' }, { text: 'upcoming' }, { text: 'completed' }]
  labelname: any;
  tripList: any = [];
  totalRecords = 0;
  columnChart = {
    chartType: "ColumnChart",
    dataTable: [],
    options: {
      title: "",
      vAxis: {
        title: "No of Trips",
        titleTextStyle: {
          bold: true
        },
        viewWindow: {
          min: 0
        }
      },
      hAxis: {
        titleTextStyle: {
          bold: true
        },
      },
      legend: 'none',
      height: "350",
      chartArea: { height: '75%', width: '85%' },
      seriesType: 'bars',
      bar: { groupWidth: "40%" },
      colors: ['#ed734c'],
    }
  };
  type: any;
  subscription: Subscription;
  messages: Observable<Message>;
  uniqueId: any;
  reportingData: any = {};
  deviceIsConnected = false;
  isConnected = false;
  flag = false;
  tripflag = false;
  cpId = '';
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
  searchParameters = {
    parentEntityGuid: '',
    pageNumber: 0,
    pageSize: 5,
    searchText: '',
    sortBy: 'tripId asc'
  };
  arrivalTime: any = '-';
  can_odometer: any;

  constructor(
    public _service: DriverService,
    public fleetService: FleetService,
    private activatedRoute: ActivatedRoute,
    private spinner: NgxSpinnerService,
    private _notificationService: NotificationService,
    private deviceService: DeviceService,
    public alertsService: AlertsService,
    private stompService: StompRService,
    private tripService: TripService) {
    this.activatedRoute.params.subscribe(params => {
      if (params.driverGuid != null) {
        this.driverGuid = params.driverGuid;
        this.getDriverDetails(params.driverGuid);
      }
    });
  }

  ngOnInit() {
    this.labelname = 'on going';
    this.getTripData(this.labelname);
    let type = 'd';
    this.type = type
    this.getTripsByDriver(this.driverGuid, type);
  }

  /**
   * 
   * @param event
   */
  changeGraphFilter(event) {
    let type = 'd';
    if (event.value === 'Week') {
      type = 'w';
    } else if (event.value === 'Month') {
      type = 'm';
    }
    else if (event.value === 'Day') {
      type = 'd';
    }
    this.type = type
    this.getTripsByDriver(this.driverGuid, type);

  }

  /**
   * Get driver details by driverGuid
   * @param driverGuid
   */
  getDriverDetails(driverGuid) {
    this.spinner.show();
    this._service.getDriverDashboardDetail(driverGuid).subscribe(response => {
      if (response.isSuccess === true) {
        this.driverDashboardDetails = response.data;
        this.driverDashboardDetails.driverId = '(' + this.driverDashboardDetails.driverId + ')';
        if (this.driverDashboardDetails) {
          this.getAlertList(this.driverDashboardDetails.fleetGuid);
        }
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
   * Get list of alerts by fleetGuid
   * @param fleetGuid
   */
  getAlertList(fleetGuid) {
    this.alerts = [];
    let parameters = {
      "fleetGuid": fleetGuid,
      "searchText": "",
      "pageNo": 0,
      "pageSize": 10,
      "orderBy": 'eventDate desc'
    }
    this.spinner.show();
    this.alertsService.getAlerts(parameters).subscribe(response => {
      this.spinner.hide();
      if (response.isSuccess === true) {
        if (response.data.count > 0) {
          this.alerts = response.data.items;
        }

      }
      else {
        this.alerts = [];
        this._notificationService.handleResponse(response, "error");

      }
    }, error => {
      this.alerts = [];
      this._notificationService.handleResponse(error, "error");
    });
  }

  /**
   * Get trip data by status
   * @param status
   */
  getTripData(status) {
    this.spinner.show();
    let currentdatetime = moment().format('YYYY-MM-DD[T]HH:mm:ss');
    let timezone = moment().utcOffset();

    const parameter = {
      'pageNo': this.searchParameters.pageNumber + 1,
      'pageSize': this.searchParameters.pageSize,
      'searchText': this.searchParameters.searchText,
      'orderBy': this.searchParameters.sortBy,
      'currentDate': currentdatetime.toString(),
      'timeZone': timezone.toString(),
      "driverGuid": this.driverGuid,
      "status": status

    };
    this.fleetService.getFleetBytrip(parameter).subscribe(response => {
      this.spinner.hide();

      if (response.isSuccess === true) {
        if (response.data.count > 0) {
          if (status == "on going") {
            this.ongoingTripDetails = response.data.items[0];
            this.ongoingTripDetailsCount = response.data.count;
            this.lat = this.ongoingTripDetails.sourceLatitide;
            this.lng = this.ongoingTripDetails.sourceLongitude;
            this.uniqueId = this.ongoingTripDetails.uniqueId;
            this.totalMiles = this.calculateTotalMiles(this.ongoingTripDetails.sourceLatitude, this.ongoingTripDetails.sourceLongitude, this.ongoingTripDetails.destinationLatitude, this.ongoingTripDetails.destinationLongitude);
            this.getStompConfig();
          }
          else {
            this.totalRecords = response.data.count;
            this.tripList = response.data.items;
          }
        }else{
          this.tripList = [];
        }
      }
      else {
        this._notificationService.add(new Notification('error', response.message));
        this.tripList = [];
      }
    }, error => {
      this.spinner.hide();
      this._notificationService.add(new Notification('error', error));
    });

  }

  /**
   * Change page event
   * @param pagechangeresponse
   */
  ChangePaginationAsPageChange(pagechangeresponse) {
    this.searchParameters.pageSize = pagechangeresponse.pageSize;
    this.searchParameters.pageNumber = pagechangeresponse.pageIndex;
    this.getTripData(this.labelname);
  }

  /**
  * Set order 
  * @param sort
  */
  setOrder(sort: any) {
    if (!sort.active || sort.direction === '') {
      return;
    }
    this.searchParameters.sortBy = sort.active + ' ' + sort.direction;
    this.getTripData(this.labelname)
  }

  /**
   * On tab change 
   * @param tab
   */
  onTabChange(tab) {
    if (tab != undefined && tab != '') {
      this.labelname = tab.tab.textLabel;
      this.getTripData(this.labelname)
    }
  }

  /**
   * Get trips by driver by driverGuid and type
   * @param driverGuid
   * @param type
   */
  getTripsByDriver(driverGuid, type) {
    this.spinner.show();
    var data = { driverGuid: driverGuid, frequency: type }
    this._service.getTripsByDriver(data).subscribe(response => {
      this.spinner.hide();
      if (response.isSuccess === true) {
        let data = [];
        if (response.data.length) {
          data.push(['name', 'Total Trips'])

          response.data.forEach(element => {
            data.push([element.name, element.totalTrips])
          });
        }
        this.columnChart = {
          chartType: "ColumnChart",
          dataTable: data,
          options: {
            bar: { groupWidth: "25%" },
            colors: ['#5496d0'],
            legend: 'none',
            height: "350",
            chartArea: { height: '75%', width: '85%' },
            seriesType: 'bars',
            title: "",
            vAxis: {
              title: "No of Trips",
              titleTextStyle: {
                bold: true
              },
              viewWindow: {
                min: 0
              }
            },
            hAxis: {
              titleTextStyle: {
                bold: true
              },
            },
          }
        };
      }
      else {
        this.columnChart.dataTable = [];
        this._notificationService.add(new Notification('error', response.message));

      }
    }, error => {
      this.columnChart.dataTable = [];
      this.spinner.hide();
      this._notificationService.add(new Notification('error', error));
    });
  }

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

  public stompSubscribe() {
    if (this.subscribed) {
      return;
    }

    this.messages = this.stompService.subscribe('/topic/' + this.cpId + '-' + this.uniqueId);
    this.subscription = this.messages.subscribe(this.on_next);
    this.subscribed = true;
  }

  public on_next = (message: Message) => {
    let obj: any = JSON.parse(message.body);
    let reporting_data = obj.data.data.reporting;
    this.isConnected = true;
    this.reportingData = reporting_data
    this.can_odometer =this.reportingData.can_odometer
    if (this.reportingData) {
      this.lat = this.reportingData.gps_lat;
      this.lng = this.reportingData.gps_lng;
      this.remailingMiles = this.calculateTotalMiles(this.lat, this.lng, this.ongoingTripDetails.destinationLatitude, this.ongoingTripDetails.destinationLongitude);
      this.progressMilesPerc = this.progressMilesPer(this.totalMiles, this.remailingMiles);
      let distanceInKm = this.remailingMiles / 0.62137;
      if (distanceInKm < 1 && this.flag != true) {
        let currentdatetime = moment().format('YYYY-MM-DD[T]HH:mm:ss');
        let timezone = moment().utcOffset();
        let data = {
          "tripGuid": this.ongoingTripDetails.tripGuid,
          "currentDate": currentdatetime,
          "timeZone": timezone, "coveredMiles": Math.floor(this.progressMilesPerc)
        }
        this.tripService.endTrip(data).subscribe(response => {
          if (response.isSuccess === true) {
            this.flag = true;
            this._notificationService.add(new Notification('success', "Trip has been complated successfully."));
          } else {
            this.flag = true;
          }
        })
      }
      let arrivalResponse = this.tripService.calculateArrivalTime(this.reportingData.can_vehicle_speed, distanceInKm);
      if (arrivalResponse != '')
        this.arrivalTime = this.tripService.hoursToDhms(arrivalResponse);
      else if (parseFloat(arrivalResponse) == 0)
        this.arrivalTime = '0 second';
      else { }
      var start = new google.maps.LatLng(this.lat, this.lng);
        var end = new google.maps.LatLng(this.ongoingTripDetails.destinationLatitude, this.ongoingTripDetails.destinationLongitude);
        var objsign = this;
        var directionsService = new google.maps.DirectionsService();
          var request = {    
            origin: start,
            destination: end,
            travelMode: google.maps.DirectionsTravelMode.DRIVING
          
          };

          directionsService.route(request, function (response, status) {
            if (status == google.maps.DirectionsStatus.OK) {
              let point = response.routes[ 0 ].legs[ 0 ];
              if(point.duration.value > 0){
                var arrival_time = new Date();
                arrival_time.setSeconds(arrival_time.getSeconds() + point.duration.value);
                let datetime = objsign.getFormattedDateTime(arrival_time)
                if(objsign.tripflag != true){
                  let currentdatetime = moment().format('YYYY-MM-DD[T]HH:mm:ss');
                  let timezone = moment().utcOffset();
                  let data = {"tripGuid": objsign.ongoingTripDetails.tripGuid,
                  "etaEndDateTime": datetime,
                  "odometer":objsign.can_odometer,
                  "currentDate": currentdatetime,
                  "timeZone": timezone}
                  objsign.tripService.startTrip(data).subscribe(response => {
                   if (response.isSuccess === true) {
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

  rad(x) {
    return x * Math.PI / 180;
  }

  /**
   * Calculate total miles
   * @param slat
   * @param slng
   * @param dlat
   * @param dlng
   */
  calculateTotalMiles(slat, slng, dlat, dlng) {

    var R = 6378137; // Earth’s mean radius in meter
    var dLat = this.rad(slat - dlat);
    var dLong = this.rad(slng - dlng);
    var a = Math.sin(dLat / 2) * Math.sin(dLat / 2) +
      Math.cos(this.rad(slat)) * Math.cos(this.rad(dlat)) *
      Math.sin(dLong / 2) * Math.sin(dLong / 2);
    var c = 2 * Math.atan2(Math.sqrt(a), Math.sqrt(1 - a));
    var d = R * c; // returns the distance in meter
    return d * 0.00062137;
  }

  /**
   * Progress miles
   * @param total
   * @param remain
   */
  progressMilesPer(total, remain) {

    let r = (100 * remain) / total;
    this.coveredMiles = total - remain;
    return 100 - r;
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

}
