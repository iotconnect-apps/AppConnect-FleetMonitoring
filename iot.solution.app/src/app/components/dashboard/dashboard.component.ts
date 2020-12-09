/// <reference types="@types/googlemaps" />
import * as moment from 'moment-timezone'
import { ChangeDetectorRef, Component, OnInit, ViewChild, ElementRef, NgZone, Output, EventEmitter } from '@angular/core'
import { NgxSpinnerService } from 'ngx-spinner'
import { Router } from '@angular/router'
import { AppConstant, DeleteAlertDataModel } from "../../app.constants";
import { MatDialog, MatPaginator, MatSort, MatTableDataSource } from '@angular/material'
import { DashboardService, Notification, NotificationService, DeviceService, AlertsService, TripService, DynamicDashboardService } from '../../services';
import { Observable } from 'rxjs/Observable';
import { StompRService } from '@stomp/ng2-stompjs'
import { Message } from '@stomp/stompjs'
import { Subscription } from 'rxjs'
import { MapsAPILoader, AgmMap, MouseEvent } from '@agm/core';
declare const google: any
import { DisplayGrid, CompactType, GridsterConfig, GridsterItem, GridsterItemComponent, GridsterPush, GridType, GridsterComponentInterface, GridsterItemComponentInterface } from 'angular-gridster2';
import { HttpClient } from '@angular/common/http';
@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.css'],
  providers: [StompRService]
})

export class DashboardComponent implements OnInit {
  @Output() changeSearchtext = new EventEmitter();
  infopopup = false;
  searchText = ''
  highenergy: any;
  graphdata: any = [];
  graphChartData = {
    chartType: 'LineChart',
    options: {
      legend: 'none',
      curveType: 'function',
      pointSize: 20,
      height: 400,
      interpolateNulls: true,
      hAxis: {
        title: '',
        gridlines: {
          count: 5
        },
      },
      vAxis: {
        title: 'Values',
        gridlines: {
          count: 5
        },
      }
    },
    dataTable: [],

  }
  FleetStatusChartData = {
    chartType: 'LineChart',
    options: {
      legend: 'none',
      curveType: 'function',
      pointSize: 10,
      height: 400,
      interpolateNulls: true,
      hAxis: {
        title: 'hiii',
        gridlines: {
          count: 5
        },
      },
      vAxis: {
        title: 'Values',
        gridlines: {
          count: 5
        },
      }
    },
    dataTable: [],

  }
  remailingMiles: number = 0;
  coveredMiles: number = 0;
  totalMiles: number = 0;
  progressMilesPerc: number = 0;
  public alerts: any = [];
  overviewstatics: any = { 'totalFleetCount': 0, 'inTransitFleetCount': 0, 'inGarageFleetCount': 0, 'driverUtilizationPer': 0, 'fleetUtilizationPer': 0, 'totalFuelConsumption': 0, 'totalAlerts': 0, 'totalUserCount': 0, 'activeUserCount': 0, 'inactiveUserCount': 0 }
  lat: number = 37.0902;
  lng: number = -95.7129;
  mediaUrl = "";
  locationList: any = [];
  isShowLeftMenu = true;
  isSearch = false;
  mapview = true;
  deviceConnected = false;
  flag = false;
  tripflag = false;
  deleteAlertDataModel: DeleteAlertDataModel;
  ChartHead = ['Date/Time'];
  chartData = [];
  maplist: any = [];
  topics: any = [];
  datadevice: any = [];
  columnArray: any = [];
  chartHeight = 320;
  chartWidth = '100%';
  currentUser = JSON.parse(localStorage.getItem('currentUser'));
  message: any;
  type: any;

  entitySelected: number;
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
  zoom = 12;
  private geoCoder;
  @ViewChild('search', { static: true }) public searchElementRef: ElementRef;
  can_odometer: any;
  /*Dynamic Dashboard Code*/
  @ViewChild('gridster', { static: false }) gridster;
  isDynamicDashboard: boolean = true;
  options: GridsterConfig;
  dashboardWidgets: Array<any> = [];
  dashboardList = [];
  dashboardData = {
    id: '',
    index: 0,
    dashboardName: '',
    isDefault: false,
    widgets: []
  };
  resizeEvent: EventEmitter<any> = new EventEmitter<any>();
  alertLimitchangeEvent: EventEmitter<any> = new EventEmitter<any>();
  chartTypeChangeEvent: EventEmitter<any> = new EventEmitter<any>();
  zoomChangeEvent: EventEmitter<any> = new EventEmitter<any>();
  telemetryDeviceChangeEvent: EventEmitter<any> = new EventEmitter<any>();
  telemetryAttributeChangeEvent: EventEmitter<any> = new EventEmitter<any>();
  sideBarSubscription: Subscription;
  deviceData: any = [];
  /*Dynamic Dashboard Code*/
  constructor(
    private router: Router,
    private spinner: NgxSpinnerService,
    private dashboardService: DashboardService,
    private _notificationService: NotificationService,
    public _appConstant: AppConstant,
    public dialog: MatDialog,
    public _service: AlertsService,
    private stompService: StompRService,
    private deviceService: DeviceService,
    private tripService: TripService,
    private mapsAPILoader: MapsAPILoader,
    private ngZone: NgZone,
    public dynamicDashboardService: DynamicDashboardService

  ) {
    this.getMapList(this.searchText);
    this.mediaUrl = this._notificationService.apiBaseUrl;
    /*Dynamic Dashboard Code*/
    this.sideBarSubscription = this.dynamicDashboardService.isToggleSidebarObs.subscribe((toggle) => {
      if (this.isDynamicDashboard && this.dashboardList.length > 0) {
        this.spinner.show();
        this.changedOptions();
        let cond = false;
        Observable.interval(700)
          .takeWhile(() => !cond)
          .subscribe(i => {
            cond = true;
            this.checkResponsiveness();
            this.spinner.hide();
          });
      }
    })
		/*Dynamic Dashboard Code*/
  }

  ngOnInit() {
    this.getDashbourdCount();
    this.getfleettypeusage();
    this.getenergyusageGraph()
    this.getFleetStatusGraph();
    this.getAlertList();
    this.getStompConfig();
    this.getDashboards();

    /*Dynamic Dashboard Code*/
    this.options = {
      gridType: GridType.Fixed,
      displayGrid: DisplayGrid.Always,
      initCallback: this.gridInit.bind(this),
      itemResizeCallback: this.itemResize.bind(this),
      fixedColWidth: 20,
      fixedRowHeight: 20,
      keepFixedHeightInMobile: false,
      keepFixedWidthInMobile: false,
      mobileBreakpoint: 640,
      pushItems: false,
      draggable: {
        enabled: false
      },
      resizable: {
        enabled: false
      },
      enableEmptyCellClick: false,
      enableEmptyCellContextMenu: false,
      enableEmptyCellDrop: false,
      enableEmptyCellDrag: false,
      enableOccupiedCellDrop: false,
      emptyCellDragMaxCols: 50,
      emptyCellDragMaxRows: 50,

      minCols: 60,
      maxCols: 192,
      minRows: 62,
      maxRows: 375,
      setGridSize: true,
      swap: true,
      swapWhileDragging: false,
      compactType: CompactType.None,
      margin: 0,
      outerMargin: true,
      outerMarginTop: null,
      outerMarginRight: null,
      outerMarginBottom: null,
      outerMarginLeft: null,
    };
    /*Dynamic Dashboard Code*/

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
        if (reporting_data.can_engine_rpm <= 999 || reporting_data.can_engine_rpm == 0.00) {
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
  /**
	 * Get getenergyusageGraph
	 * */
  getenergyusageGraph() {
    this.spinner.show();
    var data = {
      "companyguid": this.currentUser.userDetail.companyId
    }
    this.dashboardService.getenergyusageGraph(data).subscribe(response => {
      this.spinner.hide();
      if (response.isSuccess === true) {
        let data = [];
        data.push(["", ""])
        response.data.forEach(element => {
          data.push([element.name, parseFloat(element.energyConsumption)])
        });
        this.graphChartData = {
          chartType: 'LineChart',
          options: {
            legend: 'none',
            curveType: 'function',
            pointSize: 10,
            height: 400,
            interpolateNulls: true,
            hAxis: {
              title: '',
              gridlines: {
                count: 5
              },
            },
            vAxis: {
              title: 'Fuel (Gallons)',
              gridlines: {
                count: 5
              },
            }
          },
          dataTable: data,
        };
      }
    }, error => {
      this.spinner.hide();
      this._notificationService.add(new Notification('error', error));
    });

  }
  /**
	 * Get getFleetStatusGraph
	 * */
  getFleetStatusGraph() {
    this.spinner.show();
    var data = {
      "companyguid": this.currentUser.userDetail.companyId
    }
    this.dashboardService.getFleetStatusGraph(data).subscribe(response => {
      this.spinner.hide();
      if (response.isSuccess === true) {
        let data = [];
        data.push(["", "Active", "Halt", "Break"])
        response.data.forEach(element => {
          data.push([element.fleetId, parseFloat(element.activeCount), parseFloat(element.haltCount), parseFloat(element.idleCount)])
        });
        this.FleetStatusChartData = {
          chartType: 'LineChart',
          options: {
            legend: 'bottom',
            curveType: 'function',
            pointSize: 10,
            height: 400,
            interpolateNulls: true,
            hAxis: {
              title: '',
              gridlines: {
                count: 5
              },
            },
            vAxis: {
              title: 'Fleet Status (no. of hours)',
              gridlines: {
                count: 5
              },
            }
          },
          dataTable: data,
        };
      }
    }, error => {
      this.spinner.hide();
      this._notificationService.add(new Notification('error', error));
    });

  }
  /**
  * Get Alert List
  * */
  getAlertList() {
    let parameters = {
      pageNo: 0,
      pageSize: 10,
      searchText: '',
      orderBy: 'eventDate desc',
      deviceGuid: '',
      entityGuid: '',
      parentEntityGuid: ""
    };
    this.spinner.show();
    this._service.getAlerts(parameters).subscribe(response => {
      this.spinner.hide();
      if (response.isSuccess === true) {
        if (response.data.count) {
          this.alerts = response.data.items;
        }

      }
      else {
        this.alerts = [];
        this._notificationService.add(new Notification('error', response.message));

      }
    }, error => {
      this.alerts = [];

      this._notificationService.add(new Notification('error', error));
    });
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
          if (this.maplist) {
            this.lat = +response.data.items[0].sourceLatitude ? +response.data.items[0].sourceLatitude : +response.data.items[0].latitude;
            this.lng = +response.data.items[0].sourceLongitude ? +response.data.items[0].sourceLongitude : +response.data.items[0].longitude;
          }
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

  /**
  * Convert To float
  * @param value
  * */
  convertToFloat(value) {
    return parseFloat(value)
  }
  /**
	 * Get Timezone
	 * */
  getTimeZone() {
    return /\((.*)\)/.exec(new Date().toString())[1];
  }
  /**
	 * Get color
	 * */
  getcolor(colorname) {
    if (colorname == 'red') {
      return 'warn';
    } else {
      return 'primary';
    }
  }
  /**
    * Get count of variables for Dashboard
    * */
  getDashbourdCount() {
    this.spinner.show();
    this.dashboardService.getDashboardoverview().subscribe(response => {
      this.spinner.hide();
      if (response.isSuccess === true) {
        if (response.data) {
          this.overviewstatics = response.data;
          if (this.overviewstatics) {
            this.overviewstatics.totalFuelConsumption = this.overviewstatics.totalFuelConsumption ? this.dashboardService.transform(this.overviewstatics.totalFuelConsumption) : 0;
          }
        }
      }
      else {
        this._notificationService.add(new Notification('error', response.message));
      }
    }, error => {
      this.spinner.hide();
      this._notificationService.add(new Notification('error', error));
    });
  }
  /**
	 * Get fleet type Graph Data
   * @param companyguid
	 * */
  getfleettypeusage() {
    this.spinner.show();
    var data = {
      "companyguid": this.currentUser.userDetail.companyId
    }
    this.dashboardService.getfleettypeusage(data).subscribe(response => {
      this.spinner.hide();
      if (response.isSuccess === true && response.data != '') {
        this.graphdata = response.data;
      } else {
        this.graphdata = []
      }
    })
  }

  /**
    * Get LocalDate by lDate
    * @param lDate
    */
  getLocalDate(lDate) {
    var utcDate = moment.utc(lDate, 'YYYY-MM-DDTHH:mm:ss.SSS');
    var localDate = moment(utcDate).local();
    let res = moment(localDate).format('MMM DD, YYYY hh:mm:ss A');
    return res;
  }

  /**
   * Change search
   * @param searchText
   */
  changeSearch(searchText) {
    this.changeSearchtext.emit(searchText);
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
  /*Dynamic Dashboard Code*/
  getDashboards() {
    this.spinner.show();
    this.dashboardList = [];
    let isAnyDefault = false;
    let systemDefaultIndex = 0;
    this.dynamicDashboardService.getUserWidget().subscribe(response => {
      this.isDynamicDashboard = false;
      for (var i = 0; i <= (response.data.length - 1); i++) {
        response.data[i].id = response.data[i].guid;
        response.data[i].widgets = JSON.parse(response.data[i].widgets);
        this.dashboardList.push(response.data[i]);
        if (response.data[i].isDefault === true) {
          isAnyDefault = true;
          this.dashboardData.index = i;
          this.isDynamicDashboard = true;
        }
        if (response.data[i].isSystemDefault === true) {
          systemDefaultIndex = i;
        }
      }
      /*Display Default Dashboard if no data*/
      if (!isAnyDefault && response.data.length > 0) {
        this.dashboardData.index = systemDefaultIndex;
        this.isDynamicDashboard = true;
        this.dashboardList[systemDefaultIndex].isDefault = true;
      }
      /*Display Default Dashboard if no data*/
      this.spinner.hide();
      if (this.isDynamicDashboard) {
        this.editDashboard('view', 'n');
      }
      else {

        this.getAlertList();
      }
    }, error => {
      this.spinner.hide();
      /*Load Old Dashboard*/
      this.isDynamicDashboard = false;
      this.getAlertList();
      /*Load Old Dashboard*/
      this._notificationService.add(new Notification('error', error));
    });
  }

  editDashboard(type: string = 'view', is_cancel_btn: string = 'n') {
    this.spinner.show();
    this.dashboardWidgets = [];

    this.dashboardData.id = '';
    this.dashboardData.dashboardName = '';
    this.dashboardData.isDefault = false;
    for (var i = 0; i <= (this.dashboardList[this.dashboardData.index].widgets.length - 1); i++) {
      this.dashboardWidgets.push(this.dashboardList[this.dashboardData.index].widgets[i]);
    }

    if (this.options.api && this.options.api.optionsChanged) {
      this.options.api.optionsChanged();
    }
    this.spinner.hide();
  }

  gridInit(grid: GridsterComponentInterface) {
    if (this.options.api && this.options.api.optionsChanged) {
      this.options.api.optionsChanged();
    }
    let cond = false;
    Observable.interval(500)
      .takeWhile(() => !cond)
      .subscribe(i => {
        cond = true;
        this.checkResponsiveness();
      });
  }

  checkResponsiveness() {
    if (this.gridster) {
      let fixedColWidth = 20;
      let tempWidth = parseFloat((((this.gridster.curWidth * fixedColWidth) / (fixedColWidth * this.gridster.columns)).toFixed(2)).toString());
      tempWidth = (tempWidth - 0.01);
      if (this.gridster.curWidth >= 640) {
        //tempWidth = Math.floor((this.gridster.curWidth / 60));
        this.options.fixedColWidth = tempWidth;
      }
      else {
        this.options.fixedColWidth = fixedColWidth;
      }
      for (var i = 0; i <= (this.dashboardWidgets.length - 1); i++) {
        if (this.gridster.curWidth < 640) {
          for (var g = 0; g <= (this.gridster.grid.length - 1); g++) {
            if (this.gridster.grid[g].item.id == this.dashboardWidgets[i].id) {
              this.dashboardWidgets[i].properties.w = this.gridster.grid[g].el.clientWidth;
            }
          }
        }
        else {
          this.dashboardWidgets[i].properties.w = (tempWidth * this.dashboardWidgets[i].cols);
        }
        this.resizeEvent.emit(this.dashboardWidgets[i]);
      }
      this.changedOptions();
    }
  }

  changedOptions() {
    if (this.options.api && this.options.api.optionsChanged) {
      this.options.api.optionsChanged();
    }
  }

  itemResize(item: any, itemComponent: GridsterItemComponentInterface) {
    this.resizeEvent.emit(item);
  }

  deviceSizeChange(size) {
    this.checkResponsiveness();
  }


  /*Dynamic Dashboard Code*/
}
