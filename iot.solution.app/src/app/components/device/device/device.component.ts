import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router'
import { NgxSpinnerService } from 'ngx-spinner'
import { MatDialog } from '@angular/material'
import { DeleteDialogComponent } from '../../../components/common/delete-dialog/delete-dialog.component';
import {  DeviceService } from './../../../services/index'
import { Notification, NotificationService } from 'app/services';
import { AppConstant, DeleteAlertDataModel } from "../../../app.constants";

@Component({
  selector: 'app-device',
  templateUrl: './device.component.html',
  styleUrls: ['./device.component.css']
})
export class DeviceComponent implements OnInit {

  changeStatusDeviceName:any;
	changeStatusDeviceStatus:any;
	moduleName = "Devices";

	isSearch = false;
	totalRecords = 0;
	pageSizeOptions: number[] = [5, 10, 25, 100];
	
	searchParameters = {
		pageNumber: 0,
		pageSize: 10,
		searchText: '',
		sortBy: 'name asc'
	};
	displayedColumns: string[] = ['name', 'uniqueId', 'isActive', 'action'];
	deviceList = [];
	deleteAlertDataModel : DeleteAlertDataModel ; 


	constructor(
		private spinner: NgxSpinnerService,
		private router: Router,
		public dialog: MatDialog,
    public deviceService:DeviceService,
		private _notificationService: NotificationService,
		public _appConstant : AppConstant
	) { }

	ngOnInit() {
		this.getDeviceList();
	}


  /**
   * go to add device page
   */
	clickAdd() {
		this.router.navigate(['/devices/add']);
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
		this.getDeviceList();
	}

  /**
   * to open confironmation popUp to delete the device
   * @param userModel 
   */
	deleteModel(userModel: any) {
		this.deleteAlertDataModel = {
			title: "Delete Device" ,
			message: this._appConstant.msgConfirm.replace('modulename', "Device"), 
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
				this.deleteDevice(userModel.guid);
			}
		});
	}


  /**
   * to open the confiromation popUp for active/inactive status
   * @param deviceId 
   * @param isActive 
   * @param name 
   */
	activeInactiveDevice(deviceId: string, isActive: boolean, name:string) {
		var status = isActive == false ? this._appConstant.activeStatus : this._appConstant.inactiveStatus;
		var mapObj = {
			statusname:status,
			fieldname:name,
			modulename:"device"
		 };
		this.deleteAlertDataModel = {
			title: "Device Status",
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
				this.changeDeviceStatus(deviceId, isActive);

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
		this.getDeviceList();
	}

  /**
   * to filter by search
   * @param filterText 
   */
	searchTextCallback(filterText) {
		this.searchParameters.searchText = filterText;
		this.searchParameters.pageNumber = 0;
		this.isSearch = true;
		this.getDeviceList();
	}

  /**
   * to get the device list/filter/short
   */
	getDeviceList() {
		this.spinner.show();
		this.deviceService.getDeviceList(this.searchParameters).subscribe(response => {
			this.spinner.hide();
		
			if (response.isSuccess === true) {
				this.totalRecords = response.data.count;
				// this.isSearch = false;
				this.deviceList = response.data.items;
			}
			else {
        this.totalRecords=0;
				this._notificationService.add(new Notification('error', response.message));
				this.deviceList = [];
			}
		}, error => {
      this.totalRecords=0;
      this.deviceList = [];
			this.spinner.hide();
			this._notificationService.add(new Notification('error', error ));
		});
	}

  /**
   * to change the device status from active/inactive
   * @param deviceId
   * @param isActive 
   */
  changeDeviceStatus(deviceId, isActive) {
    let data = {
      'deviceId': deviceId,
      'isActive': isActive }
		this.spinner.show();
    this.deviceService.changeStatus(data).subscribe(response => {
			this.spinner.hide();
			if (response.isSuccess === true) {
				this._notificationService.add(new Notification('success', this._appConstant.msgStatusChange.replace("modulename", "Device")));
				this.getDeviceList();

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
   * to delete the device
   * @param deviceId 
   */
	deleteDevice(deviceId) {
		this.spinner.show();
		this.deviceService.deleteDevice(deviceId).subscribe(response => {
			this.spinner.hide();
			if (response.isSuccess === true) {
				this._notificationService.add(new Notification('success', this._appConstant.msgDeleted.replace("modulename", "Device")));
				this.getDeviceList();

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
