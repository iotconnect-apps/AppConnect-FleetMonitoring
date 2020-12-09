import { Component, OnInit, ViewChild } from '@angular/core';
import {MatPaginator} from '@angular/material/paginator';
import {MatTableDataSource} from '@angular/material/table';
import { NgxSpinnerService } from 'ngx-spinner';
import { Router } from '@angular/router';
import { MatDialog } from '@angular/material';
import { FleetService, NotificationService, Notification} from '../../services';
import { AppConstant, DeleteAlertDataModel } from '../../app.constants';
import { DeleteDialogComponent } from '..';
import * as moment from 'moment'

@Component({
  selector: 'app-fleet',
  templateUrl: './fleet.component.html',
  styleUrls: ['./fleet.component.css']
})
export class FleetComponent implements OnInit {

  displayedColumns: string[] = ['fleetId', 'fleetTypeName', 'registrationNo', 'loadingCapacity', 'materialTypeName','templateName','status','action'];
  searchParameters = {
    parentEntityGuid:'',
    pageNumber: 0,
    pageSize: 10,
    searchText: '',
    sortBy: 'fleetId asc'
  };
  totalRecords = 0;
  fleetList = [];
  isSearch = false;
  pageSizeOptions: number[] = [5, 10, 25, 100];
  deleteAlertDataModel: DeleteAlertDataModel; 

  constructor(
    private spinner: NgxSpinnerService,
    private router: Router,
    public dialog: MatDialog,
    public _service: FleetService,
    private _notificationService: NotificationService,
    public _appConstant: AppConstant) { }

  ngOnInit() {
    this.getFleetList();
  }

  /**
   * Change page event
   * @param pagechangeresponse
   */
  ChangePaginationAsPageChange(pagechangeresponse) {
    this.searchParameters.pageSize = pagechangeresponse.pageSize;
    this.searchParameters.pageNumber = pagechangeresponse.pageIndex;
    this.isSearch = true;
    this.getFleetList();
  }

  /**
   * Searh for text
   * @param filterText
   */
  searchTextCallback(filterText) {
    this.searchParameters.searchText = filterText;
    this.searchParameters.pageNumber = 0;
    this.isSearch = true;
    this.getFleetList();
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
    this.getFleetList();
  }

  /**
   * Get fleet list 
   * */
  getFleetList() {
    this.spinner.show();
    let currentdatetime = moment().format('YYYY-MM-DD[T]HH:mm:ss');
    let timezone = moment().utcOffset();
    let currentUser = JSON.parse(localStorage.getItem('currentUser'));
    let parameter = {
        'pageNo': this.searchParameters.pageNumber + 1,
        'pageSize': this.searchParameters.pageSize,
        'searchText': this.searchParameters.searchText,
        'orderBy': this.searchParameters.sortBy,
        'currentDate': currentdatetime.toString(),
        'timeZone': timezone.toString()
    }; 
    
    this._service.getFleet(parameter).subscribe(response => {
      this.spinner.hide();

      if (response.isSuccess === true) {
        this.totalRecords = response.data.count;
        this.fleetList = response.data.items;
      }
      else {
        this._notificationService.add(new Notification('error', response.message));
        this.fleetList = [];
      }
    }, error => {
      this.spinner.hide();
      this._notificationService.add(new Notification('error', error));
    });
  }

  /**
   * Delete fleet comfirmation popup
   * @param fleetModel
   */
  deleteModel(fleetModel: any) {
    this.deleteAlertDataModel = {
      title: "Delete Fleet",
      message: this._appConstant.msgConfirm.replace('modulename', "Fleet"),
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
        this.deleteFleet(fleetModel.guid);
      }
    });
  }

  /**
   * Delete fleet by fleetGuid
   * @param fleetGuid
   */
  deleteFleet(fleetGuid) {
    this.spinner.show();
    this._service.deleteFleet(fleetGuid).subscribe(response => {
      this.spinner.hide();
      if (response.isSuccess === true) {
        this._notificationService.add(new Notification('success', this._appConstant.msgDeleted.replace("modulename", "Fleet")));
        this.getFleetList();

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
