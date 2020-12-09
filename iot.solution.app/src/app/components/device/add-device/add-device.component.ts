import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router'
import { FormControl, FormGroup, Validators } from '@angular/forms'
import { NgxSpinnerService } from 'ngx-spinner'
import { Notification, NotificationService, RolesService, LookupService, DeviceService } from 'app/services';
import { AppConstant } from 'app/app.constants';

@Component({
  selector: 'app-add-device',
  templateUrl: './add-device.component.html',
  styleUrls: ['./add-device.component.css']
})
export class AddDeviceComponent implements OnInit {

  moduleName = "Add Device";
  buttonName = "Submit"
  deviceObject = {};
  deviceGuid = '';
  isEdit = false;
  deviceForm: FormGroup;
  currentUser = JSON.parse(localStorage.getItem("currentUser"));
  checkSubmitStatus = false;

  deviceTemplateList: any[];

  constructor(
    //private deviceService: DeviceService,
    private router: Router,
    //private _notificationService: NotificationService,
    private activatedRoute: ActivatedRoute,
    private spinner: NgxSpinnerService,
    private rolesService: RolesService,
    private deviceService: DeviceService,
    private _notificationService: NotificationService,
    public _appConstant: AppConstant,
    public lookupService: LookupService
  ) {
    this.createFormGroup();
    this.activatedRoute.params.subscribe(params => {
      if (params.deviceGuid != 'add') {
        this.getDeviceDetails(params.deviceGuid);
        this.deviceGuid = params.deviceGuid;
        this.moduleName = "Edit Device";
        this.buttonName = "Update";
        this.isEdit = true;
      } else {
        this.deviceObject = {}
      }
    });
  }

  ngOnInit() {
    this.getDeviceTemplateLookup();
  }


  /**
   * create a form for 
   */
  createFormGroup() {
    this.deviceForm = new FormGroup({
      uniqueId: new FormControl('', [Validators.required, Validators.pattern('^[A-Za-z0-9]+$')]),
      name: new FormControl('', [Validators.required]),
      description: new FormControl(''),
      specification: new FormControl(''),
      templateGuid: new FormControl('', [Validators.required]),
      entityGuid: new FormControl('', [Validators.required]),
      isActive: new FormControl('', [Validators.required]),
    });
  }

  /**
  * Get Device Template Lookup
  * */
  getDeviceTemplateLookup() {
    this.deviceTemplateList = [];
    this.lookupService.getLookup('templates').
      subscribe(response => {
        if (response.isSuccess === true) {
          this.deviceTemplateList = response['data'];
        } else {
          this._notificationService.add(new Notification('error', response.message));
        }
      }, error => {
        this.spinner.hide();
        this._notificationService.add(new Notification('error', error));
      })
  }

  /**
   * Manage Device ADD/UPDATE   
   */
  manageDevice() {
    this.checkSubmitStatus = true;
    if (this.isEdit) {
      this.deviceForm.patchValue({ "isActive": this.deviceObject['isActive'] });
    } else {
      this.deviceForm.patchValue({ "isActive": true });
    }
    this.deviceForm.get('entityGuid').setValue(this.currentUser.userDetail.entityGuid);

    if (this.deviceForm.status === "VALID") {
      this.spinner.show();
      let successMessage = this._appConstant.msgCreated.replace("modulename", "Device");
      if (this.isEdit) {
        this.deviceForm.registerControl("guid", new FormControl(''));
        this.deviceForm.patchValue({ "guid": this.deviceGuid });
        successMessage = this._appConstant.msgUpdated.replace("modulename", "Device");
      }
      this.deviceService.manageDevice(this.deviceForm.value).subscribe(response => {
        this.spinner.hide();
        if (response.isSuccess === true) {
          this.router.navigate(['/devices']);
          this._notificationService.add(new Notification('success', successMessage));
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

  /**
   * Get device detail by device guid 
   * @param deviceGuid 
   */
  getDeviceDetails(deviceGuid) {
    this.spinner.show();
    this.deviceService.getDeviceDetails(deviceGuid).subscribe(response => {
      this.spinner.hide();
      if (response.isSuccess === true) {
        this.deviceObject = response.data;
      } else {
        this._notificationService.add(new Notification('error', response.message));
      }
    }, error => {
      this.spinner.hide();
      this._notificationService.add(new Notification('error', error));
    });
  }

}
