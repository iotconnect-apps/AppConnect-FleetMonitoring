import { Component, OnInit, EventEmitter, ViewChild, Inject, OnDestroy } from '@angular/core';
import { ChangeDetectionStrategy, ChangeDetectorRef, ViewEncapsulation } from '@angular/core';
import { DisplayGrid, CompactType, GridsterConfig, GridsterItem, GridsterItemComponent, GridsterPush, GridType, GridsterComponentInterface, GridsterItemComponentInterface } from 'angular-gridster2';

import { DynamicDashboardService } from 'app/services/dynamic-dashboard/dynamic-dashboard.service';
import { DashboardService } from 'app/services/dashboard/dashboard.service';
import { Notification, NotificationService, DeviceService } from 'app/services';
import { NgxSpinnerService } from 'ngx-spinner'
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { DOCUMENT } from '@angular/common';
import { Options } from 'ng5-slider';

import { AppConstant, DeleteAlertDataModel } from "../../../app.constants";
import { MatDialog } from "@angular/material";
import { DeleteDialogComponent } from "../../../components/common/delete-dialog/delete-dialog.component";

import { forkJoin, of, Subscription } from 'rxjs';
import { map, catchError } from 'rxjs/operators';

@Component({
  selector: 'app-dynamic-dashboard',
  templateUrl: './dynamic-dashboard.component.html',
  styleUrls: ['./dynamic-dashboard.component.css'],
  changeDetection: ChangeDetectionStrategy.OnPush,
  encapsulation: ViewEncapsulation.None
})
export class DynamicDashboardComponent implements OnInit, OnDestroy {
  @ViewChild('gridster', { static: false }) gridster;
  /*Gridster code*/
  options: GridsterConfig;
  dashboardWidgets: Array<any> = [];
  dashboardWidgetsIndex = [];
  dashboardData = {
    id: '',
    index: 0,
    dashboardName: '',
    isDefault: false,
    widgets: []
  };
  defaultWidgets = [
  ];
  dashboardList = [];
  fontList = [];
  iconList = [];
  isPreview: boolean = false;

  deviceSize: any;
  /*Gridster code*/

  countData: any = {
    totalDevices: '-',
    totalAlerts: '-',
    totalUnderMaintenanceCount: '-',
    totalEnergyCount: '-',
    totalFuelUsed: '-',
    totalGenerators: '-',
    totalLocations: '-',
    minDeviceCount: '-',
    maxDeviceCount: '-',
    minDeviceName: '-',
    maxDeviceName: '-',
    totalUserCount: '-',
    totalConnected: '-',
    totalDisConnected: '-',
  }
  resizeEvent: EventEmitter<any> = new EventEmitter<any>();
  alertLimitchangeEvent: EventEmitter<any> = new EventEmitter<any>();
  chartTypeChangeEvent: EventEmitter<any> = new EventEmitter<any>();
  zoomChangeEvent: EventEmitter<any> = new EventEmitter<any>();
  telemetryDeviceChangeEvent: EventEmitter<any> = new EventEmitter<any>();
  telemetryAttributeChangeEvent: EventEmitter<any> = new EventEmitter<any>();
  draggedWidgetIndex: number = 0;
  draggedWidget: any;
  isEditItem: boolean = false;
  editWidgetIndex: number = 0;
  originalDashboardData: any;
  originalWidgetData: any;

  deviceData: any = [];
  deviceAttributesData: any = [];

  /*New Design Variables*/
  sliderValue: number = 100;
  sliderOptions: Options = {
    floor: 1,
    ceil: 100,
    animate: true
  };
  mapOptions: Options = {
    floor: 1,
    ceil: 20,
    animate: true
  };
  iconOptions: Options = {
    floor: 1,
    ceil: 10,
    animate: true
  };
  color: string = '#999';
  /*New Design Variables*/

  /*Delete Modal*/
  deleteAlertDataModel: DeleteAlertDataModel;
  /*Delete Modal*/
  currentUser = JSON.parse(localStorage.getItem('currentUser'));
  /*All Observables*/
  masterWidget = this.dynamicDashboardService.getMasterWidget();
  userDashboards = this.dynamicDashboardService.getUserWidget();
  dashbourdCount = this.dashboardService.getDashboardoverview();
  sideBarSubscription: Subscription;
  totalAlerts: any;
  totalFacilities: any;
  totalZones: any;
  totalIndoorZones: any;
  totalOutdoorZones: any;
  totalUserCount: any;
  activeUserCount: any;
  inactiveUserCount: any;
  totalConnected: any;
  totalDisconnected: any;
  totalDevices: any;
  totalEnergy: any;
  totalFleetCount: any;
  inTransitFleetCount: any;
  inGarageFleetCount: any;
  driverUtilizationPer: any;
  fleetUtilizationPer: any;
  totalFuelConsumption: any;
  /*All Observables*/

  constructor(
    private http: HttpClient,
    private spinner: NgxSpinnerService,
    public dashboardService: DashboardService,
    public dynamicDashboardService: DynamicDashboardService,
    private _notificationService: NotificationService,
    @Inject(DOCUMENT) private document: Document,
    private changeDetector: ChangeDetectorRef,
    public _appConstant: AppConstant,
    public dialog: MatDialog,
    private deviceService: DeviceService,
  ) {
    this.getMasterWidget();
    this.getFontList();
    this.getDashbourdCount();
    this.getDeviceList();
  }

  ngOnInit() {
    this.options = {
      gridType: GridType.Fixed,
      displayGrid: DisplayGrid.Always,
      initCallback: this.gridInit.bind(this),
      destroyCallback: this.gridDestroy.bind(this),
      gridSizeChangedCallback: this.gridSizeChanged.bind(this),
      itemChangeCallback: this.itemChange.bind(this),
      itemResizeCallback: this.itemResize.bind(this),
      itemInitCallback: this.itemInit.bind(this),
      itemRemovedCallback: this.itemRemoved.bind(this),
      itemValidateCallback: this.itemValidate.bind(this),
      fixedColWidth: 20,
      fixedRowHeight: 20,
      keepFixedHeightInMobile: false,
      keepFixedWidthInMobile: false,
      mobileBreakpoint: 100,
      pushItems: true,
      draggable: {
        enabled: true
      },
      resizable: {
        enabled: true
      },

      enableEmptyCellClick: false,
      enableEmptyCellContextMenu: false,
      enableEmptyCellDrop: true,
      enableEmptyCellDrag: false,
      enableOccupiedCellDrop: true,
      emptyCellDropCallback: this.emptyCellClick.bind(this),
      emptyCellDragCallback: this.emptyCellClick.bind(this),
      emptyCellDragMaxCols: 50,
      emptyCellDragMaxRows: 50,

      minCols: 60,
      maxCols: 192,
      minRows: 62,
      maxRows: 375,
      setGridSize: true,
      swap: true,
      swapWhileDragging: false,
      compactType: CompactType.None,

      margin: 0,
      outerMargin: true,
      outerMarginTop: null,
      outerMarginRight: null,
      outerMarginBottom: null,
      outerMarginLeft: null,
    };
    this.displayWidgets();
    this.sideBarSubscription = this.dynamicDashboardService.isToggleSidebarObs.subscribe((toggle) => {
      this.changedOptions();
      if (this.isPreview) {
        this.spinner.show();
        this.changedOptions();
        let cond = false;
        Observable.interval(700)
          .takeWhile(() => !cond)
          .subscribe(i => {
            cond = true;
            this.checkResponsiveness();
            this.spinner.hide();
          });
      }
    })
  }

  ngOnDestroy(): void {
    this.document.body.classList.remove('editdashboard');
    this.sideBarSubscription.unsubscribe();
  }

  checkResponsiveness() {
    let fixedColWidth = 20;
    if (this.gridster) {
      let tempWidth = parseFloat((((this.gridster.curWidth * fixedColWidth) / (fixedColWidth * this.gridster.columns)).toFixed(2)).toString());
      tempWidth = (tempWidth - 0.01);
      if (this.gridster.curWidth >= 640 && this.isPreview) {
        //tempWidth = Math.floor((this.gridster.curWidth / 60));
        this.options.fixedColWidth = tempWidth;
      }
      else {
        this.options.fixedColWidth = fixedColWidth;
      }
      for (var i = 0; i <= (this.dashboardWidgets.length - 1); i++) {
        if (this.gridster.curWidth < 640) {
          for (var g = 0; g <= (this.gridster.grid.length - 1); g++) {
            if (this.gridster.grid[g].item.id == this.dashboardWidgets[i].id) {
              this.dashboardWidgets[i].properties.w = this.gridster.grid[g].el.clientWidth;
            }
          }
        }
        else {
          this.dashboardWidgets[i].properties.w = (this.isPreview ? (tempWidth * this.dashboardWidgets[i].cols) : (fixedColWidth * this.dashboardWidgets[i].cols));
        }
        this.resizeEvent.emit(this.dashboardWidgets[i]);
      }
      this.changedOptions();
      this.changeDetector.detectChanges();
    }
  }

  displayWidgets() {
    if (this.isPreview) {
      this.document.body.classList.remove('editdashboard');
    }
    else {
      this.document.body.classList.add('editdashboard');
      this.document.body.classList.add('sidebar-close');
      this.document.body.classList.remove('sidebar-collapse');
    }
    this.checkResponsiveness();
  }

  deviceSizeChange(size) {
    this.deviceSize = size;
    let cellW = /*this.gridster.curColWidth*/20;
    let cellH = this.gridster.curRowHeight;
    this.dashboardList.forEach((dashboard, Dindex) => {
      dashboard.widgets.forEach((widget, wIndex) => {
        let minH = 0;
        let minW = 0;
        let maxH = 0;
        let maxW = 0;
        if (this.deviceSize.name == 'xs' || 'sm') {
          minH = (widget.properties['sm'].minHSm != '') ? parseFloat(widget.properties['sm'].minHSm) : 0;
          minW = (widget.properties['sm'].minWSm != '') ? parseFloat(widget.properties['sm'].minWSm) : 0;
          maxH = (widget.properties['sm'].maxHSm != '') ? parseFloat(widget.properties['sm'].maxHSm) : 0;
          maxW = (widget.properties['sm'].maxWSm != '') ? parseFloat(widget.properties['sm'].maxWSm) : 0;
        }
        else if (this.deviceSize.name == 'lg' || 'xl') {
          minH = (widget.properties['lg'].minHLg != '') ? parseFloat(widget.properties['lg'].minHLg) : 0;
          minW = (widget.properties['lg'].minWLg != '') ? parseFloat(widget.properties['lg'].minWLg) : 0;
          maxH = (widget.properties['lg'].maxHLg != '') ? parseFloat(widget.properties['lg'].maxHLg) : 0;
          maxW = (widget.properties['lg'].maxWLg != '') ? parseFloat(widget.properties['lg'].maxWLg) : 0;
        }
        else {
          minH = (widget.properties['md'].minHMd != '') ? parseFloat(widget.properties['md'].minHMd) : 0;
          minW = (widget.properties['md'].minWMd != '') ? parseFloat(widget.properties['md'].minWMd) : 0;
          maxH = (widget.properties['md'].maxHMd != '') ? parseFloat(widget.properties['md'].maxHMd) : 0;
          maxW = (widget.properties['md'].maxWMd != '') ? parseFloat(widget.properties['md'].maxWMd) : 0;
        }
        widget.minItemRows = (minH > cellH) ? Math.floor(minH / cellH) : 1;
        widget.maxItemRows = (maxH > cellH) ? Math.floor(maxH / cellH) : this.options.minRows;
        widget.minItemCols = (minW > cellW) ? Math.floor(minW / cellW) : 1;
        widget.maxItemCols = (maxW > cellW) ? Math.floor(maxW / cellW) : this.options.minCols;
        /*If property not set*/
        if (widget.componentName == 'widget-chart-b') {
          widget.widgetProperty.telemetryUniqueId = (widget.widgetProperty.telemetryUniqueId && widget.widgetProperty.telemetryUniqueId != '') ? widget.widgetProperty.telemetryUniqueId : '';
          widget.widgetProperty.telemetryAttributes = (widget.widgetProperty.telemetryAttributes && widget.widgetProperty.telemetryAttributes.length && widget.widgetProperty.telemetryAttributes.length > 0) ? widget.widgetProperty.telemetryAttributes : [];
        }
        /*If property not set*/
      });
    });
    this.defaultWidgets.forEach((widget, wIndex) => {
      let minH = 0;
      let minW = 0;
      let maxH = 0;
      let maxW = 0;
      if (this.deviceSize.name == 'xs' || 'sm') {
        minH = (widget.properties['sm'].minHSm != '') ? parseFloat(widget.properties['sm'].minHSm) : 0;
        minW = (widget.properties['sm'].minWSm != '') ? parseFloat(widget.properties['sm'].minWSm) : 0;
        maxH = (widget.properties['sm'].maxHSm != '') ? parseFloat(widget.properties['sm'].maxHSm) : 0;
        maxW = (widget.properties['sm'].maxWSm != '') ? parseFloat(widget.properties['sm'].maxWSm) : 0;
      }
      else if (this.deviceSize.name == 'lg' || 'xl') {
        minH = (widget.properties['lg'].minHLg != '') ? parseFloat(widget.properties['lg'].minHLg) : 0;
        minW = (widget.properties['lg'].minWLg != '') ? parseFloat(widget.properties['lg'].minWLg) : 0;
        maxH = (widget.properties['lg'].maxHLg != '') ? parseFloat(widget.properties['lg'].maxHLg) : 0;
        maxW = (widget.properties['lg'].maxWLg != '') ? parseFloat(widget.properties['lg'].maxWLg) : 0;
      }
      else {
        minH = (widget.properties['md'].minHMd != '') ? parseFloat(widget.properties['md'].minHMd) : 0;
        minW = (widget.properties['md'].minWMd != '') ? parseFloat(widget.properties['md'].minWMd) : 0;
        maxH = (widget.properties['md'].maxHMd != '') ? parseFloat(widget.properties['md'].maxHMd) : 0;
        maxW = (widget.properties['md'].maxWMd != '') ? parseFloat(widget.properties['md'].maxWMd) : 0;
      }
      widget.minItemRows = (minH > cellH) ? Math.floor(minH / cellH) : 1;
      widget.maxItemRows = (maxH > cellH) ? Math.floor(maxH / cellH) : this.options.minRows;
      widget.minItemCols = (minW > cellW) ? Math.floor(minW / cellW) : 1;
      widget.maxItemCols = (maxW > cellW) ? Math.floor(maxW / cellW) : this.options.minCols;
      widget.rows = widget.minItemRows;
      widget.cols = widget.minItemCols;
    });
    this.checkResponsiveness();
    /*set minHeight and minWidth for all widgets*/
  }

  getMasterWidget() {
    this.spinner.show();
    this.defaultWidgets = [];
    this.dynamicDashboardService.getMasterWidget().subscribe(response => {
      if (response.isSuccess === true) {
        for (var i = 0; i <= (response.data.length - 1); i++) {
          response.data[i].id = response.data[i].guid;
          let widget = JSON.parse(response.data[i].widgets);
          widget.id = response.data[i].id;
          this.defaultWidgets.push(widget);
        }
      }
      this.deviceSizeChange(this.deviceSize);
    }, error => {
      this.spinner.hide();
      this._notificationService.handleResponse(error, "error");
    });
  }

  getWidgetType(type = 'counter') {
    if (type == 'counter')
      return 'Counter Widget';
    else if (type == 'alert')
      return 'Alert widget';
    else if (type == 'chart')
      return 'Chart Widget';
    else if (type == 'map')
      return 'Map View Widget';
    else
      return 'User case specific widget';
  }

  getDashboards() {
    this.spinner.show();
    this.dashboardList = [];
    this.dashboardData.index = 0;
    let isAnyDefault = false;
    let systemDefaultIndex = 0;
    this.dynamicDashboardService.getUserWidget().subscribe(response => {
      this.spinner.hide();
      if (response.isSuccess === true) {
        for (var i = 0; i <= (response.data.length - 1); i++) {
          response.data[i].id = response.data[i].guid;
          response.data[i].widgets = JSON.parse(response.data[i].widgets);
          this.dashboardList.push(response.data[i]);
          if (response.data[i].isDefault === true) {
            isAnyDefault = true;
            this.dashboardData.index = i;
          }
          if (response.data[i].isSystemDefault === true) {
            systemDefaultIndex = i;
          }
        }
      }
      this.deviceSizeChange(this.deviceSize);
      /*Display Default Dashboard if no data*/
      if (!isAnyDefault) {
        this.dashboardList[systemDefaultIndex].isDefault = true;
      }
      /*Display Default Dashboard if no data*/
      if (this.dashboardList.length > 0)
        this.editDashboard('view', 'n');
      else
        this.editDashboard('', 'n');
    }, error => {
      this.spinner.hide();
      this._notificationService.handleResponse(error, "error");
    });
  }

  getFontList() {
    this.fontList = this.dynamicDashboardService.fontList;
  }

  getIconName(item) {
    item = item.replace('fas fa-', '');
    item = item.replace('fab fa-', '');
    return item;
  }

  getDashbourdCount() {
    this.spinner.show();
    this.dashboardService.getDashboardoverview().subscribe(response => {
      if (response.isSuccess === true) {
        this.totalConnected = response.data.totalConnected;
        this.totalDisconnected = response.data.totalDisconnected;
        this.totalDevices = response.data.totalDevices;
        this.totalEnergy = response.data.totalEnergy;
        this.totalFacilities = response.data.totalEntities;
        this.totalFleetCount = (response.data.totalFleetCount) ? response.data.totalFleetCount : 0
        this.inTransitFleetCount = (response.data.inTransitFleetCount) ? response.data.inTransitFleetCount : 0
        this.inGarageFleetCount = (response.data.inGarageFleetCount) ? response.data.inGarageFleetCount : 0
        this.spinner.hide();
        this.driverUtilizationPer = (response.data.driverUtilizationPer) ? response.data.driverUtilizationPer : 0
        this.fleetUtilizationPer = (response.data.fleetUtilizationPer) ? response.data.fleetUtilizationPer : 0
        this.totalFuelConsumption = (response.data.totalFuelConsumption) ? response.data.totalFuelConsumption : 0
        this.totalFuelConsumption = this.dashboardService.transform(this.totalFuelConsumption);
        this.totalAlerts = (response.data.totalAlerts) ? response.data.totalAlerts : 0
        this.totalUserCount = (response.data.totalUserCount) ? response.data.totalUserCount : 0
        this.activeUserCount = (response.data.activeUserCount) ? response.data.activeUserCount : 0
        this.inactiveUserCount = (response.data.inactiveUserCount) ? response.data.inactiveUserCount : 0
      } else {
        this._notificationService.handleResponse(response, "error");
      }
      this.changeDetector.detectChanges();
    }, error => {
      this.spinner.hide();
      this._notificationService.handleResponse(error, "error");
    });
  }

  loadStyle(index: number = 0) {
    let family = this.fontList[index].name.trim();
    family = family.replace(/ /g, '+');
    const head = this.document.getElementsByTagName('head')[0];
    const style = this.document.createElement('link');
    style.id = 'client-theme' + index;
    style.rel = 'stylesheet';
    style.href = 'https://fonts.googleapis.com/css?family=' + family;
    head.appendChild(style);
  }

  /*Gridster code*/
  itemChange(item: any, itemComponent: GridsterItemComponentInterface) {
    let iIndex = this.dashboardWidgetsIndex.indexOf(item.id);
    this.dashboardWidgets[iIndex].cols = itemComponent.$item.cols;
    this.dashboardWidgets[iIndex].rows = itemComponent.$item.rows;
    this.dashboardWidgets[iIndex].x = itemComponent.$item.x;
    this.dashboardWidgets[iIndex].y = itemComponent.$item.y;
    this.dashboardWidgets[iIndex].properties.w = itemComponent.width;
    this.dashboardWidgets[iIndex].properties.h = itemComponent.height;
  }

  itemResize(item: any, itemComponent: GridsterItemComponentInterface) {
    let iIndex = this.dashboardWidgetsIndex.indexOf(item.id);
    if (this.dashboardWidgets[iIndex]) {
      this.dashboardWidgets[iIndex].cols = itemComponent.$item.cols;
      this.dashboardWidgets[iIndex].rows = itemComponent.$item.rows;
      this.dashboardWidgets[iIndex].x = itemComponent.$item.x;
      this.dashboardWidgets[iIndex].y = itemComponent.$item.y;
      this.dashboardWidgets[iIndex].properties.w = itemComponent.width;
      this.dashboardWidgets[iIndex].properties.h = itemComponent.height;
      this.resizeEvent.emit(item);
    }
  }

  itemInit(item: any, itemComponent: GridsterItemComponentInterface) {
    //console.info('itemInitialized', item, itemComponent);
    if (item && item.componentName && item.componentName == 'widget-chart-b') {
      let iIndex = this.dashboardWidgetsIndex.indexOf(item.id);
      this.deviceChange(iIndex);
    }
  }

  itemRemoved(item: any, itemComponent: GridsterItemComponentInterface) {
  }

  itemValidate(item: any) {
    return item.cols > 0 && item.rows > 0;
  }

  gridInit(grid: GridsterComponentInterface) {
    this.changedOptions();
    this.displayWidgets();
  }

  gridDestroy(grid: GridsterComponentInterface) {
  }

  gridSizeChanged(grid: GridsterComponentInterface) {
  }

  changedOptions() {
    if (this.options.api && this.options.api.optionsChanged) {
      this.options.api.optionsChanged();
    }
  }

  removeItem($event, item, index: number = 0) {
    this.deleteAlertDataModel = {
      title: "Delete Widget",
      message: this._appConstant.msgConfirm.replace('modulename', "Widget"),
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
        if (index == this.editWidgetIndex) {
          this.isEditItem = false;
          this.editWidgetIndex = null;
          this.originalWidgetData = null;
        }
        $event.preventDefault();
        $event.stopPropagation();
        this.dashboardWidgets.splice(this.dashboardWidgetsIndex.indexOf(item.id), 1);
        this.dashboardWidgetsIndex.splice(this.dashboardWidgetsIndex.indexOf(item.id), 1);
        this.changeDetector.detectChanges();
      }
    });
  }

  changeChartType(index: number = 0) {
    let widget = this.dashboardWidgets[index];
    this.chartTypeChangeEvent.emit(widget);
    this.changeDetector.detectChanges();
  }

  zoomChanged(index: number = 0) {
    let widget = this.dashboardWidgets[index];
    this.zoomChangeEvent.emit(widget);
    this.changeDetector.detectChanges();
  }

  dragStopHandler(ev, item: any, index: number = 0) {
  }

  dragStartHandler(ev, item: any, index: number = 0) {
    let newItem = JSON.parse(JSON.stringify(item));
    this.draggedWidgetIndex = index;
    this.draggedWidget = newItem;
  }

  emptyCellClick(event: MouseEvent, item: any) {
    if (this.draggedWidget && this.draggedWidget.id) {
      this.draggedWidget.x = item.x;
      this.draggedWidget.y = item.y;
      this.dashboardWidgets.push(this.draggedWidget);
      this.dashboardWidgetsIndex.push(this.draggedWidget.id);
      this.draggedWidgetIndex = -1;
      this.draggedWidget = null;
    }
  }

  editProperties(ev, item, index: number = 0) {
    if (this.isEditItem && this.originalWidgetData) {
      let x = this.dashboardWidgets[this.editWidgetIndex].x;
      let y = this.dashboardWidgets[this.editWidgetIndex].y;
      let cols = this.dashboardWidgets[this.editWidgetIndex].cols;
      let rows = this.dashboardWidgets[this.editWidgetIndex].rows;
      this.dashboardWidgets[this.editWidgetIndex] = this.originalWidgetData;
      this.dashboardWidgets[this.editWidgetIndex].x = x;
      this.dashboardWidgets[this.editWidgetIndex].y = y;
      this.dashboardWidgets[this.editWidgetIndex].cols = cols;
      this.dashboardWidgets[this.editWidgetIndex].rows = rows;
    }
    this.isEditItem = true;
    this.editWidgetIndex = index;
    this.originalWidgetData = JSON.parse(JSON.stringify(this.dashboardWidgets[this.editWidgetIndex]));
    this.changeDetector.detectChanges();
  }

  editWidget(e, editWidgetForm) {
    this.isEditItem = false;
    this.editWidgetIndex = null;
    this.originalWidgetData = null;
    this._notificationService.handleResponse({ message: "Widget edited successfully." }, "success");
  }

  cancelEditWidget(is_save: string = 'n') {
    if (is_save == 'n' && this.originalWidgetData) {
      this.dashboardWidgets[this.editWidgetIndex].widgetProperty = this.originalWidgetData.widgetProperty;
      this.chartTypeChangeEvent.emit(this.dashboardWidgets[this.editWidgetIndex]);
      if (this.dashboardWidgets[this.editWidgetIndex].componentName == 'widget-chart-b') {
        this.deviceChange(this.editWidgetIndex, 'n');
      }
      this.telemetryDeviceChangeEvent.emit(this.dashboardWidgets[this.editWidgetIndex]);
      this.changeDetector.detectChanges();
    }
    /*attributes check for telemetry device*/
    let isProcess = true;
		/*if(is_save == 'y'){
			if(this.dashboardWidgets[this.editWidgetIndex].componentName == 'widget-chart-b'){
				if(this.dashboardWidgets[this.editWidgetIndex].widgetProperty.telemetryUniqueId != ''){
					if(this.dashboardWidgets[this.editWidgetIndex].widgetProperty.telemetryAttributes.length == 0){
						this._notificationService.handleResponse({message:"You must select atleast one attribute to save widget."},"error");
						isProcess = false;
					}
				}
				else{
					this._notificationService.handleResponse({message:"You must select atleast one generator to save widget."},"error");
					isProcess = false;
				}
			}
		}*/
    /*attributes check for telemetry device*/
    if (isProcess) {
      this.isEditItem = false;
      this.editWidgetIndex = null;
      this.originalWidgetData = null;
    }
    this.changeDetector.detectChanges();
  }

  saveDashboard() {
    if (this.dashboardData.dashboardName.trim() != '' && this.dashboardWidgets.length > 0) {
      let isOk = true;
      if (isOk) {
        this.spinner.show();
        this.dashboardData.widgets = this.dashboardWidgets;
        let currentUser = JSON.parse(localStorage.getItem('currentUser'));
        let body = {
          guid: this.dashboardData.id,
          dashboardName: this.dashboardData.dashboardName,
          widgetsList: this.dashboardWidgets,
          isDefault: this.dashboardData.isDefault,
          isSystemDefault: false,
          isActive: true
        };
        if (this.dashboardData.id == '') {
          delete body.guid;
        }
        this.dynamicDashboardService.addEditUserDashboard(body).subscribe(response => {
          if (response.isSuccess == true) {
            this.dashboardData.id = '';
            this.dashboardData.dashboardName = '';
            this.isEditItem = false;
            this.editWidgetIndex = null;
            this.draggedWidgetIndex = -1;
            this.draggedWidget = null;
            this.dashboardWidgets = [];
            this.dashboardWidgetsIndex = [];
            this.getDashboards();
            this._notificationService.handleResponse({ message: "Dashboard Saved successfully." }, "success");
          }
          else {
            this.spinner.hide();
            this._notificationService.handleResponse(response, "error");
          }
          this.originalWidgetData = null;
        }, error => {
          this.spinner.hide();
          this._notificationService.handleResponse(error, "error");
        });
      }
    }
  }

  editDashboard(type: string = 'view', is_cancel_btn: string = 'n') {
    this.spinner.show();
    this.dashboardWidgets = [];
    this.dashboardWidgetsIndex = [];
    if (type == 'edit' || type == 'copy') {
      this.dashboardData.id = (type == 'edit' ? this.dashboardList[this.dashboardData.index].id : '');
      this.dashboardData.dashboardName = (type == 'edit' ? this.dashboardList[this.dashboardData.index].dashboardName : '');
      this.dashboardData.isDefault = (type == 'edit' ? this.dashboardList[this.dashboardData.index].isDefault : false);
      for (var i = 0; i <= (this.dashboardList[this.dashboardData.index].widgets.length - 1); i++) {
        let newObj = this.dashboardList[this.dashboardData.index].widgets[i];
        this.dashboardWidgets.push(JSON.parse(JSON.stringify(newObj)));
        this.dashboardWidgetsIndex.push(this.dashboardList[this.dashboardData.index].widgets[i].id);
      }

      this.isPreview = false;
      this.options.enableEmptyCellDrop = true;
      this.options.draggable.enabled = true;
      this.options.resizable.enabled = true;
      this.options.pushItems = true;
      this.options.enableOccupiedCellDrop = true;
      this.options.swap = true;
      if (type == 'edit')
        this.originalDashboardData = JSON.parse(JSON.stringify(this.dashboardList[this.dashboardData.index]));
    }
    else if (type == 'view') {
      if (is_cancel_btn == 'y' && this.dashboardData.id != '') {
        this.dashboardList[this.dashboardData.index].dashboardName = this.originalDashboardData.dashboardName;
        this.dashboardList[this.dashboardData.index].isDefault = this.originalDashboardData.isDefault;
        this.dashboardList[this.dashboardData.index].widgets = this.originalDashboardData.widgets;
      }

      this.dashboardData.id = '';
      this.dashboardData.dashboardName = '';
      this.dashboardData.isDefault = false;
      for (var i = 0; i <= (this.dashboardList[this.dashboardData.index].widgets.length - 1); i++) {
        let newObj = this.dashboardList[this.dashboardData.index].widgets[i];
        this.dashboardWidgets.push(JSON.parse(JSON.stringify(newObj)));
        this.dashboardWidgetsIndex.push(this.dashboardList[this.dashboardData.index].widgets[i].id);
      }

      this.isPreview = true;
      this.options.enableEmptyCellDrop = false;
      this.options.draggable.enabled = false;
      this.options.resizable.enabled = false;
      this.options.pushItems = false;
      this.options.enableOccupiedCellDrop = false;
      this.options.swap = false;
      this.alertLimitchangeEvent.emit();
      this.zoomChangeEvent.emit();

      this.originalWidgetData = null;
      this.changeDetector.detectChanges();
    }
    else {
      this.dashboardData.id = '';
      this.dashboardData.dashboardName = '';
      this.dashboardData.isDefault = false;
      this.dashboardWidgets = [];
      this.dashboardWidgetsIndex = [];

      this.isPreview = false;
      this.options.enableEmptyCellDrop = true;
      this.options.draggable.enabled = true;
      this.options.resizable.enabled = true;
      this.options.pushItems = true;
      this.options.enableOccupiedCellDrop = true;
      this.options.swap = true;
    }
    this.isEditItem = false;
    this.editWidgetIndex = null;
    this.draggedWidgetIndex = -1;
    this.draggedWidget = null;

    if (this.options.api && this.options.api.optionsChanged) {
      this.options.api.optionsChanged();
    }
    this.displayWidgets()
    this.spinner.hide();
  }

  deleteDashbord() {
    this.deleteAlertDataModel = {
      title: "Delete Dashboard",
      message: this._appConstant.msgConfirm.replace('modulename', "Dashboard"),
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
        if (this.dashboardList[this.dashboardData.index].id != '') {
          this.spinner.show();
          this.dynamicDashboardService.deleteDashboard(this.dashboardList[this.dashboardData.index].id).subscribe(response => {
            if (response.isSuccess == true) {
              this.getDashboards();
              this._notificationService.handleResponse({ message: "Dashboard deleted successfully." }, "success");
            }
            else {
              this.spinner.hide();
              this._notificationService.handleResponse(response, "error");
            }
          }, error => {
            this.spinner.hide();
            this._notificationService.handleResponse(error, "error");
          });
        }
      }
    });
  }

  resetWidget(widgetId: string = '') {
    let index = this.dashboardWidgetsIndex.indexOf(widgetId);
    if (widgetId != '' && index >= 0) {
      this.deleteAlertDataModel = {
        title: "Reset Widget",
        message: 'Are you sure you want to reset this Widget?',
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
          let x = this.dashboardWidgets[index].x;
          let y = this.dashboardWidgets[index].y;
          this.dashboardWidgets[index] = JSON.parse(JSON.stringify(this.defaultWidgets.find(x => x.id == widgetId)));
          this.dashboardWidgets[index].x = x;
          this.dashboardWidgets[index].y = y;
          this.originalWidgetData = JSON.parse(JSON.stringify(this.dashboardWidgets[this.editWidgetIndex]));
          this.changeDetector.detectChanges();
        }
      });
    }
  }
  /*Gridster code*/

  /*Telemetry Widget*/
  getDeviceList() {
    this.getDashboards();
  }

  deviceChange(widgetIndex: number = 0, isClear: string = 'n') {
    if (isClear == 'y') {
      this.dashboardWidgets[widgetIndex].widgetProperty.telemetryAttributes = [];
    }
    this.getDeviceAttributesData(widgetIndex);
    this.telemetryDeviceChangeEvent.emit(this.dashboardWidgets[widgetIndex]);
    this.changeDetector.detectChanges();
  }

  changeDeviceAttributes(e, attributeValue: string = '', index: number = 0) {
    if (e.checked) {
      this.dashboardWidgets[index].widgetProperty.telemetryAttributes.push(attributeValue);
    }
    else {
      let i = this.dashboardWidgets[index].widgetProperty.telemetryAttributes.indexOf(attributeValue);
      if (i >= 0) {
        this.dashboardWidgets[index].widgetProperty.telemetryAttributes.splice(i, 1);
      }
    }
    this.telemetryAttributeChange(index);
  }

  telemetryAttributeChange(index: number = 0) {
    this.telemetryAttributeChangeEvent.emit(this.dashboardWidgets[index]);
    this.changeDetector.detectChanges();
  }

  telemetrySelectChange(e) {
    this.dashboardWidgets[this.editWidgetIndex].widgetProperty.telemetryUniqueId = e;
  }

  telemetryDeviceChange(index: number = 0) {
    this.dashboardWidgets[index].widgetProperty.telemetryAttributes = [];
    this.getDeviceAttributesData(index);
    this.telemetryDeviceChangeEvent.emit(this.dashboardWidgets[index]);
    this.changeDetector.detectChanges();
  }

  getDeviceAttributesData(widgetIndex) {
    if (this.dashboardWidgets[widgetIndex].widgetProperty.telemetryUniqueId != '') {
      this.deviceAttributesData = [];
      let device = this.deviceData.filter(s => s.uniqueId == this.dashboardWidgets[widgetIndex].widgetProperty.telemetryUniqueId);
      if (device && device.length > 0 && device[0].guid) {
        this.spinner.show();
        this.deviceService.getgenraterTelemetryData(device[0].templateGuid).subscribe(response => {
          this.spinner.hide();
          if (response.isSuccess === true) {
            this.deviceAttributesData = response.data;
          } else {
            this._notificationService.handleResponse(response, "error");
          }
        }, error => {
          this.spinner.hide();
          this._notificationService.handleResponse(error, "error");
        });
      }
    }
  }
  /*Telemetry Widget*/
}
