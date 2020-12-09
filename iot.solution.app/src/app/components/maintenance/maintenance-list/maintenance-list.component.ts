import { Component, OnInit } from '@angular/core';
import { NgxSpinnerService } from 'ngx-spinner';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import * as moment from 'moment-timezone';
import { DeleteDialogComponent } from '../..';
import { MatDialog } from '@angular/material';
import { LookupService, Notification, NotificationService, DeviceService, TripService } from '../../../services';
import { AppConstant, DeleteAlertDataModel } from '../../../app.constants';
@Component({
  selector: 'app-maintenance-list',
  templateUrl: './maintenance-list.component.html',
  styleUrls: ['./maintenance-list.component.css']
})
export class MaintenanceListComponent implements OnInit {

  displayedColumns: string[] = ['deviceName', 'startDateTime', 'status', 'actions'];
  zones: any[];
  isAdmin = false;
  currentUser = JSON.parse(localStorage.getItem("currentUser"));
  fleetList: any = [];
  isFilterShow: boolean = false;
  checkFilterSubmitStatus: boolean = false;
  pageSizeOptions: number[] = [5, 10, 25, 100];
  maintenanceList: any = [];
  filterForm: FormGroup;
  totalRecords = 0;
  searchParameters = {
    deviceId: '',
    pageNumber: 0,
    pageSize: 10,
    searchText: '',
    sortBy: 'deviceName asc'
  };
  isSearch = false;

  constructor(
    public dialog: MatDialog,
    public lookupService: LookupService,
    public _notificationService: NotificationService,
    private formBuilder: FormBuilder,
    private _service: DeviceService,
    public _appConstant: AppConstant,
    private spinner: NgxSpinnerService,
    public tripService: TripService) { }

  ngOnInit() {
    if (this.currentUser.userDetail.roleName == "Admin") {
      this.isAdmin = true;
    }
    else {
      this.isAdmin = false;
    }
    this.createFilterFormGroup();
    let currentUser = JSON.parse(localStorage.getItem('currentUser'));
    this.getFleetLookup(currentUser.userDetail.companyId);
    this.getMaintenance();
  }

  /**
   * 
   * called on location change
   */
  onLocationChange(event){
  this.filterForm.get('zoneGuid').setValue(''); 
    this.checkFilterSubmitStatus=false;
    this.getZonesLookup(event);
  }

  /**
   * get the lookup values for zones
   * @param event 
   */
  getZonesLookup(event) {
    this.spinner.show();
    this.lookupService.getZonelookup(event.value).subscribe(response => {
      this.spinner.hide();
      if (response.isSuccess) {
        this.zones = response.data;
      } else {
        this._notificationService.add(new Notification("error", response.message));
      }
    }, error => {
      this._notificationService.add(new Notification("error", error));
    });
  }

  /**
   * filter by serach text
   * @param event 
   */
  searchTextCallback(event) {
    this.searchParameters.searchText = event;
    this.getMaintenance();
  }

  /**
 * Show hide filter
 */
  public showHideFilter() {
    this.isFilterShow = !this.isFilterShow;
  }

  /**
   * Get maintenence list
   * */
  getMaintenance() {
    this.spinner.show();
    this._service.getMaintenancelist(this.searchParameters).subscribe((response: any) => {
      this.spinner.hide();
      this.totalRecords = response.data.count;
      if (response.data.count) {
        this.maintenanceList = response.data.items;
      } else {
        this.maintenanceList = [];
      }
    }, error => {
      this.spinner.hide();
      this._notificationService.add(new Notification('error', error));
    });
  }

  /**
   * create form for filter 
   */
  createFilterFormGroup() {
    this.filterForm = this.formBuilder.group({
      entityGuid: ['',Validators.required]
    });
  }

/**
* Get Location Lookup by companyId
* @param companyId
*/
getFleetLookup(companyId) {
  this.spinner.show();
  this.lookupService.getfleetlookup().
    subscribe(response => {
      this.spinner.hide();
      if (response.isSuccess === true) {
        this.fleetList = response.data;

      } else {
        this._notificationService.add(new Notification('error', response.message));
      }
    }, error => {
      this.spinner.hide();
      this._notificationService.add(new Notification('error', error));
    })
}


  /**
   * For filter maintanance list 
   * */
  filterCall() {
    this.checkFilterSubmitStatus = true;
    if (this.filterForm.valid) {
      this.searchParameters.deviceId = this.filterForm.value.entityGuid;
      this.getMaintenance();
    }
  }

  /**
   * for shorting
   * @param sort 
   */
  setOrder(sort: any) {
    if (!sort.active || sort.direction === '') {
      return;
    }
    if (sort.active == 'deviceName') {
      sort.active = 'deviceName';
    }
    this.searchParameters.sortBy = sort.active + ' ' + sort.direction;
    this.getMaintenance();
  }

  /**
   * For Cleare filter maintanance list
   */
  clearFilter() {
    this.filterForm.reset();
    this.isFilterShow = false;
    this.checkFilterSubmitStatus = false;
    this.searchParameters.deviceId = '';
    this.getMaintenance();
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
   * delert alert Model
   */
  deleteAlertDataModel: DeleteAlertDataModel;

  /**
   * pop-up model for delete 
   * @param model 
   */
  deleteModel(model: any) {
    this.deleteAlertDataModel = {
      title: "Delete Scheduled Maintenance",
      message: this._appConstant.msgConfirm.replace('modulename', "Scheduled Maintenance"),
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
        this.deleteMaintenance(model.guid);
      }
    });
  }

  /**
   * for deleting the Maintenance 
   * @param guid 
   */
  deleteMaintenance(guid) {
    this.spinner.show();
    this._service.deleteMaintenance(guid).subscribe(response => {
      this.spinner.hide();
      if (response.isSuccess === true) {
        this._notificationService.add(new Notification('success', this._appConstant.msgDeleted.replace("modulename", "Scheduled Maintenance")));
        this.getMaintenance();
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
   * change pagination size and reload the data
   * @param pageChangeResponse 
   */
  changePaginationAsPageChange(pageChangeResponse) {
    this.searchParameters.pageNumber = pageChangeResponse.pageIndex;
    this.searchParameters.pageSize = pageChangeResponse.pageSize;
    this.isSearch = true;
    this.getMaintenance();
  }

  /**
   * Complete maintenance comfirmation popup
   * @param tripModel
   */
  completeModel(maintenanceModel: any) {
    this.deleteAlertDataModel = {
      title: "Complete Maintenance",
      message: "Are you sure you want to complete maintenance?",
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
        this.completeMaintance(maintenanceModel);
      }
    });
  }

  /**
   * Complete Maintenance
   * @param maintenanceModel
   */
  completeMaintance(maintenanceModel) {
    let currentdatetime = moment().format('YYYY-MM-DD[T]HH:mm:ss');
    let timezone = moment().utcOffset();
    let data = {
      "guid": maintenanceModel.guid,
      "currentDate": currentdatetime,
      "timeZone": timezone,
    }
    this._service.completeMaintenence(data).subscribe(response => {
      if (response.isSuccess === true) {
        this.getMaintenance();
      }
    })
  }

}
