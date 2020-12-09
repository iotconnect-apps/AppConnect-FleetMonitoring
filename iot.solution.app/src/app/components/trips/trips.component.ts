import { Component, OnInit, ViewChild } from '@angular/core';
import { MatPaginator } from '@angular/material/paginator';
import { MatTableDataSource } from '@angular/material/table';
import { NgxSpinnerService } from 'ngx-spinner';
import { Router } from '@angular/router';
import { MatDialog } from '@angular/material';
import { TripService, NotificationService, Notification, DeviceService } from '../../services';
import { AppConstant, DeleteAlertDataModel } from '../../app.constants';
import { DeleteDialogComponent } from '..';
import { Subscription, Observable } from 'rxjs';
import { Message } from '@stomp/stompjs';
import { StompRService } from '@stomp/ng2-stompjs';
import * as moment from 'moment-timezone';
import { MessageDialogComponent } from '../common/message-dialog/message-dialog.component';
import { FormGroup, FormControl, Validators } from '@angular/forms';

@Component({
  selector: 'app-trips',
  templateUrl: './trips.component.html',
  styleUrls: ['./trips.component.css'],
  providers: [StompRService]
})
export class TripsComponent implements OnInit {

  displayedColumns: string[] = ['tripId', 'fleetName', 'driverName', 'startDateTime', 'endDateTime', 'materialType', 'weight', 'coveredMiles', 'status', 'action'];
  searchParameters = {
    startDate: '',
    endDate: '',
    status:'',
    driverGuid: '',
    fleetGuid:'',
    pageNumber: 0,
    pageSize: 10,
    searchText: '',
    sortBy: 'tripId asc'
  };
  tripForm: FormGroup;
  checkSubmitStatus: boolean;
  isFilterShow: boolean = false;
  topics: any[] = [];
  reportingData: any = {};
  totalRecords = 0;
  tripList = [];
  isSearch = false;
  pageSizeOptions: number[] = [5, 10, 25, 100];
  deleteAlertDataModel: DeleteAlertDataModel;

  uniqueId: any;
  subscription: Subscription;
  messages: Observable<Message>;
  currentUser = JSON.parse(localStorage.getItem("currentUser"));
  deviceIsConnected = false;
  isConnected = false;
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
  lat: number;
  lng: number;
  totalMiles: number = 0;
  remailingMiles: number = 0;
  coveredMiles: number = 0;
  progressMilesPerc: number = 0;
  isAdmin = false;

  constructor(
    private spinner: NgxSpinnerService,
    private router: Router,
    public dialog: MatDialog,
    public _service: TripService,
    private _notificationService: NotificationService,
    public _appConstant: AppConstant,
    public deviceService: DeviceService,
    private stompService: StompRService,) { }

  ngOnInit() {
    this.getTripList();
    this.createFilterForm();

    if (this.currentUser.userDetail.roleName == "Admin") {
      this.isAdmin = true;
    }
    else {
      this.isAdmin = false;
    }
  }

  /**
   * create Filter Form
   */
  createFilterForm() {
    this.tripForm = new FormGroup({
      startDate: new FormControl('', Validators.required),
      endDate: new FormControl('', Validators.required)
    });
  }

  /**
   * Change page event
   * @param pagechangeresponse
   */
  ChangePaginationAsPageChange(pagechangeresponse) {
    this.searchParameters.pageSize = pagechangeresponse.pageSize;
    this.searchParameters.pageNumber = pagechangeresponse.pageIndex;
    this.isSearch = true;
    this.getTripList();
  }

  /**
   * Searh for text
   * @param filterText
   */
  searchTextCallback(filterText) {
    this.searchParameters.searchText = filterText;
    this.searchParameters.pageNumber = 0;
    this.isSearch = true;
    this.getTripList();
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
    this.getTripList();
  }

  /**
   * Get trip list 
   * */
  getTripList() {
    this.spinner.show();
    this._service.getTrip(this.searchParameters).subscribe(response => {
      this.spinner.hide();

      if (response.isSuccess === true) {
        this.totalRecords = response.data.count;
        this.tripList = response.data.items;
        if (this.tripList) {
          this.getStompConfig();
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
   * Delete trip comfirmation popup
   * @param tripModel
   */
  deleteModel(tripModel: any) {
    this.deleteAlertDataModel = {
      title: "Delete Trip",
      message: this._appConstant.msgConfirm.replace('modulename', "Trip"),
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
        this.deleteTrip(tripModel.guid);
      }
    });
  }

  /**
   * Delete trip by tripGuid
   * @param tripGuid
   */
  deleteTrip(tripGuid) {
    this.spinner.show();
    this._service.deleteTrip(tripGuid).subscribe(response => {
      this.spinner.hide();
      if (response.isSuccess === true) {
        this._notificationService.add(new Notification('success', this._appConstant.msgDeleted.replace("modulename", "Trip")));
        this.getTripList();

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
    * Stomp subscribe
    * */
  public stompSubscribe() {
    if (this.subscribed) {
      return;
    }

    this.tripList.forEach(element => {
      if (!this.topics.find(x => x.uniqueId === element.uniqueId)) {

        this.messages = this.stompService.subscribe('/topic/' + this.cpId + '-' + element.uniqueId);
        this.subscription = this.messages.subscribe(this.on_next);
        this.topics.push({ uniqueId: element.uniqueId, destinationLatitude: element.destinationLatitude, destinationLongitude: element.destinationLongitude });
      }
    });
  }

  public on_next = (message: Message) => {
    let uniqeId = (message.headers.destination).split("-");
    let obj: any = JSON.parse(message.body);

    let reporting_data = obj.data.data.reporting;
    this.isConnected = true;
    this.reportingData = reporting_data
    if (this.reportingData) {
      this.lat = this.reportingData.gps_lat;
      this.lng = this.reportingData.gps_lng;

      this.topics.forEach(element => {
        if (element.uniqueId == uniqeId[1]) {
          this.remailingMiles = this._service.calculateTotalMiles(this.lat, this.lng, element.destinationLatitude, element.destinationLongitude);
        }
      });

      for (const key in this.tripList) {
        if (this.tripList[key].uniqueId == uniqeId[1] && this.tripList[key].status == 'On Going') {
          this.tripList[key].coveredMiles = this.tripList[key].totalMiles - this.remailingMiles;
        }
      }
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

  /**
   * it used to get the Date in local form
   * @param lDate 
   */
  getLocalDate(lDate) {
    var stillUtc = moment.utc(lDate).toDate();
    var local = moment(stillUtc).local().format('MMM DD, YYYY hh:mm:ss A');
    return local;
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
        this.getTripList();
      }
    })
  }

  /**
   * Show hide filter
   * */
  showHideFilter() {
    this.isFilterShow = !this.isFilterShow;
  }

  /**
   * filter trip  list
   */
  filterTripList() {
    this.checkSubmitStatus = true;
    if (this.tripForm.valid) {
      this.searchParameters.startDate = this.tripForm.get("startDate").value;
      this.searchParameters.endDate = this.tripForm.get("endDate").value;
      this.getTripList();
    }
  }

  resetForm() {
    this.tripForm.reset();
    this.checkSubmitStatus = false;
    this.searchParameters.startDate = "";
    this.searchParameters.endDate = "";
    //this.showHideFilter();
    this.getTripList();
  }
}
