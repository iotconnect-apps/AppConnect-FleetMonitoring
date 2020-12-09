/// <reference types="@types/googlemaps" />
import * as moment from 'moment-timezone'
import { ChangeDetectorRef, ViewRef , OnInit, Component, Input, EventEmitter, OnDestroy, ViewChild } from '@angular/core';
import { NgxSpinnerService } from 'ngx-spinner'
import { DashboardService } from 'app/services/dashboard/dashboard.service';
import { Notification, NotificationService, AlertsService, DeviceService, TripService } from 'app/services';
import { AgmMap} from '@agm/core';
import { DeleteDialogComponent } from '../../../../components/common/delete-dialog/delete-dialog.component';
import { AppConstant, DeleteAlertDataModel } from "../../../../app.constants";
import { MatDialog, MatPaginator, MatSort, MatTableDataSource } from '@angular/material'
import { Observable } from 'rxjs/Observable';
import { StompRService } from '@stomp/ng2-stompjs'
import { Message } from '@stomp/stompjs'
import { Subscription } from 'rxjs'
declare const google: any
@Component({
	selector: 'app-widget-map-a',
	templateUrl: './widget-map-a.component.html',
	styleUrls: ['./widget-map-a.component.css'],
	providers: [StompRService]
})
export class WidgetMapAComponent implements OnInit, OnDestroy {
	flag = false;
	tripflag = false;
	can_odometer: any;
	deviceConnected = false;
	topics: any = [];
	maplist: any = [];
	zoom = 12;
	remailingMiles: number = 0;
  	coveredMiles: number = 0;
  	totalMiles: number = 0;
  	progressMilesPerc: number = 0;
	infopopup = false;
	deleteAlertDataModel: DeleteAlertDataModel;
	locationList: any = [];
	searchParameters = {
		pageNumber: 0,
		pageNo: 0,
		pageSize: 10,
		searchText: '',
		sortBy: 'uniqueId asc'
	  };
	  mapview = true;
	  lat: number;
	  lng: number;
	@Input() widget;
	@Input() count;
	@Input() resizeEvent: EventEmitter<any>;
	@Input() zoomChangeEvent: EventEmitter<any>;
	resizeSub: Subscription;
	zoomSub: Subscription;
	cpId = '';
	subscription: Subscription;
	messages: Observable<Message>;
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
	arrivalTime: any = '-';
	@ViewChild(AgmMap,{static:false}) myMap : any;
	mapHeight = '2000px';
	greenhouse = [];
	mediaUrl: string;
	searchText = '';
	constructor(
		public _appConstant: AppConstant,
		public dialog: MatDialog,
		public dashboardService: DashboardService,
		private spinner: NgxSpinnerService,
		private _notificationService: NotificationService,
		private changeDetector: ChangeDetectorRef,
		public _service: AlertsService,
		private deviceService: DeviceService,
		private stompService: StompRService,
		private tripService: TripService
		) {
		this.mediaUrl = this._notificationService.apiBaseUrl;
		this.getMapList(this.searchText);
	}

	ngOnInit() {
		this.mapHeight = (this.widget.properties.h > 0 ? parseInt((this.widget.properties.h - 125).toString())+'px' : this.mapHeight);
		this.widget.widgetProperty.zoom = (this.widget.widgetProperty.zoom && this.widget.widgetProperty.zoom > 0 ? parseInt(this.widget.widgetProperty.zoom) : 10);
		this.resizeSub = this.resizeEvent.subscribe((widget) => {
			if(widget.id == this.widget.id){
				this.widget = widget;
				this.resizeMap();
			}
		});
		this.zoomSub = this.zoomChangeEvent.subscribe((widget) => {
			if(widget && widget.id == this.widget.id){
				this.widget = widget; 
				this.resizeMap();
			}
		});
		this.resizeMap();
		this.getStompConfig();
	}
	search(searchText) {
		if (searchText) {
		  this.searchText = searchText;
		  this.getMapList(this.searchText);
		} else {
		  this.getMapList(this.searchText);
		}
	  }
	

/**
 * Get Map List
 * */
getMapList(search) {
    let currentdatetime = moment().format('YYYY-MM-DD[T]HH:mm:ss');
    let timezone = moment().utcOffset();
    let parameters = {
      "pageNo": -1,
      "pageSize": -1,
      "orderBy": "fleetId asc",
      "currentDate": currentdatetime,
      "timeZone": timezone,
      "searchText": search
    };
    this.spinner.show();
    this._service.getMaplist(parameters).subscribe(response => {
      this.spinner.hide();
      if (response.isSuccess === true) {
        if (response.data.count) {
          this.infopopup = false;
          this.maplist = response.data.items;
          this.lat = +response.data.items[0].latitude
          this.lng = +response.data.items[0].longitude
          if (search) {
            this.zoom = 12;
            this.infopopup = true;
          }
        } else {
          this.infopopup = false;
          this.maplist = [];
        }

      }
      else {
        this.maplist = [];
        this._notificationService.add(new Notification('error', response.message));

      }
    }, error => {
      this.maplist = [];

      this._notificationService.add(new Notification('error', error));
    });
  }

	resizeMap(){
		this.mapHeight = (this.widget.properties.h > 0 ? parseInt((this.widget.properties.h - 125).toString())+'px' : this.mapHeight);
		if(this.myMap){
			this.myMap.triggerResize();
		}
	}

	/**
  * Get stomp config data
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
/**
  * Init stomp
  * */
 initStomp() {
    let config = this.stompConfiguration;
    this.stompService.config = config;
    this.stompService.initAndConnect();
    this.stompSubscribe();
  }

  /**
  * Stomp subscribe
  * */
 public stompSubscribe() {
    if (this.subscribed) {
      return;
    }
    this.maplist.forEach(element => {
      if (!this.topics.includes(element.uniqueId)) {
        this.messages = this.stompService.subscribe('/topic/' + this.cpId + '-' + element.uniqueId);
        this.subscription = this.messages.subscribe(this.on_next);
        this.topics.push(element.uniqueId);
      }
    });
  }
  public on_next = (message: Message) => {
    let uniqeId = (message.headers.destination).split("-");
    let obj: any = JSON.parse(message.body);
    if (obj.data.msgType === 'telemetry') {
      let reporting_data = obj.data.data.reporting
      let dates = obj.data.data.time;
      let now = moment();
      if (obj.data.data.status != 'off' && reporting_data != '') {
        this.deviceConnected = true;
        let pos = this.maplist.map(function (x) { return x.uniqueId; }).indexOf(uniqeId[1]);
        this.maplist[pos].sourceLatitude = obj.data.data.reporting.gps_lat;
        this.maplist[pos].sourceLongitude = obj.data.data.reporting.gps_lng;
        this.can_odometer = reporting_data.can_odometer;
        this.maplist[pos].isStarted = true;
        if (reporting_data.can_engine_rpm < 0 || reporting_data.can_engine_rpm == 0.00 ) {
          this.maplist[pos].isStarted = false;
        }
        this.remailingMiles = this.calculateTotalMiles(this.maplist[pos].sourceLatitude, this.maplist[pos].sourceLongitude, this.maplist[pos].destinationLatitude, this.maplist[pos].destinationLongitude);
        this.progressMilesPerc = this.progressMilesPer(this.totalMiles, this.remailingMiles);
        let distanceInKm = this.remailingMiles / 0.62137;

        if (distanceInKm < 1 && this.flag != true) {
          let currentdatetime = moment().format('YYYY-MM-DD[T]HH:mm:ss');
          let timezone = moment().utcOffset();
          let data = {
            "tripGuid": this.maplist[pos].tripGuid,
            "currentDate": currentdatetime,
            "timeZone": timezone, "coveredMiles": this.remailingMiles
          }
          this.tripService.endTrip(data).subscribe(response => {
            if (response.isSuccess === true) {
              this.flag = true;
              this.maplist[pos].isStarted = false;
            } else {
              this.flag = true;
            }
          })
        }

        let arrivalResponse = this.calculateArrivalTime(reporting_data.can_vehicle_speed, distanceInKm);
        if (arrivalResponse != '') {
          this.arrivalTime = this.HoursToDhms(arrivalResponse);
        }
        else if (parseFloat(arrivalResponse) == 0) {
          this.arrivalTime = '0 second';
        }

        this.maplist.forEach(element => {
          if (element.uniqueId == uniqeId[1] && element.status == 'Trip Running') {
            element['arrivalTime'] = this.arrivalTime;
          }
        });

        var start = new google.maps.LatLng(this.maplist[pos].sourceLatitude, this.maplist[pos].sourceLongitude);
        var end = new google.maps.LatLng(this.maplist[pos].destinationLatitude, this.maplist[pos].destinationLongitude);
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
                  "tripGuid": objsign.maplist[pos].tripGuid,
                  "etaEndDateTime": datetime,
                  "currentDate": currentdatetime,
                  "odometer": Math.floor(objsign.can_odometer),
                  "timeZone": timezone
                }
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
      obj.data.data.time = now;
    } else if (obj.data.msgType === 'simulator') {
      if (obj.data.data.status === 'off') {


      } else {

      }
    }
  }
  rad(x) {
    return x * Math.PI / 180;
  }
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
  progressMilesPer(total, remain) {

    let r = (100 * remain) / total;
    this.coveredMiles = total - remain;
    return 100 - r;
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
  HoursToDhms(hours) {
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
	ngOnDestroy() {
		this.resizeSub.unsubscribe();
		this.zoomSub.unsubscribe();
	}
}
