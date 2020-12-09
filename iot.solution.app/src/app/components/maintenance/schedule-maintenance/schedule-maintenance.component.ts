import { Component, OnInit, Input } from '@angular/core';
import { Notification, NotificationService, LookupService, DeviceService } from '../../../services';
import { ActivatedRoute, Router } from '@angular/router';
import { NgxSpinnerService } from 'ngx-spinner';
import { MatDialog } from '@angular/material';
import { AppConstant } from '../../../app.constants';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import * as moment from 'moment'

function biggerThenStartDate(enddate: any) {
	return {
		biggerDate: {}
	}
}

@Component({
	selector: 'app-schedule-maintenance',
	templateUrl: './schedule-maintenance.component.html',
	styleUrls: ['./schedule-maintenance.component.css']
})
export class ScheduleMaintenanceComponent implements OnInit {

	fleetList: any = [];
	fleets: any[];
	assets: any[];
	rangeFromLabel = 'From';
	rangeToLabel = 'To';
	isEdit = false;
	maintenance: any;
	maintenanceGuid: any;
	moduleName = "Schedule Maintenance";
	buttonName = "Submit";
	maintenanceForm: FormGroup;
	maintenanceObject: any = {};
	machines: any = [];
	checkSubmitStatus = false;
	today: any;
	public endDateValidate: any;
	minDate: any;
	maintenanceStatus = [{ "value": "Scheduled", 'disabled': false }, { "value": "Under maintenance", 'disabled': false }, { "value": "Completed", 'disabled': false }];

	constructor(
		private activatedRoute: ActivatedRoute,
		private spinner: NgxSpinnerService,
		private router: Router,
		private _service: DeviceService,
		public _appConstant: AppConstant,
		public lookupService: LookupService,
		private _notificationService: NotificationService,
	) {

		this.activatedRoute.params.subscribe(params => {
			if (params.maintenanceGuid != 'add') {
				this.isEdit = true;
				this.createFormGroup();
				this.maintenanceGuid = params.maintenanceGuid;
				this.moduleName = "Update Maintenance";
				this.buttonName = 'Update'
				setTimeout(() => {
					this.getMaintenanceDetails(this.maintenanceGuid);
				}, 200);

			} else {
				this.createFormGroup();
				this.maintenanceObject = { startDateTime: "", endDateTime: "", parentEntityGuid: "", entityGuid: "", deviceGuid: "", description: "", name: '', zipcode: '', countryGuid: '', stateGuid: '', isActive: 'true', city: '', latitude: '', longitude: '' }
			}
		});
	}

	ngOnInit() {
		if (!this.isEdit) {
			let currentUser = JSON.parse(localStorage.getItem('currentUser'));
			this.getFleetLookup(currentUser.userDetail.companyId);
		}
		this.today = new Date();
		let dd = this.today.getDate();
		let mm = this.today.getMonth() + 1; //January is 0!
		let yyyy = this.today.getFullYear();
		this.minDate = new Date(yyyy, mm - 1, dd);

		if (dd < 10) {
			dd = '0' + dd
		}
		if (mm < 10) {
			mm = '0' + mm
		}

		this.today = yyyy + '-' + mm + '-' + dd;
		this.endDateValidate = yyyy + '-' + mm + '-' + dd;
	}

	/**
	 * called while change the zone
	 * @param event   
	 */
	changeZone(event) {
		this.getAssetLookup(event.value);
	}

	/**
	 * get all assets
	 * @param entityGuid 
	 */
	getAssetLookup(entityGuid: any) {
		this.spinner.show();
		this.lookupService.getDevicelookup(entityGuid).subscribe(result => {
			this.spinner.hide();
			if (result.isSuccess) {
				this.assets = result.data;
			} else {
				this._notificationService.add(new Notification("error", result.message));
			}
		}, error => {
			this.spinner.show();
			this._notificationService.add(new Notification("error", error));
		});
	}

	/**
	 *This method will be invoked, when a user selects a location and based on that selection it will get the list of fleets.
	  * @param event The model value of the location(Entity)
	  */
	changeTemplate(event) {
		this.maintenanceForm.get("entityGuid").reset();
		this.getFleetLookup(event.value);
	}

	


	/**
	 * create a form of maintenance registration
	 */
	createFormGroup() {
		this.maintenanceForm = new FormGroup({
			entityGuid: new FormControl(''),
			deviceGuid: new FormControl({ value: '', disabled: this.isEdit }, [Validators.required]),
			description: new FormControl(null),
			timeZone: new FormControl(''),
			startDateTime: new FormControl({ value: '', disabled: false }, [Validators.required]),
			endDateTime: new FormControl({ value: '', disabled: false }, [Validators.required,Validators.min(this.endDateValidate)]),
		});
	}

	/**
	* Get Fleet Lookup by companyId
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
  	* Get Refrigerator Lookup by locationId
  	* @param locationId
  	*/
	getMachineslookup(locationId) {
		this.spinner.show();
		this.lookupService.getDevicelookup(locationId).
			subscribe(response => {
				if (response.isSuccess === true) {
					this.spinner.hide();
					this.machines = response.data;
					this.machines = this.machines.filter(word => word.isActive == true);

				} else {
					this._notificationService.add(new Notification('error', response.message));
				}
			}, error => {
				this.spinner.hide();
				this._notificationService.add(new Notification('error', error));
			})
	}

	getTimeZone() {
		return /\((.*)\)/.exec(new Date().toString())[1];
	}

	/**
	* validate end date using start date change
	* @param startdate
	*/
	onChangeStartDate(startdate) {
		let date = moment(startdate).add(this._appConstant.minGap, 'm').format();
		this.endDateValidate = new Date(date);
	}

	/**
	* get the local date
	* @param lDate 
	*/
	getLocalDate(lDate) {
     var stillUtc = moment.utc(lDate).toDate();
    var local = moment(lDate).local().format('MMM DD, YYYY hh:mm:ss A');
    return local;
	}

	/**
	* Get maintenance detials maintananceID
	* @param maintananceID
	*/
	getMaintenanceDetails(maintananceID) {
		this.spinner.show();
		this._service.getMaintenanceDetails(maintananceID).subscribe(response => {
			if (response.isSuccess === true) {
				this.maintenanceObject = response.data;
				if (this.maintenanceObject) {
				this.maintenanceObject.entityGuid = this.maintenanceObject.entityGuid?this.maintenanceObject.entityGuid.toUpperCase():"";
				this.maintenanceObject.parentEntityGuid = this.maintenanceObject.parentEntityGuid?this.maintenanceObject.parentEntityGuid.toUpperCase():"";
				this.maintenanceObject.deviceGuid = this.maintenanceObject.deviceGuid?this.maintenanceObject.deviceGuid.toUpperCase():"";
				this.maintenanceObject.startDateTime = moment(this.maintenanceObject.startDateTime + 'Z').local();
				this.maintenanceObject.endDateTime = moment(this.maintenanceObject.endDateTime + 'Z').local();
				}				
				if (this.maintenanceObject.status == 'Under Maintenance') {
					this.maintenanceStatus[0].disabled = true;
				}
				if (this.maintenanceObject.status == 'Completed') {
					this.maintenanceForm.controls['startDateTime'].disable();
					this.maintenanceForm.controls['endDateTime'].disable();
					this.maintenanceStatus[0].disabled = true;
					this.maintenanceStatus[1].disabled = true;
				}
				let currentUser = JSON.parse(localStorage.getItem('currentUser'));
				this.getFleetLookup(currentUser.userDetail.companyId);
				this.spinner.hide();
			}
		});
	}

	/**
	 * Schedule/Update Maintenance
	 * */
	scheduleMaintenance() {
		this.checkSubmitStatus = true;
		var maintenance = { ...this.maintenanceForm.value };
		maintenance.startDateTime = moment(maintenance.startDateTime).format('YYYY-MM-DDTHH:mm:ss');
		maintenance.endDateTime = moment(maintenance.endDateTime).format('YYYY-MM-DDTHH:mm:ss');
		maintenance.timeZone = moment().utcOffset();
		let currentUser = JSON.parse(localStorage.getItem('currentUser'));
		maintenance.entityGuid = currentUser.userDetail.entityGuid;
		if (this.isEdit) {
			this.maintenanceForm.registerControl('guid', new FormControl(this.maintenanceGuid))
		}
		if (this.maintenanceForm.status === "VALID") {

			this.spinner.show();
			let data = maintenance;
			if (this.isEdit) {
				data.entityGuid = this.maintenanceForm.get('entityGuid').value;
				data.deviceGuid = this.maintenanceForm.get('deviceGuid').value;
				data.guid = this.maintenanceGuid;
			}

			this._service.scheduleMaintenance(data).subscribe(response => {
				if (response.isSuccess === true) {
					this.spinner.hide();
					if (this.isEdit) {
						this._notificationService.add(new Notification('success', "Scheduled Maintenance has been updated successfully."));
					} else {
						this._notificationService.add(new Notification('success', "Scheduled Maintenance has been added successfully."));
					}
					this.router.navigate(['/maintenance']);
				} else {
					this.spinner.hide();
					this._notificationService.add(new Notification('error', response.message));
				}
			});
		}
	}
}
