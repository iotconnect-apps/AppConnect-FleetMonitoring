import { Component, OnInit, ViewChild } from '@angular/core'
import { Router } from '@angular/router'
import { NgxSpinnerService } from 'ngx-spinner'
import { MatDialog, MatTableDataSource, MatSort, MatPaginator } from '@angular/material'
import { DeleteDialogComponent } from '../../common/delete-dialog/delete-dialog.component';
import { DeviceService, NotificationService } from 'app/services';
import{Notification} from 'app/services/notification/notification.service';
import { AppConstant, DeleteAlertDataModel } from "../../../app.constants";

@Component({ selector: 'app-subscribers-list', templateUrl: './subscribers-list.component.html', styleUrls: ['./subscribers-list.component.scss'] })

export class SubscribersListComponent implements OnInit {
	changeStatusDeviceName:any;
	changeStatusDeviceStatus:any;
	order = true;
	isSearch = false;
	pageSizeOptions: number[] = [5, 10, 25, 100];
	reverse = false;
	orderBy = 'companyName';
	totalRecords=0;
	searchParameters = {
		pageNo:0,
		pageSize: 10,
		searchText: '',
		sortBy: 'companyName asc'
	};
	displayedColumns: string[] = [ 'subscriberName','companyName', 'email','subscriptionStartDate','subscriptionEndDate','planName'];
	dataSource=[];
	deleteAlertDataModel : DeleteAlertDataModel ; 
	
	constructor(
		private spinner: NgxSpinnerService,
		private router: Router,
		public dialog: MatDialog,
		private deviceService:DeviceService,
		private _notificationService: NotificationService,
		public _appConstant : AppConstant
	) { }

	ngOnInit() {
		this.getSubscribersList();
	}

	clickAdd() {
		this.router.navigate(['/device/add']);
	}

	setOrder(sort: any) {
		if (!sort.active || sort.direction === '') {
			return;
	   }
	  	this.searchParameters.sortBy = sort.active + ' ' + sort.direction;
		this.getSubscribersList();
	}

  onPageSizeChangeCallback(pageSize) {
    this.searchParameters.pageSize = pageSize;
    this.searchParameters.pageNo = 1;
    this.isSearch = true;
    this.getSubscribersList();
  }

	ChangePaginationAsPageChange(pagechangeresponse) {
	this.searchParameters.pageNo = pagechangeresponse.pageIndex;
	this.searchParameters.pageSize = pagechangeresponse.pageSize;
	this.isSearch = true;
    this.getSubscribersList();
	}

	searchTextCallback(filterText) {
		this.searchParameters.searchText = filterText;
		this.searchParameters.pageNo = 0;
		this.getSubscribersList();
		this.isSearch = true;
	}


	
	getSubscribersList() {
		this.spinner.show();
		this.deviceService.getsubscribers(this.searchParameters).subscribe(response => {
			this.spinner.hide();
			if (response.isSuccess === true) {
				this.totalRecords = response.data.count;
				this.dataSource = response.data.items;
			}
			else {
				this._notificationService.add(new Notification('error', response.message));
				this.dataSource = [];
			}
		}, error => {
			this.spinner.hide();
			this._notificationService.add(new Notification('error', error ));
		});
	}

}
