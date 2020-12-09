import { Component, OnInit, ViewChild } from '@angular/core'
import { Router } from '@angular/router'
import { NgxSpinnerService } from 'ngx-spinner'
import { UserService } from 'app/services/user/user.service';
import { MatDialog, MatPaginator, MatSort, MatTableDataSource } from '@angular/material'
import { DeleteDialogComponent } from '../../../components/common/delete-dialog/delete-dialog.component';
import { tap } from 'rxjs/operators';
import { AppConstant, DeleteAlertDataModel } from "../../../app.constants";
import { Notification, NotificationService, LookupService } from 'app/services';
import { empty } from 'rxjs';
import { FormBuilder, FormGroup, FormControl, Validators } from '@angular/forms';

@Component({
	selector: 'app-user-list',
	templateUrl: './user-list.component.html',
	styleUrls: ['./user-list.component.css']
})

export class UserListComponent implements OnInit {
	isFilterShow: boolean = false;
	checkSubmitStatus:boolean=false;
	changeStatusDeviceName: any;
	changeStatusDeviceStatus: any;
	changeDeviceStatus: any;
	deleteAlertDataModel: DeleteAlertDataModel;
	currentUser = JSON.parse(localStorage.getItem("currentUser"));
	userList = [];
	totalRecords = 0;
	pageSizeOptions: number[] = [5, 10, 25, 100];
	moduleName = "Users";
	displayedColumns: string[] = ['name', 'roleName', 'entityName', 'isActive', 'action'];
	isSearch = false;
	orderBy = 'name';
	searchParameters = {
		pageNumber: 0,
		pageSize: 10,
		searchText: '',
		sortBy: 'firstName asc',
		entityGuid: ''
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
		public lookupService:LookupService,
		private _notificationService: NotificationService,
	) { }

	ngOnInit() {
		this.getUserList();
	}

	/**
	 * called while the filter is applied
	 * @param filterValue 
	 */
	applyFilter(filterValue: string) {
		filterValue = filterValue.trim(); // Remove whitespace
		filterValue = filterValue.toLowerCase(); // Datasource defaults to lowercase matches
		this.dataSource.filter = filterValue;
	}

	/**
	 * navigate to add user page
	 */
	clickAdd() {
		this.router.navigate(['/users/add']);
	}

	/**
	 * short value of the table
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
	 * on page size change's
	 * @param pagechangeresponse 
	 */
	changePaginationAsPageChange(pagechangeresponse) {
		this.searchParameters.pageNumber = pagechangeresponse.pageIndex;
		this.searchParameters.pageSize = pagechangeresponse.pageSize;
		this.isSearch = true;
		this.getUserList();
	}

	/**
	 * search filter
	 * @param filterText 
	 */
	searchTextCallback(filterText) {
		this.searchParameters.searchText = filterText;
		this.searchParameters.pageNumber = 0;
		this.getUserList();
		this.isSearch = true;
	}

	/**
	 * get the list of user
	 */
	getUserList() {
    this.spinner.show();
    let parameter = {
      'pageNo': this.searchParameters.pageNumber + 1,
      'pageSize': this.searchParameters.pageSize,
      'searchText': this.searchParameters.searchText,
      'orderBy': this.searchParameters.sortBy
    }; 
    this.userService.getUserlist(parameter).subscribe(response => {
			this.spinner.hide();
			if (response.data.items) {
				this.totalRecords = response.data.count;
				// this.isSearch = false;
				this.userList = response.data.items;
			}
			else {
				this._notificationService.add(new Notification('error', response.message));
				this.userList = [];
			}
		}, error => {
			this.spinner.hide();
			this._notificationService.add(new Notification('error', error));
		});
	}

	/**
	 * NOT IN USE
	 * @param filterValue 
	 */
	onKey(filterValue: string) {
		this.applyFilter(filterValue);
	}

	/**
	 * called when user tries to delete user
	 * @param userModel 
	 */
	deleteModel(userModel: any) {
		this.deleteAlertDataModel = {
			title: "Delete User",
			message: this._appConstant.msgConfirm.replace('modulename', "User"),
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
				this.deleteUser(userModel.guid);
			}
		});
	}

	/**
	 * delete user from db
	 * @param guid 
	 */
	deleteUser(guid) {
		this.spinner.show();
		this.userService.deleteUser(guid).subscribe(response => {
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
	 * to active inactive user
	 * @param id 
	 * @param isActive 
	 * @param fname 
	 * @param lname 
	 */
	activeInactiveUser(id: string, isActive: boolean, fname: string, lname: string) {
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
				this.changeUserStatus(id, isActive);

			}
		});

	}

	/**
	 * to change the user status
	 * @param id 
	 * @param isActive 
	 */
	changeUserStatus(id, isActive) {

		this.spinner.show();
		this.userService.changeStatus(id, isActive).subscribe(response => {
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

	/**
	 * Show hide filter
	 * */
	showHideFilter() {
		this.isFilterShow = !this.isFilterShow;
	}

}
