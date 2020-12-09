/// <reference types="@types/googlemaps" />
import { Component, OnInit, ViewChild, ElementRef, NgZone, Renderer2 } from '@angular/core';
import { FormGroup, Validators, FormControl, FormArray } from '@angular/forms';
import { NgxSpinnerService } from 'ngx-spinner';
import { Router, ActivatedRoute } from '@angular/router';
import { NotificationService, LookupService, Notification, FleetService } from '../../../services';
import { MessageAlertDataModel, DeleteAlertDataModel, AppConstant } from '../../../app.constants';
import { MatDialog } from '@angular/material';
import { MessageDialogComponent, DeleteDialogComponent } from '../..';
import { MouseEvent } from '@agm/core';
import { debug } from 'util';
import { MapsAPILoader, AgmMap } from '@agm/core';
import { deviceObj } from './device-model';
declare const google: any

// just an interface for type safety.
interface marker {
  lat: number;
  lng: number;
  label?: string;
  draggable: boolean;
}

@Component({
  selector: 'app-add-fleet',
  templateUrl: './add-fleet.component.html',
  styleUrls: ['./add-fleet.component.css']
})
export class AddFleetComponent implements OnInit {

  
  @ViewChild('myFile', { static: false }) myFile: ElementRef;
  @ViewChild('mediaFile', { static: false }) mediaFile: ElementRef;
  @ViewChild('search', { static: true }) public searchElementRef: ElementRef;

  public zoom: number = 12;
  public latitude: number;
  public longitude: number;
  public latlongs: any = []; 
  public latlong: any = {};
  public searchControl: FormControl;
  public radius: number = 100;
  currentAddress: any;
 
  moduleName = "Add Fleet";
  buttonname = "Submit";
  MessageAlertDataModel: MessageAlertDataModel;
  deleteAlertDataModel: DeleteAlertDataModel;
  selectedFiles: any = [];
  selectedImages: any = [];
  selectedFilesObj: any = [];
  fileName = '';
  fileToUpload: any;
  mediaUrl: any;
  hasImage = false;
  handleImgInput = false;
  fleetObject: any = {};
  fileUrl: any;
  buttonName: any = "Submit";
  fleetTypeList: any = [];
  materialTypeList: any = [];
  deviceList: any = [];
  deviceTemplateList: any = [];
  fleetGuid = '';
  isEdit = false;
  isView = false;
  checkSubmitStatus = false;
  checkDeviceSubmitStatus = false;
  currentImage: any;
  hasDevice = true;
  addDeviceMsg: any = 'Add a device first';
  selectDevice: string = "Select Device";
  fleetForm: FormGroup;

  deviceForm = new FormGroup({
    deviceGuid: new FormArray([]),
    templateGuid: new FormArray([]),
    index: new FormArray([]),
  });

  get deviceGuid(): FormArray {
    return this.deviceForm.get('deviceGuid') as FormArray;
  }

  get templateGuid(): FormArray {
    return this.deviceForm.get('templateGuid') as FormArray;
  }

  get index(): FormArray {
    return this.deviceForm.get('index') as FormArray;
  }

  addDevices() {
    this.deviceGuid.push(new FormControl('', Validators.required));
    this.templateGuid.push(new FormControl('', Validators.required));
    this.index.push(new FormControl(''));
  }

  removeDevices(i) {
    this.deviceGuid.removeAt(i);
    this.templateGuid.removeAt(i);
    this.index.removeAt(i);
  }

  constructor(
    private router: Router,
    private spinner: NgxSpinnerService,
    private _notificationService: NotificationService,
    private activatedRoute: ActivatedRoute,
    public lookupService: LookupService,
    public dialog: MatDialog,
    public _appConstant: AppConstant,
    public _service: FleetService,
    private mapsAPILoader: MapsAPILoader,
    private ngZone: NgZone,
    public renderer: Renderer2) {
    this.createFormGroup();
    this.addDevices();
    this.activatedRoute.params.subscribe(params => {
      if (params.fleetGuid != 'add') {
        this.fleetGuid = params.fleetGuid;
        this.moduleName = "Edit Fleet";
        this.buttonName = "Update";
        this.isEdit = true;
        this.getFleetDetails(this.fleetGuid);
      }
      this.fleetObject = {
        fleetId: '', registrationNo: '', loadingCapacity: '', typeGuid: '', materialTypeGuid: '',
        speedLimit: '', image: '', fleetPermissionFiles: '', latitude: '', longitude: '', radius: '',
        devices: [new deviceObj()]
      };
    });
  }

  ngOnInit() {
    this.mediaUrl = this._notificationService.apiBaseUrl;
    //this.zoom = 12;
    //this.latitude = 32.897480;
    //this.longitude = -97.040443;

    this.searchControl = new FormControl();
   
    //load Places Autocomplete
    this.mapsAPILoader.load().then(() => {
      this.setCurrentPosition();
      let autocomplete = new google.maps.places.Autocomplete(this.searchElementRef.nativeElement, {
        types: []
      });
      autocomplete.addListener("place_changed", () => {
        this.ngZone.run(() => {
          //get the place result
          let place: google.maps.places.PlaceResult = autocomplete.getPlace();

          //verify result
          if (place.geometry === undefined || place.geometry === null) {
            return;
          }

          //set latitude, longitude and zoom
          this.latitude = place.geometry.location.lat();
          this.longitude = place.geometry.location.lng();
          this.zoom = 12;
        });
      });
    });
    this.getFleetTypeLookup();
    this.getMaterialTypeLookup();
    this.getDeviceTemplateLookup();
    // this.renderer.setProperty(this.searchElementRef.nativeElement,'value',this.currentAddress);
    
    //this.searchControl.setValue(this.currentAddress);
  }

  recenterMap() {
    this.latitude = 36.8392542;
    this.longitude = 10.313922699999999;
  }

  private setCurrentPosition() {
    if (navigator.geolocation) {
      navigator.geolocation.getCurrentPosition((position) => {
        this.latitude = +position.coords.latitude;
        this.longitude = +position.coords.longitude;
        this.zoom = 12;

        var google_map_pos = new google.maps.LatLng(this.latitude, this.longitude);

        /* Use Geocoder to get address */
        var google_maps_geocoder = new google.maps.Geocoder();
        google_maps_geocoder.geocode(
          { 'latLng': google_map_pos },
          function (results, status) {
            if (status == google.maps.GeocoderStatus.OK && results[0]) {
              this.currentAddress = results[0].formatted_address;
            }
          }
        );
      });
    }
  }

  event(type, $event) {
    if ($event >= 100)
    this.radius = +$event;
  }

  getPosition() {
    if (navigator.geolocation) {
      navigator.geolocation.getCurrentPosition((position) => {
        this.latitude = position.coords.latitude + (0.0000000000100 * Math.random());
        this.longitude = position.coords.longitude + (0.0000000000100 * Math.random());
      });
    } else {
      alert("Geolocation is not supported by this browser.");
    }
  }

  /**
   * Create Form deviceForm
   * */
  createFormGroup() {
    this.fleetForm = new FormGroup({
      fleetId: new FormControl('', [Validators.required]),
      typeGuid: new FormControl('', [Validators.required]),
      registrationNo: new FormControl('', [Validators.required]),
      loadingCapacity: new FormControl('', [Validators.required]),
      materialTypeGuid: new FormControl('', [Validators.required]),
      imageFile: new FormControl('', [Validators.required]),
      speedLimit: new FormControl('', [Validators.required]),
      permissionFiles: new FormControl(''),
      latitude: new FormControl(''),
      longitude: new FormControl(''),
      location: new FormControl('', [Validators.required]),
      radius: new FormControl('', [Validators.required]),
      devices: new FormArray([]),
    });
  }

  /**
  * Get Fleet Type Lookup
  * */
  getFleetTypeLookup() {
    this.fleetTypeList = [];
    this.lookupService.getLookup('fleettype').
      subscribe(response => {
        if (response.isSuccess === true) {
          this.fleetTypeList = response['data'];
        } else {
          this._notificationService.add(new Notification('error', response.message));
        }
      }, error => {
        this.spinner.hide();
        this._notificationService.add(new Notification('error', error));
      })
  }

  /**
  * Get Material Type Lookup
  * */
  getMaterialTypeLookup() {
    this.materialTypeList = [];
    this.lookupService.getLookup('fleetmaterialtype').
      subscribe(response => {
        if (response.isSuccess === true) {
          this.materialTypeList = response['data'];
        } else {
          this._notificationService.add(new Notification('error', response.message));
        }
      }, error => {
        this.spinner.hide();
        this._notificationService.add(new Notification('error', error));
      })
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
   * Get Device lookup by templateId
   * @param templateId
   */
  getDeviceLookup(templateId, deviceGuid) {
    this.deviceList = [];
    this.spinner.show();
    this.lookupService.getDeviceByTemplateLookup(templateId, deviceGuid).
      subscribe(response => {
        if (response.isSuccess === true) {
          if (response.data.length > 0) {
            this.selectDevice = "Select Device";
            this.hasDevice = true;
          }
          else {
            this.selectDevice = "No Device";
            this.hasDevice = false;
          }
          this.deviceList = response['data'];
        } else {
          this.hasDevice = false;
          this._notificationService.add(new Notification('error', response.message));
        }
        this.spinner.hide();
      }, error => {
        this.spinner.hide();
        this._notificationService.add(new Notification('error', error));
      })
  }

  /**
   * Manage fleet
   * */
  manageFleet() {
    this.checkSubmitStatus = true;
    this.checkDeviceSubmitStatus = true;
    this.fleetForm.value.devices = [];

    this.fleetForm.get('latitude').setValue(this.latitude);
    this.fleetForm.get('longitude').setValue(this.longitude);
    this.fleetForm.get('radius').setValue(this.radius);
   
    if (this.isEdit) {
      this.fleetForm.registerControl('guid', new FormControl(''));
      this.fleetForm.patchValue({ "guid": this.fleetGuid });
    }
    if (this.fleetForm.status === "VALID" && this.deviceForm.status === "VALID") {
      if (this.selectedFiles) {
        this.fleetForm.get('permissionFiles').setValue(this.selectedFiles);
      }
      if (this.fileToUpload) {
        this.fleetForm.get('imageFile').setValue(this.fileToUpload);
      }
      
      for (let i = 0; i < this.deviceGuid.length; i++) {
        this.fleetForm.value.devices.push({ DeviceGuid: this.deviceGuid.at(i).value, TemplateGuid: this.templateGuid.at(i).value });
      }
      this.spinner.show();

      this._service.addFleet(this.fleetForm.value).subscribe(response => {
        this.spinner.hide();
        if (response.isSuccess === true) {
          if (this.isEdit) {
            this._notificationService.add(new Notification('success', "Fleet has been updated successfully."));
          } else {
            this._notificationService.add(new Notification('success', "Fleet has been added successfully."));
          }
          this.router.navigate(['/fleet']);
        } else {
          this._notificationService.add(new Notification('error', response.message));
        }
      });

    }
  }

  /**
   * Add device 
   * */
  addDevice() {
    this.checkDeviceSubmitStatus = true;

    if(this.deviceForm.status === "VALID")
    this.addDevices();
  }

  removeDevice(index) {
    this.removeDevices(index);
  }

  /**
	 * Handle image type
	 * @param event
	 */
  handleImageInput(event) {
    this.handleImgInput = true;
    let files = event.target.files;
    var that = this;
    if (files.length) {
      let fileType = files.item(0).name.split('.');
      let imagesTypes = ['jpeg', 'JPEG', 'jpg', 'JPG', 'png', 'PNG'];
      if (imagesTypes.indexOf(fileType[fileType.length - 1]) !== -1) {
        this.fileName = files.item(0).name;
        this.fileToUpload = files.item(0);
        if (event.target.files && event.target.files[0]) {
          var reader = new FileReader();
          reader.readAsDataURL(event.target.files[0]);
          reader.onload = (innerEvent: any) => {
            this.fileUrl = innerEvent.target.result;
            that.fleetObject.image = this.fileUrl;

          }
        }
      } else {
        this.imageRemove();
        this.MessageAlertDataModel = {
          title: "Fleet Image",
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
    * Remove image
    * */
  imageRemove() {
    this.myFile.nativeElement.value = "";
    if (this.fleetObject['image'] == this.currentImage) {
      this.fleetForm.get('imageFile').setValue('');
      if (!this.handleImgInput) {
        this.handleImgInput = false;
        this.deleteImgModel();
      }
      else {
        this.handleImgInput = false;
      }
    }
    else {
      if (this.currentImage) {
        this.spinner.hide();
        this.fleetObject['image'] = this.currentImage;
        this.fileToUpload = false;
        this.fileName = '';
        this.fileUrl = null;
      }
      else {
        this.spinner.hide();
        this.fleetObject['image'] = null;
        this.fleetForm.get('imageFile').setValue('');
        this.fileToUpload = false;
        this.fileName = '';
        this.fileUrl = null;
      }
    }
  }

  /**
	 * Delete image confirmation popup
	 * */
  deleteImgModel() {
    this.deleteAlertDataModel = {
      title: "Delete Image",
      message: this._appConstant.msgConfirm.replace('modulename', "Fleet Image"),
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
        this.deleteFleetImg();
      }
    });
  }

  /**
 * Delete Fleet image
 * */
  deleteFleetImg() {
    this.spinner.show();
    this._service.removeFleetImage(this.fleetGuid).subscribe(response => {
      this.spinner.hide();
      if (response.isSuccess === true) {
        this.currentImage = '';
        this.fleetObject['image'] = null;
        this.fleetForm.get('imageFile').setValue('');
        this.fileUrl = "";
        this._notificationService.add(new Notification('success', this._appConstant.msgDeleted.replace("modulename", "Fleet Image")));
      } else {
        this._notificationService.add(new Notification('error', response.message));
      }
    }, error => {
      this.spinner.hide();
      this._notificationService.add(new Notification('error', error));
    });
  }

  /**
   * Remove file from selectedFiles list
   * @param fileName
   */
  fileRemove(fileName): void {
    this.mediaFile.nativeElement.value = "";
    this.selectedFiles = this.selectedFiles.filter(({ name }) => name !== fileName);
  }

  /**
   * Delete Refrigerator file by fileId
   * @param fileId
   */
  removeMediaImage(fileId) {
    this.deleteAlertDataModel = {
      title: "Delete File",
      message: this._appConstant.msgConfirm.replace('modulename', "Permission File"),
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
        this.spinner.show();
        this._service.removePermissionFile(this.fleetGuid, fileId).subscribe(response => {
          this.spinner.hide();
          if (response.isSuccess === true) {
            if (this.selectedFilesObj.length <= 1) {
              this.fleetObject['fleetPermissionFiles'] = null;
              this.fleetForm.get('permissionFiles').setValue('');
            }
            this.selectedFilesObj = this.selectedFilesObj.filter(({ guid }) => guid !== fileId);
            this._notificationService.add(new Notification('success', this._appConstant.msgDeleted.replace("modulename", "Permission File")));
          } else {
            this._notificationService.add(new Notification('error', response.message));
          }
        }, error => {
          this.spinner.hide();
          this._notificationService.add(new Notification('error', error));
        });
      }
    });
  }


  /**
   * Handle media files
   * @param event
   */
  handleMediaFileInput(event) {
    //this.selectedFiles = [];
    const fileList: FileList = event.target.files;
    for (let x = 0; x < fileList.length; x++) {
      if (event.target.files[x]) {
        let fileType = fileList.item(x).name.split('.');
        let fileTypes = ['doc', 'DOC', 'docx', 'DOCX', 'pdf', 'PDF'];
        if (fileTypes.indexOf(fileType[fileType.length - 1]) !== -1) {
          this.selectedFiles.push(event.target.files[x]);
        } else {
          this.checkSubmitStatus = false;
          //this.selectedFiles = [];
          this.MessageAlertDataModel = {
            title: "Permission File",
            message: "Invalid File Type.",
            message2: "Upload .doc, .docx, .pdf file Only.",
            okButtonName: "OK",
          };
          const dialogRef = this.dialog.open(MessageDialogComponent, {
            width: '400px',
            height: 'auto',
            data: this.MessageAlertDataModel,
            disableClose: false
          });
          return;
        }
      }

    }
    if (event.target.files && event.target.files[0]) {
      var reader = new FileReader();
      reader.readAsDataURL(event.target.files[0]);
      reader.onload = (innerEvent: any) => {
        //this.fileUrl = innerEvent.target.result;
      }
    }
  }

  /**
   * Get fleet details by fleetGuid
   * @param fleetGuid
   */
  getFleetDetails(fleetGuid) {
    this.spinner.show();
    this._service.getFleetDetails(fleetGuid).subscribe(response => {
      if (response.isSuccess === true) {
        this.fleetObject = response.data;
        this.selectedFilesObj = this.fleetObject.fleetPermissionFiles;
        this.radius = this.fleetObject.radius;
        this.latitude = +this.fleetObject.latitude;
        this.longitude = +this.fleetObject.longitude;
        this.fleetObject.image = this.mediaUrl + this.fleetObject.image;
        this.currentImage = this.fleetObject.image;
        this.fleetForm.get('permissionFiles').setValue(this.fleetObject.fleetPermissionFiles);

        this.fleetForm.get('location').setValue(true);
        
        if (this.fleetObject.devices[0].templateGuid) {
          
          this.getDeviceLookup(this.fleetObject.devices[0].templateGuid, this.fleetObject.devices[0].deviceGuid);
        }

        this.fleetForm.get('permissionFiles').setValue(true);

        this.fleetForm.get('imageFile').setValue(true);

        var google_map_pos = new google.maps.LatLng(this.latitude, this.longitude);

        /* Use Geocoder to get address */
        var google_maps_geocoder = new google.maps.Geocoder();
        google_maps_geocoder.geocode(
          { 'latLng': google_map_pos },
          function (results, status) {
            if (status == google.maps.GeocoderStatus.OK && results[0]) {
              //this.currentAddress = results[0].formatted_address;
              document.getElementById('search')['value'] = results[0].formatted_address;
            }
          }
        );
        
        if (this.isView)
          this.fleetForm.disable();
      }
      else {
        this._notificationService.add(new Notification('error', response.message));
      }
      this.spinner.hide();
    }, error => {
      this.spinner.hide();
      this._notificationService.add(new Notification('error', error));
    });
  }
}
