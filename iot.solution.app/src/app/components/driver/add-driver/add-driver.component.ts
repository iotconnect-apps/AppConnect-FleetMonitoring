import { Component, OnInit, ViewChild, ElementRef } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router'
import { FormControl, FormGroup, Validators, FormBuilder } from '@angular/forms'
import { NgxSpinnerService } from 'ngx-spinner'
import { valHooks } from 'jquery';
import { MatDialog } from '@angular/material';
import { MessageAlertDataModel, DeleteAlertDataModel, AppConstant } from '../../../app.constants';
import { RolesService, Notification, NotificationService, LookupService, DeviceService } from '../../../services';
import { DriverService } from '../../../services/driver/driver.service';
import { MessageDialogComponent, DeleteDialogComponent } from '../..';
import { CustomValidators } from '../../../helpers/custom.validators';

@Component({
  selector: 'app-add-driver',
  templateUrl: './add-driver.component.html',
  styleUrls: ['./add-driver.component.css']
})
export class AddDriverComponent implements OnInit {
  @ViewChild('driverImgFile', { static: false }) driverImgFile: ElementRef;
  @ViewChild('licenceImgFile', { static: false }) licenceImgFile: ElementRef;

  public contactNoError: boolean = false;
  public mask = {
    guide: true,
    showMask: false,
    keepCharPositions: true,
    mask: ['(', /[0-9]/, /\d/, ')', '-', /\d/, /\d/, /\d/, '-', /\d/, /\d/, /\d/, '-', /\d/, /\d/, /\d/, /\d/]
  };
  fleetGuid = null;
  hasDriverImage = false;
  hasLicenceImage = false;
  mediaUrl: any;
  imagefileUrl: any;
  licenceFileUrl: any;
  licenceFileName: any;
  handleLicFileInput: boolean;
  validStatusLicFile: boolean;
  currentDriverImage: any;
  currentLicenceImage: any;
  moduleName = "Add Driver";
  buttonName = "Submit"
  driverObject: any = {};
  driverGuid = '';
  isEdit = false;
  driverForm: FormGroup;
  currentUser = JSON.parse(localStorage.getItem("currentUser"));
  checkSubmitStatus = false;
  MessageAlertDataModel: MessageAlertDataModel;
  deleteAlertDataModel: DeleteAlertDataModel;


  deviceTemplateList: any[];
  countryList: any[];
  stateList: any[];
  handleImgFileInput: boolean;
  validStatusImgFile: boolean;
  imgFileName: any;
  imgFileToUpload: any;
  currentImageFile: any;
  licFileToUpload: any;
  fleetList: any[];

  constructor(
    private formBuilder: FormBuilder,
    private router: Router,
    private activatedRoute: ActivatedRoute,
    private spinner: NgxSpinnerService,
    private rolesService: RolesService,
    public driverService: DriverService,
    private _notificationService: NotificationService,
    public _appConstant: AppConstant,
    public lookupService: LookupService,
    public dialog: MatDialog,
  ) {
    this.createFormGroup();
    this.activatedRoute.params.subscribe(params => {
      if (params.driverGuid != 'add') {
        this.getDriverDetails(params.driverGuid);
        this.driverGuid = params.driverGuid;
        this.moduleName = "Edit Driver";
        this.buttonName = "Update";
        this.isEdit = true;
      } else {
        this.getFleetLookup();
        this.driverObject = {}
      }
    });
  }

  ngOnInit() {
    this.mediaUrl = this._notificationService.apiBaseUrl;
    this.getCountryLookup();
  }

  /**
	 * Handle image type
	 * @param event
	 */
  handleImageInput(event, imageType) {
    if (imageType == "P") {
      this.handleImgFileInput = true;
    }
    if (imageType == "L") {
      this.handleLicFileInput = true;
    }
    let files = event.target.files;
    var that = this;
    if (files.length) {
      let fileType = files.item(0).name.split('.');
      let imagesTypes = ['jpeg', 'JPEG', 'jpg', 'JPG', 'png', 'PNG'];
      if (imagesTypes.indexOf(fileType[fileType.length - 1]) !== -1) {
        if (imageType == "P") {
          this.validStatusImgFile = true;
          this.imgFileName = files.item(0).name;
          this.imgFileToUpload = files.item(0);
        }
        if (imageType == "L") {
          this.validStatusLicFile = true;
          this.licenceFileName = files.item(0).name;
          this.licFileToUpload = files.item(0);
        }
        if (imageType == "P") {
          if (event.target.files && event.target.files[0]) {
            var reader = new FileReader();
            reader.readAsDataURL(event.target.files[0]);
            reader.onload = (innerEvent: any) => {
              this.imagefileUrl = innerEvent.target.result;
              that.driverObject["image"] = this.imagefileUrl;
            }
          }
        }

        if (imageType == "L") {
          if (event.target.files && event.target.files[0]) {
            var reader = new FileReader();
            reader.readAsDataURL(event.target.files[0]);
            reader.onload = (innerEvent: any) => {
              this.licenceFileUrl = innerEvent.target.result;
              that.driverObject["licenceImage"] = this.licenceFileUrl;
            }
          }
        }

      } else {
        this.driverImageRemove();
        this.licenceImageRemove();
        this.MessageAlertDataModel = {
          title: "Image Type",
          message: "Invalid Image Type.",
          message2: "Upload .jpg, .jpeg, .png Image Only.",
          okButtonName: "OK",
        };
        const dialogRef = this.dialog.open(MessageDialogComponent, {
          width: '400px',
          height: 'auto',
          data: this.MessageAlertDataModel,
          disableClose: false
        });
      }
    }
  }

  /**
	 * Delete image confirmation popup
	 * */
  deleteImgModel(imageType) {
    if (this.imgFileToUpload || this.licFileToUpload) {
      if (imageType == 'P') {
        
        if (this.driverObject.image) {
          this.driverObject.image = this.mediaUrl + this.driverObject.image;
          this.currentDriverImage = this.driverObject.image;
          this.hasDriverImage = true;
        }
        else {
          this.currentImageFile = '';
          this.driverObject['image'] = null;
          this.driverForm.get('imageFile').setValue('');
          this.imagefileUrl = "";
        }
      }
      if (imageType == 'L') {
        this.currentImageFile = '';
        this.driverObject['licenceImage'] = null;
        this.driverForm.get('licenceFile').setValue('');
        this.licenceFileUrl = "";
      }
    }
    else if (this.hasDriverImage || this.hasLicenceImage) {
      this.deleteAlertDataModel = {
        title: "Delete Image",
        message: this._appConstant.msgConfirm.replace('modulename', "Image"),
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
        if (result && imageType == 'P') {
          this.deleteDriverImg();
        }
        else if (result && imageType == 'L') {
          this.deleteLicenceImg();
        }
      });
    }
  }

  /**
	 * Delete driver image
	 * */
  deleteDriverImg() {

    this.spinner.show();
    this.driverService.deleteDriverImage(this.driverGuid).subscribe(response => {
     	this.spinner.hide();
      if (response.isSuccess === true) {
        this.currentDriverImage = '';
        this.driverObject['licenceImage'] = null;
        this.driverForm.get('licenceFile').setValue('');
        this.imagefileUrl = "";
     		this._notificationService.add(new Notification('success', this._appConstant.msgDeleted.replace("modulename", "Driver Image")));
     	} else {
     		this._notificationService.add(new Notification('error', response.message));
     	}
     }, error => {
     	this.spinner.hide();
     	this._notificationService.add(new Notification('error', error));
     });
  }

  /**
   * Delete licence image
   * */
  deleteLicenceImg() {

    this.spinner.show();
    this.driverService.deleteLicenceImage(this.driverGuid).subscribe(response => {
      this.spinner.hide();
      if (response.isSuccess === true) {
        this.currentLicenceImage = '';
        this.driverObject['image'] = null;
        this.driverForm.get('imageFile').setValue('');
        this.licenceFileUrl = "";
        this._notificationService.add(new Notification('success', this._appConstant.msgDeleted.replace("modulename", "Licence Image")));
      } else {
        this._notificationService.add(new Notification('error', response.message));
      }
    }, error => {
      this.spinner.hide();
      this._notificationService.add(new Notification('error', error));
    });
  }

  /**
   * to get the country Lookup
   */
  getCountryLookup() {
    this.spinner.show();
    this.lookupService.getcountryList()
      .subscribe(response => {
        if (response.isSuccess) {
          this.countryList = response.data.data;
        }
      },
        error => {
          this._notificationService.add(new Notification("error", error));
        });
  }

  /**
   * to get the list of fleet
   */
  getFleetLookup() {
    this.spinner.show();
    this.lookupService.driverfleetlookup(this.fleetGuid)
      .subscribe(response => {
        if (response.isSuccess) {
          this.spinner.hide();
          this.fleetList = response.data;
        } else {
          this.spinner.hide();
          this._notificationService.add(new Notification("error", response.message));
        }
      }, error => {
        this.spinner.hide();
        this._notificationService.add(new Notification("error", error));

      });
  }

  /**
   * get state look up based on country
   */
  getStateList(event) {
    let countryGuid = event.value
    this.spinner.show();
    this.lookupService.getcitylist(countryGuid).subscribe(response => {
      if (response.isSuccess) {
        this.stateList = response.data;
      } else {
        this._notificationService.add(new Notification("error", response.message));

      }
      this.spinner.hide();
    }, error => {
      this.spinner.hide();
      this._notificationService.add(new Notification("error", error));
    });
  }

  /**
   * create a form for 
   */
  createFormGroup() {
    this.driverForm = this.formBuilder.group({
      //guid:null,
      driverId: new FormControl('', [Validators.required]),
      imageFile: new FormControl('', [Validators.required]),
      licenceFile: new FormControl('', [Validators.required]),
      firstName: new FormControl('', [Validators.required]),
      lastName: new FormControl('', [Validators.required]),
      email: new FormControl('', [Validators.required,
      Validators.pattern(/^(([^<>()\[\]\\.,;:\s@"]+(\.[^<>()\[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/)]),
      contactNo: new FormControl('', [Validators.required, Validators.maxLength(10), Validators.minLength(10), Validators.pattern(/^[0-9]*$/)]),
      address: new FormControl('', [Validators.required]),
      licenceNo: new FormControl('', [Validators.required]),
      city: new FormControl('', [Validators.required]),
      zipcode: new FormControl('', [Validators.required, Validators.pattern('^[A-Z0-9 _]*$')]),
      countryGuid: new FormControl('', [Validators.required]),
      stateGuid: new FormControl('', [Validators.required]),
      fleetGuid: new FormControl('', [Validators.required]),
      phoneCountryCode: new FormControl('')
    },
      {
        validators: CustomValidators.checkPhoneValue('contactNo')
      });
  }

  /**
   * Manage Driver ADD/UPDATE   
   */
  manageDriver() {
    this.checkSubmitStatus = true;
    let contactNo = this.driverForm.value.contactNo.replace("(", "")
    let contactno = contactNo.replace(")", "")
    let finalcontactno = contactno.replace("-", "")
    if (finalcontactno.match(/^0+$/)) {
      this.contactNoError = true;
      return
    } else {
      this.contactNoError = false;
    }

    if (this.driverForm.status === "VALID") {
      if (this.imgFileToUpload) {
        this.driverForm.get('imageFile').setValue(this.imgFileToUpload);
      }
      if (this.licFileToUpload) {
        this.driverForm.get('licenceFile').setValue(this.licFileToUpload);
      }
      this.driverForm.get('contactNo').setValue(contactno);
      this.spinner.show();
      let successMessage = this._appConstant.msgCreated.replace("modulename", "Driver");
      if (this.isEdit) {
        this.driverForm.registerControl("guid", new FormControl(''));
        this.driverForm.patchValue({ "guid": this.driverGuid });
        successMessage = this._appConstant.msgUpdated.replace("modulename", "Driver");
      }
      this.driverService.manageDriver(this.driverForm.value).subscribe(response => {
        this.spinner.hide();
        if (response.isSuccess === true) {
          this.router.navigate(['/drivers']);
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
   * Get driver detail by driverGuid
   * @param driverGuid
   */
  getDriverDetails(driverGuid) {
    this.spinner.show();
    this.driverService.getDriverDetails(driverGuid).subscribe(response => {
      this.spinner.hide();
      if (response.isSuccess === true) {
        this.driverObject = response.data;
        this.fleetGuid = this.driverObject.fleetGuid;
        this.getFleetLookup();
        if (this.driverObject.image) {
          this.driverObject.image = this.mediaUrl + this.driverObject.image;
          this.currentDriverImage = this.driverObject.image;
          this.hasDriverImage = true;
        } else {
          this.hasDriverImage = false;
        }
        if (this.driverObject.licenceImage) {
          this.driverObject.licenceImage = this.mediaUrl + this.driverObject.licenceImage;
          this.currentLicenceImage = this.driverObject.image;
          this.hasDriverImage = true;
        } else {
          this.hasDriverImage = false;
        }
        this.lookupService.getcitylist(response.data.countryGuid).subscribe(response => {
          this.stateList = response.data;

        });

        this.spinner.hide();
      } else {
        this._notificationService.add(new Notification('error', response.message));
      }
    }, error => {
      this.spinner.hide();
      this._notificationService.add(new Notification('error', error));
    });
  }

  /**
   * Image remove of driver
   * */
  driverImageRemove() {
    this.driverImgFile.nativeElement.value = "";
    if (this.driverObject['image'] == this.currentDriverImage) {
      this.driverForm.get('imageFile').setValue('');
      if (!this.handleImgFileInput) {
        this.handleImgFileInput = false;
        this.deleteDriverImgModel();
      }
      else {
        this.handleImgFileInput = false;
      }
    }
    else {
      if (this.currentDriverImage) {
        this.spinner.hide();
        this.driverObject['image'] = this.currentDriverImage;
        this.imgFileToUpload = false;
        this.imgFileName = '';
        this.imagefileUrl = null;
      }
      else {
        this.spinner.hide();
        this.driverObject['image'] = null;
        this.driverForm.get('imageFile').setValue('');
        this.imgFileToUpload = false;
        this.imgFileName = '';
        this.imagefileUrl = null;
      }
    }
  }

  /**
   * Delte confirmation popup
   * */
  deleteDriverImgModel() {
    this.deleteAlertDataModel = {
      title: "Delete Driver Image",
      message: this._appConstant.msgConfirm.replace('modulename', "Driver Image"),
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
        this.deleteDriverImg();
      }
    });
  }

  /**
   * Image remove of licence
   * */
  licenceImageRemove() {
    this.licenceImgFile.nativeElement.value = "";
    if (this.driverObject['licenceImage'] == this.currentLicenceImage) {
      this.driverForm.get('licenceFile').setValue('');
      if (!this.handleImgFileInput) {
        this.handleImgFileInput = false;
        this.deleteLicenceImgModel();
      }
      else {
        this.handleImgFileInput = false;
      }
    }
    else {
      if (this.currentLicenceImage) {
        this.spinner.hide();
        this.driverObject['licenceImage'] = this.currentLicenceImage;
        this.licFileToUpload = false;
        this.licenceFileName = '';
        this.licenceFileUrl = null;
      }
      else {
        this.spinner.hide();
        this.driverObject['licenceImage'] = null;
        this.driverForm.get('licenceFile').setValue('');
        this.licFileToUpload = false;
        this.licenceFileName = '';
        this.licenceFileUrl = null;
      }
    }
  }

  /**
   * Delte licence confirmation popup
   * */
  deleteLicenceImgModel() {
    this.deleteAlertDataModel = {
      title: "Delete Licence Image",
      message: this._appConstant.msgConfirm.replace('modulename', "Licence Image"),
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
        this.deleteLicenceImg();
      }
    });
  }

}
