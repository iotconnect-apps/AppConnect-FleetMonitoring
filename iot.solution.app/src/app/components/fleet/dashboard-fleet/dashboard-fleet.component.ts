import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { NotificationService, Notification, FleetService, AlertsService, DeviceService, TripService, DashboardService } from '../../../services';
import { NgxSpinnerService } from 'ngx-spinner';
import * as moment from 'moment'
import { Observable } from 'rxjs/Observable';
import { StompRService } from '@stomp/ng2-stompjs'
import { Message } from '@stomp/stompjs'
import { Subscription } from 'rxjs'
declare const google: any

@Component({
  selector: 'app-dashboard-fleet',
  templateUrl: './dashboard-fleet.component.html',
  styleUrls: ['./dashboard-fleet.component.css'],
  providers: [StompRService]
})
export class DashboardFleetComponent implements OnInit {

  rightDirections: any = ['N', 'NE', 'E', 'SE'];
  leftDirections: any = ['S', 'SW', 'W', 'NW'];
  truckDirection: any;
  isStarted: any = false;
  attributes: any = {};
  deviceAttribute: any = [];
  altitude: any = 0;
  deviceGuid: any;
  lastGPSTime: any;
  gateway_uptime: any;
  vehicle_ign_sense: any = 0;
  gps_num_sats: any = 0;
  can_vehicle_speed: any = 0;
  tripflag = false;
  arrivalTime: any = '-';
  totalMiles: number = 0;
  remailingMiles: number = 0;
  coveredMiles: number = 0;
  progressMilesPerc: number = 0;
  isRight: any = true;
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
  start_end_mark = [];
  tmpPoints = [

    {
      lat: 22.6916,
      lng: 72.8634
    },
    {
      lat: 22.5645,
      lng: 72.9289
    },
    {
      lat: 22.3072,
      lng: 73.1812
    },
    {
      lat: 21.6264,
      lng: 73.1812
    }

  ]
  public origin: any;
  public destination: any;
  lineChartData = {
    chartType: 'LineChart',
    options: {
      legend: 'none',
      curveType: 'function',
      pointSize: 13,
      series: {
        0: { pointShape: 'square', sides: 5, color: 'red', pointSize: 10 },
        1: { pointShape: 'square', sides: 5, color: 'blue', pointSize: 10 }
      },
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
        textPosition: 'none',
        gridlines: {
          count: 5
        },
      }
    },
    dataTable: [],

  }

  public radius: number = 100;
  public zoom: number = 17;
  displayedColmaintaince: string[] = ['startDateTime', 'endDateTime', 'status'];
  displayedColumns: string[] = ['tripId', 'driverName', 'startDateTime', 'endDateTime', 'totalMiles'];
  searchParameters = {
    parentEntityGuid: '',
    pageNumber: 0,
    pageSize: 5,
    searchText: '',
    sortBy: 'tripId asc'
  };

  graphChartData = {
    chartType: 'LineChart',
    options: {
      legend: 'none',
      curveType: 'function',
      pointSize: 20,
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
        textPosition: 'none',
        gridlines: {
          count: 5
        },
      }
    },
    dataTable: [],

  }
  OdometerreadingChartData = {
    chartType: 'LineChart',
    options: {
      legend: 'none',
      curveType: 'function',
      pointSize: 20,
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
        textPosition: 'none',
        gridlines: {
          count: 5
        },
      }
    },
    dataTable: [],

  }
  maintainceParameters = {
    parentEntityGuid: '',
    pageNumber: 0,
    pageSize: 10,
    searchText: '',
    sortBy: 'deviceName asc'
  };
  pageSizeOptions: number[] = [5, 10, 25, 100];
  datatrip: any = [{ text: 'Upcoming' }, { text: 'completed' }]
  totalmaintainceRecords = 0;
  totalRecords = 0;
  maintainceList: any = [];
  tripList: any = [];
  alerts: any = [];
  fleetGuid: any;
  totalTripCount: any;
  totalCompletedTripCount: any;
  totalScheduledTripCount: any;
  totalFuelConsumption: any;
  totalScheduledCount: any;
  totalMaintenanceCount: any;
  totalCompletedMaintenanceCount: any;
  totalUnderMaintenanceCount: any;
  nextMaintenanceDateTime: any;
  totalAlerts: any;
  lat: number = 0;
  lng: number = 0;
  currentLat: any = 0;
  currentLng: any = 0;
  radiusLat: any = 0;
  radiusLng: any = 0;
  labelname: any;
  fleetid: any;
  waypoints = []
  points = [];
  icon = {
    url: this.isRight ? './assets/images/truck-location-green.png' : './assets/images/truck-location-green-i.png',
    scaledSize: {
      height: 60,
      width: 60
    }
  };
  destinationLatitude: any = 0;
  destinationLongitude: any = 0;
  sourceLatitude: any = 0;
  sourceLongitude: any = 0;
  uniqueId: any;
  fleetStatus: any;
  fleetStatusDisplay: any;
  tripGuid: any;
  flag = false;
  can_odometer: any;
  radiouslat: any;
  radiouslon: any;
  constructor(private activatedRoute: ActivatedRoute,
    public _service: FleetService,
    private spinner: NgxSpinnerService,
    private _notificationService: NotificationService,
    public alertsService: AlertsService,
    private stompService: StompRService,
    private deviceService: DeviceService,
    public tripservice: TripService,
    public dashboardService: DashboardService
  ) {
    this.activatedRoute.params.subscribe(params => {
      if (params.fleetGuid != null) {
        this.fleetGuid = params.fleetGuid;
        this.getFleetDetails(params.fleetGuid);
        this.getMaintainceList();
      }
    });
  }

  ngOnInit() {
    this.labelname = 'Upcoming';
    this.gettripdata(this.labelname);
    this.getAlertList();
    this.getfleetTripgraph();
    this.getfleetMileaggraph();
    this.getOdometerreadingbyfleet();
    document.getElementById('currentLocation').innerHTML = "No location found";
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
    if (this.uniqueId != undefined) {
      this.messages = this.stompService.subscribe('/topic/' + this.cpId + '-' + this.uniqueId);
      this.subscription = this.messages.subscribe(this.on_next);
      this.subscribed = true;
    }
  }
  public on_next = (message: Message) => {
    let uniqeId = (message.headers.destination).split("-");
    if (this.uniqueId == uniqeId[1]) {
      let obj: any = JSON.parse(message.body);
      if (obj.data.msgType === 'telemetry') {
        var reporting_data = obj.data.data.reporting
        let dates = obj.data.data.time;
        let now = moment();
        if (obj.data.data.reporting) {
          if (reporting_data.can_engine_rpm <= 999 || reporting_data.can_engine_rpm == 0.00) {
            this.isStarted = false;
          }
          else {
            this.isStarted = true;
          }
          this.currentLat = obj.data.data.reporting.gps_lat;
          this.currentLng = obj.data.data.reporting.gps_lng;

          this.radiusLat = +obj.data.data.reporting.gps_lat;
          this.radiusLng = +obj.data.data.reporting.gps_lng;

          this.lat = obj.data.data.reporting.gps_lat;
          this.lng = obj.data.data.reporting.gps_lng;
          this.attributes.gps_altitude = obj.data.data.reporting.gps_altitude;
          this.attributes.gps_time = obj.data.data.reporting.gps_time;
          this.attributes.gateway_uptime = obj.data.data.reporting.gateway_uptime;
          this.attributes.vehicle_ign_sense = obj.data.data.reporting.vehicle_ign_sense;
          this.attributes.gps_num_sats = obj.data.data.reporting.gps_num_sats;
          this.attributes.can_engine_rpm = obj.data.data.reporting.can_engine_rpm;
          this.attributes.can_engine_rpm_total = +obj.data.data.reporting.can_engine_rpm_total;
          this.attributes.can_distance_to_service = obj.data.data.reporting.can_distance_to_service;
          this.attributes.can_hours_operation = +obj.data.data.reporting.can_hours_operation;
          this.attributes.can_odometer = obj.data.data.reporting.can_odometer;
          this.can_odometer = obj.data.data.reporting.can_odometer;
          var google_map_pos = new google.maps.LatLng(this.currentLat, this.currentLng);

          /* Use Geocoder to get address */
          var google_maps_geocoder = new google.maps.Geocoder();
          google_maps_geocoder.geocode(
            { 'latLng': google_map_pos },
            function (results, status) {
              if (status == google.maps.GeocoderStatus.OK && results[0]) {
                var currentAddress = results[0].formatted_address;
                $('#currentLocation').text(currentAddress);
                //document.getElementById('currentLocation').innerHTML = results[0].formatted_address;
              }
            }
          );

          this.remailingMiles = this.tripservice.calculateTotalMiles(this.currentLat, this.currentLng, this.destinationLatitude, this.destinationLongitude);
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
            this.tripservice.endTrip(data).subscribe(response => {
              if (response.isSuccess === true) {
                this.flag = true;
                this._notificationService.add(new Notification('success', "Trip has been complated successfully."));
              } else {
                this.flag = true;
              }
            })
          }
          let arrivalResponse = this.tripservice.calculateArrivalTime(reporting_data.can_vehicle_speed, distanceInKm);
          if (arrivalResponse != '')
            this.arrivalTime = this.tripservice.hoursToDhms(arrivalResponse);
          else if (parseFloat(arrivalResponse) == 0)
            this.arrivalTime = '0 second';
          else { }
          var start = new google.maps.LatLng(this.lat, this.lng);
          var end = new google.maps.LatLng(this.destinationLatitude, this.destinationLongitude);
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
                    "currentDate": currentdatetime,
                    "odometer": Math.floor(objsign.can_odometer),
                    "timeZone": timezone
                  }
                  objsign.tripservice.startTrip(data).subscribe(response => {
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
  }

  event(type, $event) {
    this.radius = +$event;
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
  getMaintainceDate(lDate) {
    var utcDate = moment.utc(lDate, 'DD-MM-YYYY');
    return utcDate;

  }

  /**
   * For :  Get All ALerts By Facility 
   * @param fleetId For
   */
  getAlertList() {
    this.alerts = [];
    let currentdatetime = moment().format('YYYY-MM-DD[T]HH:mm:ss');
    let timezone = moment().utcOffset();
    let parameters = {
      "fleetGuid": this.fleetGuid,
      "currentDate": currentdatetime,
      "timeZone": timezone,
      "searchText": "",
      "pageNo": 0,
      "pageSize": 10,
      orderBy: 'eventDate desc'
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
  * For :  Get tripdata 
  * @param status
  */
  gettripdata(status) {
    this.spinner.show();
    let currentdatetime = moment().format('YYYY-MM-DD[T]HH:mm:ss');
    let timezone = moment().utcOffset();

    const parameter = {
      'parentEntityGuid': this.searchParameters.parentEntityGuid,
      'pageNo': this.searchParameters.pageNumber + 1,
      'pageSize': this.searchParameters.pageSize,
      'searchText': this.searchParameters.searchText,
      'orderBy': this.searchParameters.sortBy,
      'currentDate': currentdatetime.toString(),
      'timeZone': timezone.toString(),
      "fleetGuid": this.fleetGuid,
      "status": status

    };
    this._service.getFleetBytrip(parameter).subscribe(response => {
      this.spinner.hide();

      if (response.isSuccess === true) {
        this.totalRecords = response.data.count;
        this.tripList = response.data.items;
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
  * For :  Get fleetTripgraph
  */
  getfleetTripgraph() {
    this.spinner.show();
    let parameter = {
      "fleetGuid": this.fleetGuid
    }
    this._service.getFleetTripgraph(parameter).subscribe(response => {
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
              title: 'Fuel(Gallons)',
              textPosition: 'none',
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
     * For :  Get Odometerreadingbyfleet
     */
  getOdometerreadingbyfleet() {
    this.spinner.show();
    let parameter = {
      "fleetGuid": this.fleetGuid
    }
    this._service.getOdometerreadingbyfleet(parameter).subscribe(response => {
      this.spinner.hide();
      if (response.isSuccess === true) {
        let data = [];
        data.push(["", ""])
        response.data.forEach(element => {
          data.push([element.name, parseFloat(element.odometer)])
        });
        this.OdometerreadingChartData = {
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
              title: 'KM',
              textPosition: 'none',
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
     * For :  Get fleetMileaggraph
     */
  getfleetMileaggraph() {
    this.spinner.show();
    let parameter = {
      "fleetGuid": this.fleetGuid
    }
    this._service.getFleetTripgraph(parameter).subscribe(response => {
      this.spinner.hide();
      if (response.isSuccess === true) {
        let data = [];
        data.push(["", "Fuel Consumption", "Mileage Average"])
        response.data.forEach(element => {
          data.push([element.name, parseFloat(element.energyConsumption), parseFloat(element.mileageAverage)])
        });
        this.lineChartData = {
          chartType: 'LineChart',
          options: {
            legend: 'bottom',
            curveType: 'function',
            pointSize: 15,
            series: {
              0: { pointShape: 'square', sides: 5, color: 'red', pointSize: 10 },
              1: { pointShape: 'square', sides: 5, color: 'blue', pointSize: 10 }
            },
            height: 400,
            interpolateNulls: true,
            hAxis: {
              title: '',
              gridlines: {
                count: 5
              },
            },
            vAxis: {
              title: '',
              textPosition: 'right',
              gridlines: {
                count: 5
              },
            }
          },
          dataTable: data
        };
      }
    }, error => {
      this.spinner.hide();
      this._notificationService.add(new Notification('error', error));
    });

  }
  /**
     * For :  Get MaintainceAsPageChange
     */
  MaintainceAsPageChange(pagechangeresponse) {
    this.maintainceParameters.pageSize = pagechangeresponse.pageSize;
    this.maintainceParameters.pageNumber = pagechangeresponse.pageIndex;
    this.getMaintainceList();
  }
  /**
   * For :  MaintaincesetOrder
   */
  MaintaincesetOrder(sort: any) {
    if (!sort.active || sort.direction === '') {
      return;
    }
    this.maintainceParameters.sortBy = sort.active + ' ' + sort.direction;
    this.getMaintainceList();
  }
  /**
  * Change page event
  * @param pagechangeresponse
  */
  ChangePaginationAsPageChange(pagechangeresponse) {
    this.searchParameters.pageSize = pagechangeresponse.pageSize;
    this.searchParameters.pageNumber = pagechangeresponse.pageIndex;
    this.gettripdata(this.labelname);
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
    this.gettripdata(this.labelname)
  }
  /**
    * onTabChange change Event
    */
  onTabChange(tab) {
    if (tab != undefined && tab != '') {
      this.labelname = tab.tab.textLabel;
      this.gettripdata(this.labelname)
    }
  }
  /**
    * Get fleet details by fleetGuid
    * @param fleetGuid
    */
  getFleetDetails(fleetGuid) {
    this.spinner.show();
    this._service.getFleetdashdetail(fleetGuid).subscribe(response => {
      if (response.isSuccess === true) {
        this.deviceGuid = response.data.deviceGuid;
        if (this.deviceGuid) {
          this.getTelemetryDetails(this.deviceGuid);
        }
        this.totalTripCount = (response.data.totalTripCount) ? response.data.totalTripCount : 0
        this.totalCompletedTripCount = (response.data.totalCompletedTripCount) ? response.data.totalCompletedTripCount : 0
        this.totalScheduledTripCount = (response.data.totalScheduledTripCount) ? response.data.totalScheduledTripCount : 0
        this.totalFuelConsumption = (response.data.totalFuelConsumption) ? response.data.totalFuelConsumption : 0;
        this.totalFuelConsumption = this.dashboardService.transform(this.totalFuelConsumption);
        this.totalScheduledCount = (response.data.totalScheduledCount) ? response.data.totalScheduledCount : 0;
        this.totalMaintenanceCount = (response.data.totalMaintenanceCount) ? response.data.totalMaintenanceCount : 0;
        this.totalCompletedMaintenanceCount = (response.data.totalCompletedMaintenanceCount) ? response.data.totalCompletedMaintenanceCount : 0;
        this.totalUnderMaintenanceCount = (response.data.totalUnderMaintenanceCount) ? response.data.totalUnderMaintenanceCount : 0
        this.nextMaintenanceDateTime = (response.data.nextMaintenanceDateTime) ? response.data.nextMaintenanceDateTime : ''
        this.totalAlerts = (response.data.totalAlerts) ? response.data.totalAlerts : 0
        this.fleetid = (response.data.fleetId) ? response.data.fleetId : ''
        this.radiouslat = +response.data.latitude
        this.radiouslon = +response.data.longitude
        this.radius = (response.data.radius) ? response.data.radius : '';
        this.fleetStatusDisplay = (response.data.fleetStatus) ? '(' + response.data.fleetStatus + ')' : '';
        this.fleetStatus = (response.data.fleetStatus) ? response.data.fleetStatus : ''
        this.destinationLatitude = (response.data.destinationLatitude) ? response.data.destinationLatitude : ''
        this.destinationLongitude = (response.data.destinationLongitude) ? response.data.destinationLongitude : ''
        this.sourceLatitude = (response.data.sourceLatitude) ? response.data.sourceLatitude : ''
        this.sourceLongitude = (response.data.sourceLongitude) ? response.data.sourceLongitude : ''
        this.origin = { lat: +this.sourceLatitude, lng: +this.sourceLongitude };
        this.destination = { lat: +this.destinationLatitude, lng: +this.destinationLongitude };
        if (this.sourceLatitude) {
          this.lat = +response.data.sourceLatitude;
          this.lng = +response.data.sourceLongitude;

          this.radiusLat = +response.data.sourceLatitude;
          this.radiusLng = +response.data.sourceLongitude;
        } else {
          this.lat = +response.data.latitude;
          this.lng = +response.data.longitude;

          this.radiusLat = +response.data.latitude;
          this.radiusLng = +response.data.longitude
        };

        var google_map_pos = new google.maps.LatLng(this.lat, this.lng);

        /* Use Geocoder to get address */
        var google_maps_geocoder = new google.maps.Geocoder();
        google_maps_geocoder.geocode(
          { 'latLng': google_map_pos },
          function (results, status) {
            if (status == google.maps.GeocoderStatus.OK && results[0]) {
              var currentAddress = results[0].formatted_address;
              $('#currentLocation').text(currentAddress);
              //document.getElementById('currentLocation').innerHTML = results[0].formatted_address;
            }
          }
        );

        this.truckDirection = this.tripservice.vehicleBearing(this.origin, this.destination);
        
        if (this.fleetStatus == 'Trip Completed') {
          this.currentLat = this.destinationLatitude;
          this.currentLng = this.destinationLongitude;

          this.radiusLat = +this.destinationLatitude;
          this.radiusLng = +this.destinationLongitude;

        } else {
          this.currentLat = this.sourceLatitude;
          this.currentLng = this.sourceLongitude;
        }
        this.uniqueId = (response.data.uniqueId) ? response.data.uniqueId : ''
        if (this.uniqueId && (this.fleetStatus != 'Trip Completed' && this.fleetStatus != 'Overdue' && this.fleetStatus != 'Maintenance Completed' && this.fleetStatus != 'Maintenance')) {
          this.getStompConfig();
        }
        this.tripGuid = (response.data.tripGuid) ? response.data.tripGuid : ''
        this.totalMiles = this.tripservice.calculateTotalMiles(this.sourceLatitude, this.sourceLongitude, this.destinationLatitude, this.destinationLongitude);

      }
      else {
        this._notificationService.add(new Notification('error', response.message));
      }
      this.spinner.hide();
    }, error => {
      this.spinner.hide();
      this._notificationService.add(new Notification('error', error));
    });
  }

  getTelemetryDetails(deviceGuid) {
    this.deviceService.gettelemetryDetails(deviceGuid).subscribe(response => {
      if (response.isSuccess) {

        this.deviceAttribute = response.data;
        this.deviceAttribute.forEach(element => {
          this.attributes[element['attributeName']] = element.attributeValue;

        });
        if (this.attributes.gps_lat && this.attributes.gps_lng) {
          this.lat = +this.attributes.gps_lat
          this.lng = +this.attributes.gps_lng
        }
      }
    }, error => {
      this.spinner.hide();
      this._notificationService.add(new Notification('error', error));
    });
  }

  /**
   * Get MaintainceList
   */
  getMaintainceList() {
    this.spinner.show();
    const reqParameter: any = {
      'deviceId': this.fleetGuid,
      'pageNo': this.maintainceParameters.pageNumber + 1,
      'pageSize': this.maintainceParameters.pageSize,
      'searchText': this.maintainceParameters.searchText,
      'orderBy': this.maintainceParameters.sortBy,
      'currentDate': moment(new Date()).format('YYYY-MM-DDTHH:mm:ss'),
      'timeZone': moment().utcOffset()
    };
    this._service.getMaintenancelist(reqParameter).subscribe(response => {
      this.spinner.hide();

      if (response.isSuccess === true) {
        this.totalmaintainceRecords = response.data.count;
        this.maintainceList = response.data.items;
      }
      else {
        this._notificationService.add(new Notification('error', response.message));
        this.maintainceList = [];
      }
    }, error => {
      this.spinner.hide();
      this._notificationService.add(new Notification('error', error));
    });

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
   * Get checkZero
   */
  checkZero(data) {
    if (data.length == 1) {
      data = "0" + data;
    }
    return data;
  }
  /**
   * Get FormattedDateTime
   */
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
