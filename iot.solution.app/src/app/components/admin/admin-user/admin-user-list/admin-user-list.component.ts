import { Component, OnInit, ViewChild } from '@angular/core'
import { Router } from '@angular/router'
import { NgxSpinnerService } from 'ngx-spinner'
import { UserService } from 'app/services/user/user.service';
import { MatDialog, MatPaginator, MatSort, MatTableDataSource } from '@angular/material'
import { DeleteDialogComponent } from '../../../../components/common/delete-dialog/delete-dialog.component';
import { tap } from 'rxjs/operators';
import { AppConstant, DeleteAlertDataModel } from "../../../../app.constants";
import { Notification, NotificationService } from 'app/services';
import { empty } from 'rxjs';

@Component({
  selector: 'app-adminuser-list',
  templateUrl: './admin-user-list.component.html',
  styleUrls: ['./admin-user-list.component.css']
})

export class UserAdminListComponent implements OnInit {
  changeStatusDeviceName: any;
  changeStatusDeviceStatus: any;
  changeDeviceStatus: any;
  deleteAlertDataModel: DeleteAlertDataModel;
  currentUser = JSON.parse(localStorage.getItem("currentUser"));
  userList = [];
  totalRecords = 0;
  pageSizeOptions: number[] = [5, 10, 25, 100];
  moduleName = "Users";
  displayedColumns: string[] = ['firstName', 'lastName', 'email', 'contactNo', 'isActive', 'action'];
  order = true;
  isSearch = false;
  reverse = false;
  orderBy = 'name';
  searchParameters = {
    pageNumber: 0,
    pageSize: 10,
    searchText: '',
    sortBy: 'firstName asc'
  };
  dataSource: MatTableDataSource<any>;
  @ViewChild('paginator', { static: false }) paginator: MatPaginator;
  @ViewChild(MatSort, { static: false }) sort: MatSort;

  constructor(
    public dialog: MatDialog,
    private spinner: NgxSpinnerService,
    private router: Router,
    private userService: UserService,
    public _appConstant: AppConstant,
    private _notificationService: NotificationService
  ) { }

  ngOnInit() {
    this.getUserList();

  }

  /**
   * Apply filter
   * @param filterValue
   */
  applyFilter(filterValue: string) {
    filterValue = filterValue.trim(); // Remove whitespace
    filterValue = filterValue.toLowerCase(); // Datasource defaults to lowercase matches
    this.dataSource.filter = filterValue;
  }

  /**
   * Routing to add user screen
   * */
  clickAdd() {
    this.router.navigate(['admin/users/adduser']);
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
    this.getUserList();
  }

  /**
   * Page size change event
   * @param pageSize
   */
  onPageSizeChangeCallback(pageSize) {
    this.searchParameters.pageSize = pageSize;
    this.searchParameters.pageNumber = 1;
    this.isSearch = true;
    this.getUserList();
  }

  /**
   * Pagination page change event
   * @param pagechangeresponse
   */
  ChangePaginationAsPageChange(pagechangeresponse) {
    this.searchParameters.pageNumber = pagechangeresponse.pageIndex;
    this.searchParameters.pageSize = pagechangeresponse.pageSize;
    this.isSearch = true;
    this.getUserList();
  }

  /**
   * Search for test in the list
   * @param filterText
   */
  searchTextCallback(filterText) {
    this.searchParameters.searchText = filterText;
    this.searchParameters.pageNumber = 0;
    this.getUserList();
    this.isSearch = true;
  }

  /**
   * Get user list
   * */
  getUserList() {
    this.spinner.show();
    this.userService.getAdminUserlist(this.searchParameters).subscribe(response => {
      this.spinner.hide();
      this.totalRecords = response.data.count;
      this.userList = response.data.items;

    }, error => {
      this.spinner.hide();
      this._notificationService.add(new Notification('error', error));
    });
  }

  /**
   * On key filter
   * @param filterValue
   */
  onKey(filterValue: string) {
    this.applyFilter(filterValue);
  }

  /**
   * Delete confirmation popup
   * @param userModel
   */
  deleteModel(userModel: any) {
    this.deleteAlertDataModel = {
      title: "Delete User",
      message: this._appConstant.msgConfirm.replace('modulename', "User"),
      okButtonName: "Yes",
      cancelButtonName: "No",
    };
    const dialogRef = this.dialog.open(DeleteDialogComponent, {
      width: '400px',
      height: 'auto',
      data: this.deleteAlertDataModel,
      disableClose: false
    });
    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.deleteuser(userModel.guid);
      }
    });
  }

  /**
   * Delete user by user guid
   * @param guid
   */
  deleteuser(guid) {
    this.spinner.show();
    this.userService.deleteadminUser(guid).subscribe(response => {
      this.spinner.hide();
      if (response.isSuccess === true) {
        this._notificationService.add(new Notification('success', this._appConstant.msgDeleted.replace("modulename", "User")));
        this.getUserList();

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
   * Change status confirmation popup
   * @param id
   * @param isActive
   * @param fname
   * @param lname
   */
  activeInactiveuser(id: string, isActive: boolean, fname: string, lname: string) {
    var status = isActive == false ? this._appConstant.activeStatus : this._appConstant.inactiveStatus;
    var mapObj = {
      statusname: status,
      fieldname: fname + " " + lname,
      modulename: ""
    };
    this.deleteAlertDataModel = {
      title: "Status",
      message: this._appConstant.msgStatusConfirm.replace(/statusname|fieldname|modulename/gi, function (matched) {
        return mapObj[matched];
      }),
      okButtonName: "Yes",
      cancelButtonName: "No",
    };
    const dialogRef = this.dialog.open(DeleteDialogComponent, {
      width: '400px',
      height: 'auto',
      data: this.deleteAlertDataModel,
      disableClose: false
    });
    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.changeUserStatus(id, isActive);

      }
    });

  }

  /**
   * Change status of user
   * @param id
   * @param isActive
   */
  changeUserStatus(id, isActive) {

    this.spinner.show();
    this.userService.adminchangeStatus(id, isActive).subscribe(response => {
      this.spinner.hide();
      if (response.isSuccess === true) {
        this._notificationService.add(new Notification('success', this._appConstant.msgStatusChange.replace("modulename", "User")));
        this.getUserList();

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
