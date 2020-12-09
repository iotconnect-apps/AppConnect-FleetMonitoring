import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router'
import { NgxSpinnerService } from 'ngx-spinner'
import { MatDialog } from '@angular/material'
import { DeleteDialogComponent } from '../../../components/common/delete-dialog/delete-dialog.component';
import { Notification, NotificationService } from './../../../services/index'
import { AppConstant, DeleteAlertDataModel } from "../../../app.constants";
import { DriverService } from '../../../services/driver/driver.service';
import * as moment from 'moment';


@Component({
  selector: 'app-driver',
  templateUrl: './driver.component.html',
  styleUrls: ['./driver.component.css']
})
export class DriverComponent implements OnInit {


  moduleName = "Driver";

  isSearch = false;
  totalRecords = 0;
  pageSizeOptions: number[] = [5, 10, 25, 100];

  searchParameters = {
    pageNumber: 0,
    pageSize: 10,
    searchText: '',
    sortBy: 'name asc'
  };
  displayedColumns: string[] = ['driverId', 'firstName', 'lastName', 'email', 'contactNo', 'licenceNo', 'fleetName', 'isActive', 'action'];
  driverList = [];
  deleteAlertDataModel: DeleteAlertDataModel;


  constructor(
    private spinner: NgxSpinnerService,
    private router: Router,
    public dialog: MatDialog,
    public driverService: DriverService,
    private _notificationService: NotificationService,
    public _appConstant: AppConstant
  ) { }

  ngOnInit() {
    this.getDriverList();
  }

  /**
   * go to add driver page
   */
  clickAdd() {
    this.router.navigate(['/drivers/add']);
  }

  /**
   * to sort the data y column name
   * @param sort 
   */
  setOrder(sort: any) {
    if (!sort.active || sort.direction === '') {
      return;
    }
    this.searchParameters.sortBy = sort.active + ' ' + sort.direction;
    this.getDriverList();
  }

  /**
   * to open confironmation popUp to delete the driver
   * @param userModel 
   */
  deleteModel(userModel: any) {
    this.deleteAlertDataModel = {
      title: "Delete Driver",
      message: this._appConstant.msgConfirm.replace('modulename', "Driver"),
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
        this.deleteDriver(userModel.guid);
      }
    });
  }

  /**
   * to open the confiromation popUp for active/inactive status
   * @param driverId 
   * @param isActive 
   * @param name 
   */
  activeInactiveDriver(driverId: string, isActive: boolean, name: string) {
    var status = isActive == false ? this._appConstant.activeStatus : this._appConstant.inactiveStatus;
    var mapObj = {
      statusname: status,
      fieldname: name,
      modulename: "Driver"
    };
    this.deleteAlertDataModel = {
      title: "Status",
      message: this._appConstant.msgStatusConfirm.replace(/statusname|fieldname|modulename/gi, function (matched) {
        return mapObj[matched];
      }),
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
        this.changeDriverStatus(driverId, isActive);

      }
    });

  }

  /**
   * 
   * @param pagechangeresponse 
   */
  changePaginationAsPageChange(pagechangeresponse) {
    this.searchParameters.pageSize = pagechangeresponse.pageSize;
    this.searchParameters.pageNumber = pagechangeresponse.pageIndex;
    this.isSearch = true;
    this.getDriverList();
  }

  /**
   * to filter by search
   * @param filterText 
   */
  searchTextCallback(filterText) {
    this.searchParameters.searchText = filterText;
    this.searchParameters.pageNumber = 0;
    this.isSearch = true;
    this.getDriverList();
  }

  /**
   * to get the driver list/filter/short
   */
  getDriverList() {
    this.spinner.show();

    let currentdatetime = moment().format('YYYY-MM-DD[T]HH:mm:ss');
    let timezone = moment().utcOffset();

    let param = {
      "pageNo": this.searchParameters.pageNumber + 1,
      "pageSize": this.searchParameters.pageSize,
      "searchText": this.searchParameters.searchText,
      "orderBy": this.searchParameters.sortBy,
      "currentDate": currentdatetime,
      "timeZone": timezone
    }

    this.driverService.getDriverList(param).subscribe(response => {
      this.spinner.hide();

      if (response.isSuccess === true) {
        this.totalRecords = response.data.count;
        // this.isSearch = false;
        this.driverList = response.data.items;
      }
      else {
        this.totalRecords = 0;
        this._notificationService.add(new Notification('error', response.message));
        this.driverList = [];
      }
    }, error => {
      this.totalRecords = 0;
      this.driverList = [];
      this.spinner.hide();
      this._notificationService.add(new Notification('error', error));
    });
  }

  /**
   * to change the driver status from active/inactive
   * @param driverId
   * @param isActive 
   */
  changeDriverStatus(driverId, isActive) {

    this.spinner.show();
    this.driverService.changeStatus(driverId, isActive).subscribe(response => {
      this.spinner.hide();
      if (response.isSuccess === true) {
        this._notificationService.add(new Notification('success', this._appConstant.msgStatusChange.replace("modulename", "Driver")));
        this.getDriverList();

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
   * to delete the driver
   * @param driverId
   */
  deleteDriver(driverId) {
    this.spinner.show();
    this.driverService.deleteDriver(driverId).subscribe(response => {
      this.spinner.hide();
      if (response.isSuccess === true) {
        this._notificationService.add(new Notification('success', this._appConstant.msgDeleted.replace("modulename", "Driver")));
        this.getDriverList();

      }
      else {
        this._notificationService.add(new Notification('error', response.message));
      }

    }, error => {
      this.spinner.hide();
      this._notificationService.add(new Notification('error', error));
    });
  }


}
