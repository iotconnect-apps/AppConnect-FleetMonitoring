import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router'
import { NgxSpinnerService } from 'ngx-spinner'
import { MatDialog } from '@angular/material'
import { DeleteDialogComponent } from '../../../components/common/delete-dialog/delete-dialog.component';
import { RolesService } from './../../../services/index'
import { Notification, NotificationService } from 'app/services';
import { AppConstant, DeleteAlertDataModel } from "../../../app.constants";


@Component({
	selector: 'app-roles-list',
	templateUrl: './roles-list.component.html',
	styleUrls: ['./roles-list.component.css']
})
export class RolesListComponent implements OnInit {
	changeStatusDeviceName:any;
	changeStatusDeviceStatus:any;
	changeDeviceStatus:any;
	moduleName = "Roles";
	order = true;
	isSearch = false;
	totalRecords = 5;
	pageSizeOptions: number[] = [5, 10, 25, 100];
	reverse = false;
	searchParameters = {
		pageNumber: 0,
		pageSize: 10,
		searchText: '',
		sortBy: 'name asc'
	};
	displayedColumns: string[] = ['name', 'description', 'isActive', 'action'];
	rolesList = [];
	deleteAlertDataModel : DeleteAlertDataModel ; 


	constructor(
		private spinner: NgxSpinnerService,
		private router: Router,
		public dialog: MatDialog,
		public rolesService : RolesService,
		private _notificationService: NotificationService,
		public _appConstant : AppConstant
	) { }

	ngOnInit() {
		this.getRolesList();
	}

	clickAdd() {
		this.router.navigate(['/roles/add']);
	}

	setOrder(sort: any) {
		if (!sort.active || sort.direction === '') {
			return;
	   }
	  	this.searchParameters.sortBy = sort.active + ' ' + sort.direction;
		this.getRolesList();
	}

	deleteModel(userModel: any) {
		this.deleteAlertDataModel = {
			title: "Delete Role" ,
			message: this._appConstant.msgConfirm.replace('modulename', "Role"), 
      okButtonName: "Confirm",
      cancelButtonName: "Cancel" ,
		};
		const dialogRef = this.dialog.open(DeleteDialogComponent, {
			width: '400px',
			height: 'auto',
			data: this.deleteAlertDataModel,
			disableClose: false
		});
		dialogRef.afterClosed().subscribe(result => {
			if (result) {
				this.deleteRole(userModel.guid);
			}
		});
	}

	activeInactiveRole(roleId: string, isActive: boolean, name:string) {
		var status = isActive == false ? this._appConstant.activeStatus : this._appConstant.inactiveStatus;
		var mapObj = {
			statusname:status,
			fieldname:name,
			modulename:"role"
		 };
		this.deleteAlertDataModel = {
			title: "Status",
			message: this._appConstant.msgStatusConfirm.replace(/statusname|fieldname|modulename/gi, function(matched){
				return mapObj[matched];
			  }),
			okButtonName: "Confirm",
			cancelButtonName: "Cancel" ,
		};
		const dialogRef = this.dialog.open(DeleteDialogComponent, {
			width: '400px',
			height: 'auto',
			data: this.deleteAlertDataModel,
			disableClose: false
		});
		dialogRef.afterClosed().subscribe(result => {
			if (result) {
				this.changeRoleStatus(roleId, isActive);

			}
		});
	
	}

	ChangePaginationAsPageChange(pagechangeresponse) {
		this.searchParameters.pageSize = pagechangeresponse.pageSize;
		this.searchParameters.pageNumber = pagechangeresponse.pageIndex;
		this.isSearch = true;
		this.getRolesList();
	}

	searchTextCallback(filterText) {
		this.searchParameters.searchText = filterText;
		this.searchParameters.pageNumber = 0;
		this.isSearch = true;
		this.getRolesList();
	}

	getRolesList() {
    this.spinner.show();
    let parameter = {
      'pageNo': this.searchParameters.pageNumber + 1,
      'pageSize': this.searchParameters.pageSize,
      'searchText': this.searchParameters.searchText,
      'orderBy': this.searchParameters.sortBy,
    }; 
    this.rolesService.getRoles(parameter).subscribe(response => {
			this.spinner.hide();
		
			if (response.isSuccess === true) {
				this.totalRecords = response.data.count;
				// this.isSearch = false;
				this.rolesList = response.data.items;
			}
			else {
				this._notificationService.add(new Notification('error', response.message));
				this.rolesList = [];
			}
		}, error => {
			this.spinner.hide();
			this._notificationService.add(new Notification('error', error ));
		});
	}

	changeRoleStatus(roleId, isActive) {
	
		this.spinner.show();
		this.rolesService.changeStatus(roleId, isActive).subscribe(response => {
			this.spinner.hide();
			if (response.isSuccess === true) {
				this._notificationService.add(new Notification('success', this._appConstant.msgStatusChange.replace("modulename", "Role")));
				this.getRolesList();

			}
			else {
				this._notificationService.add(new Notification('error', response.message));
			}
			
		}, error => {
			this.spinner.hide();
			this._notificationService.add(new Notification('error', error));
		});
	}

	deleteRole(roleId) {
		this.spinner.show();
		this.rolesService.deleteRole(roleId).subscribe(response => {
			this.spinner.hide();
			if (response.isSuccess === true) {
				this._notificationService.add(new Notification('success', this._appConstant.msgDeleted.replace("modulename", "Role")));
				this.getRolesList();

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
