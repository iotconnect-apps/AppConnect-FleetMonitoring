import { ChangeDetectorRef, ViewRef, OnInit, Component, Input, Output, EventEmitter, ViewChild, OnDestroy } from '@angular/core';
import { NgxSpinnerService } from 'ngx-spinner'
import { DeviceService } from 'app/services/device/device.service';
import { Notification, NotificationService, DashboardService } from 'app/services';
import { Subscription } from 'rxjs/Subscription';
import { ChartReadyEvent, GoogleChartComponent } from 'ng2-google-charts'

@Component({
	selector: 'app-widget-chart-c',
	templateUrl: './widget-chart-c.component.html',
	styleUrls: ['./widget-chart-c.component.css']
})
export class WidgetChartCComponent implements OnInit, OnDestroy {
	graphChartData = {
		chartType: 'LineChart',
		options: {
		  legend: 'none',
		  curveType: 'function',
		  pointSize: 20,
		  height: 400,
		  interpolateNulls: true,
		  hAxis: {
			title: '',
			gridlines: {
			  count: 5
			},
		  },
		  vAxis: {
			title: 'Values',
			gridlines: {
			  count: 5
			},
		  }
		},
		dataTable: [],
	
	  }
	graphdata: any = [];
	@Input() widget;
	@Input() gridster;
	@Input() count;
	@Input() resizeEvent: EventEmitter<any>;
	resizeSub: Subscription;
	@Input() chartTypeChangeEvent: EventEmitter<any>;
	chartTypeChangeSub: Subscription;


	@ViewChild('cchart', { static: false }) cchart: GoogleChartComponent;
	currentUser = JSON.parse(localStorage.getItem("currentUser"));
	columnArray: any = [];
	headFormate: any = {
		columns: this.columnArray,
		type: 'NumberFormat'
	};
	bgColor = ['#fff'];
	highenergy: any;
	generaytorBatteryStatus = {
		chartType: 'ColumnChart',
		dataTable: [],
		options: {
			width: 200,
			height: 200,
			interpolateNulls: true,
			legend: { position: 'none' },
			backgroundColor: this.bgColor,
			bar: { groupWidth: "25%" },
			colors: ["#5496d0"],
			hAxis: {
				title: 'Generator',
				gridlines: {
					count: 5
				}
			},
			vAxis: {
				title: '% Percentage',
				gridlines: {
					count: 1
				},
			}
		},
		formatters: this.headFormate
	};
	greenhouse = [];
	constructor(
		public deviceService: DeviceService,
		private dashboardService: DashboardService,
		private spinner: NgxSpinnerService,
		private _notificationService: NotificationService,
		private changeDetector: ChangeDetectorRef,
	) {
	}

	ngOnInit() {
		if (this.widget.widgetProperty.chartColor.length > 0) {
			this.generaytorBatteryStatus.options.colors = [];
			for (var i = 0; i <= (this.widget.widgetProperty.chartColor.length - 1); i++) {
				this.generaytorBatteryStatus.options.colors.push(this.widget.widgetProperty.chartColor[i].color);
			}
		}
		this.generaytorBatteryStatus.options.width = (this.widget.properties.w > 0 ? parseInt((this.widget.properties.w - 40).toString()) : 200);
		this.graphChartData.options.height = (this.widget.properties.h > 0 ? parseInt((this.widget.properties.h - 100).toString()) : 200);
		this.resizeSub = this.resizeEvent.subscribe((widget) => {
			if (widget.id == this.widget.id) {
				this.widget = widget;
				this.changeChartType();
			}
		});

		this.chartTypeChangeSub = this.chartTypeChangeEvent.subscribe((widget) => {
			if (widget.id == this.widget.id) {
				this.changeChartType();
			}
		});
		this.getenergyusageGraph()
	}

	 /**
	 * Get color
	 * */
	getcolor(colorname) {
		if (colorname == 'red') {
		  return 'warn';
		} else {
		  return 'primary';
		}
	  }
/**
	 * Get getenergyusageGraph
	 * */
	getenergyusageGraph() {
		this.spinner.show();
		var data = {
		  "companyguid": this.currentUser.userDetail.companyId
		}
		this.dashboardService.getenergyusageGraph(data).subscribe(response => {
		  this.spinner.hide();
		  if (response.isSuccess === true) {
			let data = [];
			data.push(["", ""])
			response.data.forEach(element => {
			  data.push([element.name, parseFloat(element.energyConsumption)])
			});
			this.graphChartData = {
			  chartType: 'LineChart',
			  options: {
				legend: 'none',
				curveType: 'function',
				pointSize: 10,
				height: 400,
				interpolateNulls: true,
				hAxis: {
				  title: '',
				  gridlines: {
					count: 5
				  },
				},
				vAxis: {
				  title: 'Fuel (Gallons)',
				  gridlines: {
					count: 5
				  },
				}
			  },
			  dataTable: data,
			};
		  }
		}, error => {
		  this.spinner.hide();
		  this._notificationService.add(new Notification('error', error));
		});
	
	  }
	

	
	changeChartType() {
		if (this.widget.widgetProperty.chartColor.length > 0) {
			this.generaytorBatteryStatus.options.colors = [];
			for (var i = 0; i <= (this.widget.widgetProperty.chartColor.length - 1); i++) {
				this.generaytorBatteryStatus.options.colors.push(this.widget.widgetProperty.chartColor[i].color);
			}
		}
		this.generaytorBatteryStatus.options.width = (this.widget.properties.w > 0 ? parseInt((this.widget.properties.w - 40).toString()) : 200);
		this.graphChartData.options.height = (this.widget.properties.h > 0 ? parseInt((this.widget.properties.h - 100).toString()) : 200);
		this.generaytorBatteryStatus.chartType = 'ColumnChart';
		if (this.widget.widgetProperty.chartType && this.widget.widgetProperty.chartType != '') {
			this.generaytorBatteryStatus.chartType = (this.widget.widgetProperty.chartType == 'bar' ? 'ColumnChart' : 'LineChart');
			if (this.generaytorBatteryStatus.dataTable.length > 1 && this.cchart) {
				let ccWrapper = this.cchart.wrapper;
				ccWrapper.setChartType(this.generaytorBatteryStatus.chartType);
				this.cchart.draw();
				ccWrapper.draw();
			}
			if (this.changeDetector && !(this.changeDetector as ViewRef).destroyed) {
				this.changeDetector.detectChanges();
			}
		}
	}

	ngOnDestroy() {
		this.resizeSub.unsubscribe();
		this.chartTypeChangeSub.unsubscribe();
	}
}
