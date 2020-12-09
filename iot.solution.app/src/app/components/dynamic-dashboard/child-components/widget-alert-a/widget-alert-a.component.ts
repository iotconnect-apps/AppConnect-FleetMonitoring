import { ChangeDetectorRef, ViewRef , OnInit, Component, Input, ViewEncapsulation, EventEmitter } from '@angular/core';
import * as moment from 'moment-timezone'
import { NgxSpinnerService } from 'ngx-spinner'
import { Notification, NotificationService, AlertsService } from 'app/services';
import {Subscription} from 'rxjs/Subscription';

@Component({
	selector: 'app-widget-alert-a',
	templateUrl: './widget-alert-a.component.html',
	styleUrls: ['./widget-alert-a.component.css']
})
export class WidgetAlertAComponent implements OnInit {
	@Input() widget;
	@Input() resizeEvent: EventEmitter<any>;
	@Input() alertLimitchangeEvent: EventEmitter<any>;
	resizeSub: Subscription;
	limitChangeSub: Subscription;
	alerts: any = [];

	constructor(
		public alertsService: AlertsService,
		private spinner: NgxSpinnerService,
		private _notificationService: NotificationService,
		private changeDetector : ChangeDetectorRef,
	){

	}

	public height:any;

	ngOnInit() {
		this.height=this.widget.properties.h-54;
		
		this.resizeSub = this.resizeEvent.subscribe((widget) => {
			if(widget.componentName==='widget-alert-a'){
				this.height=widget.properties.h-54;
			}
		});
		this.limitChangeSub = this.alertLimitchangeEvent.subscribe((limit) => {
			this.getAlertList();
		});
		this.getAlertList();
	}

	getAlertList() {
		let searchParameters = {
			pageNo: 0,
			pageSize: (this.widget.widgetProperty.alertLimit > 0 ? this.widget.widgetProperty.alertLimit : 10),
			searchText: '',
			orderBy: 'eventDate desc',
			deviceGuid: '',
			entityGuid: '',
		};
		this.spinner.show();
		this.alertsService.getAlerts(searchParameters).subscribe(response => {
			this.spinner.hide();
			if (response.isSuccess === true && response.data.items) {
				this.alerts = response.data.items;
			}
			else {
				this.alerts = [];
			}
			if (this.changeDetector && !(this.changeDetector as ViewRef).destroyed) {
				this.changeDetector.detectChanges();
			}
		}, error => {
			this.spinner.hide();
			this.alerts = [];
			this._notificationService.add(new Notification('error', error));

		});
	}

	getLocalDate(lDate) {
		var utcDate = moment.utc(lDate, 'YYYY-MM-DDTHH:mm:ss.SSS');
		var localDate = moment(utcDate).local();
		let res = moment(localDate).format('MMM DD, YYYY hh:mm:ss A');
		return res;
  	}

	ngOnDestroy() {
		this.resizeSub.unsubscribe();
		this.limitChangeSub.unsubscribe();
	}
}
