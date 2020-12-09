/// <reference types="@types/googlemaps" />
import { Component, OnInit, ViewChild, ElementRef, NgZone, ViewChildren, QueryList } from '@angular/core';
import { FormGroup, Validators, FormControl, FormArray } from '@angular/forms';
import { LookupService, NotificationService, Notification } from '../../../services';
import { Router, ActivatedRoute } from '@angular/router';
import { NgxSpinnerService } from 'ngx-spinner';
import { MessageAlertDataModel, DeleteAlertDataModel, AppConstant } from '../../../app.constants';
import { MatDialog } from '@angular/material';
import { TripService } from '../../../services/trip/trip.service';
import { MessageDialogComponent, DeleteDialogComponent } from '../..';
import * as moment from 'moment';
import { MapsAPILoader, AgmMap } from '@agm/core';
import { tripStopObj } from './tripStop-model';
declare const google: any;

@Component({
  selector: 'app-add-trips',
  templateUrl: './add-trips.component.html',
  styleUrls: ['./add-trips.component.css']
})
export class AddTripsComponent implements OnInit {
  @ViewChild('mediaFile', { static: false }) mediaFile: ElementRef;
  @ViewChildren('searchTripStop') public searchTripStopElementRef: QueryList<ElementRef>;
  @ViewChild('searchSourceLocation', { static: true }) public searchSourceLocationElementRef: ElementRef;
  @ViewChild('searchDestinationLocation', { static: true }) public searchDestinationLocationElementRef: ElementRef;

  //public latitude: number;
  //public longitude: number;
  public sourceLatitude: number;
  public sourceLongitude: number;
  public destinationLatitude: number;
  public destinationLongitude: number;
  sourceAddress: any;
  destinationAddress: any;
  public sourceControl: FormControl;
  public destinationControl: FormControl;
  moduleName = "Add Trip";
  buttonname = "Submit";
  MessageAlertDataModel: MessageAlertDataModel;
  deleteAlertDataModel: DeleteAlertDataModel;
  selectedFiles: any = [];
  selectedFilesObj: any = [];
  isFilterShow: boolean = false;
  buttonName = "Submit";
  tripForm: FormGroup;
  public endDateValidate: any;
  materialTypeList: any = [];
  fleetList: any = [];
  tripObject: any = {};
  tripGuid: any;
  checkSubmitStatus = false;
  checkTripStopSubmitStatus = false;
  isEdit = false;
  isTripStop = false;
  today: any;
  minDate: any;
  fleetGuid: any = null;
  autocompleteSource: any;
  autocompleteDestination: any;
 
  tripStopForm = new FormGroup({
    guid: new FormArray([]),
    stopName: new FormArray([]),
    latitude: new FormArray([]),
    longitude: new FormArray([]),
    endDateTime: new FormArray([]),
    index: new FormArray([])
  });

  get guid(): FormArray {
    return this.tripStopForm.get('guid') as FormArray;
  }

  get stopName(): FormArray {
    return this.tripStopForm.get('stopName') as FormArray;
  }

  get latitude(): FormArray {
    return this.tripStopForm.get('latitude') as FormArray;
  }

  get longitude(): FormArray {
    return this.tripStopForm.get('longitude') as FormArray;
  }

  get endDateTime(): FormArray {
    return this.tripStopForm.get('endDateTime') as FormArray;
  }

  get index(): FormArray {
    return this.tripStopForm.get('index') as FormArray;
  }

  updateTripStop(tripStops: any) {
    tripStops.endDateTime = moment(tripStops.endDateTime + 'Z').local();
    this.guid.push(new FormControl(tripStops.guid));
    this.latitude.push(new FormControl(tripStops.latitude));
    this.longitude.push(new FormControl(tripStops.longitude));
    this.stopName.push(new FormControl('', Validators.required));
    this.endDateTime.push(new FormControl('', Validators.required));
    this.index.push(new FormControl(''));
    setTimeout(() => {
      var defaultBounds = new google.maps.LatLngBounds(
        new google.maps.LatLng(this.sourceLatitude, this.sourceLongitude),
        new google.maps.LatLng(this.destinationLatitude, this.destinationLongitude));

      var parentdata = document.querySelectorAll('.parent')
      for (var i = 0; i < parentdata.length; i++) {
        let autocomplete = new google.maps.places.Autocomplete(parentdata[i], {
          types: [],
          bounds: defaultBounds,
          strictBounds: true,
        });
      }
    }, 700);
  }

  addTripStop() {
    if (this.tripStopForm.status === "INVALID") {
      this.checkTripStopSubmitStatus = true;
    }
    else {
      this.checkTripStopSubmitStatus = false;
      //if (this.tripObject.sourceLocation != '' && this.tripObject.destinationLocation != '') {
      this.guid.push(new FormControl(''));
      this.stopName.push(new FormControl('', Validators.required));
      this.endDateTime.push(new FormControl({ value: '', disabled: false }, [Validators.required]));
      this.index.push(new FormControl(''));
      setTimeout(() => {
        var defaultBounds = new google.maps.LatLngBounds(
          new google.maps.LatLng(this.sourceLatitude, this.sourceLongitude),
          new google.maps.LatLng(this.destinationLatitude, this.destinationLongitude));

        var parentdata = document.querySelectorAll('.parent');
        for (var i = 0; i < parentdata.length; i++) {
          let autocomplete = new google.maps.places.Autocomplete(parentdata[i], {
            types: [],
            //bounds: defaultBounds,
            //strictBounds: true,
          });
          autocomplete.addListener("place_changed", () => {
            this.ngZone.run(() => {
              //get the place result
              let place: google.maps.places.PlaceResult = autocomplete.getPlace();

              //verify result
              if (place.geometry === undefined || place.geometry === null) {
                return;
              }

              //set latitude, longitude
              this.latitude.push(new FormControl(place.geometry.location.lat()));
              this.longitude.push(new FormControl(place.geometry.location.lng()));
            });
          });
        }
      }, 700);
    }
  }

  removeTripStop(i) {
    this.guid.removeAt(i);
    this.stopName.removeAt(i);
    this.latitude.removeAt(i);
    this.longitude.removeAt(i);
    this.endDateTime.removeAt(i);
    this.index.removeAt(i);
  }

  constructor(
    public lookupService: LookupService,
    private router: Router,
    private spinner: NgxSpinnerService,
    private _notificationService: NotificationService,
    private activatedRoute: ActivatedRoute,
    public dialog: MatDialog,
    public _appConstant: AppConstant,
    public _service: TripService,
    private mapsAPILoader: MapsAPILoader,
    private tripService: TripService,
    private ngZone: NgZone) {
    this.createFormGroup();
    this.activatedRoute.params.subscribe(params => {
      if (params.tripGuid != 'add') {
        this.tripGuid = params.tripGuid;
        this.moduleName = "Edit Trip";
        this.buttonName = "Update";
        this.isEdit = true;
        this.getTripDetails(this.tripGuid);
      }
      else {
        this.getFleetLookup();
        this.addTripStop();
      }
      this.tripObject = { sourceLocation: '', destinationLocation: '', startDateTime: '', materialTypeGuid: '', fleetGuid: '', shipmentFiles: '', tripStops: [new tripStopObj()] }

    });

  }

  ngOnInit() {
    this.sourceControl = new FormControl();
    this.destinationControl = new FormControl();
    this.getMaterialTypeLookup();

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

    //load Places Autocomplete
    this.mapsAPILoader.load().then(() => {
      let autocompleteSource = this.autocompleteSource = new google.maps.places.Autocomplete(this.searchSourceLocationElementRef.nativeElement, {
        types: []
      });

      autocompleteSource.addListener("place_changed", () => {

        this.ngZone.run(() => {
          //get the place result
          let place: google.maps.places.PlaceResult = autocompleteSource.getPlace();

          //verify result
          if (place.geometry === undefined || place.geometry === null) {
            return;
          }

          //set latitude, longitude
          this.sourceLatitude = place.geometry.location.lat();
          this.sourceLongitude = place.geometry.location.lng();
        });
      });

      let autocompleteDestination = this.autocompleteDestination = new google.maps.places.Autocomplete(this.searchDestinationLocationElementRef.nativeElement, {
        types: []
      });

      autocompleteDestination.addListener("place_changed", () => {
        this.ngZone.run(() => {
          //get the place result
          let place: google.maps.places.PlaceResult = autocompleteDestination.getPlace();

          //verify result
          if (place.geometry === undefined || place.geometry === null) {
            return;
          }

          //set latitude, longitude
          this.destinationLatitude = place.geometry.location.lat();
          this.destinationLongitude = place.geometry.location.lng();
        });
      });
    });
  }

  createFormGroup() {
    this.tripForm = new FormGroup({
      tripId: new FormControl('', [Validators.required]),
      sourceLocation: new FormControl('', [Validators.required]),
      sourceLatitude: new FormControl(''),
      sourceLongitude: new FormControl(''),
      destinationLocation: new FormControl('', [Validators.required]),
      destinationLatitude: new FormControl(''),
      destinationLongitude: new FormControl(''),
      totalMiles: new FormControl(''),
      startDateTime: new FormControl({ value: '', disabled: false }, [Validators.required]),
      tripStop: new FormArray([]),
      materialTypeGuid: new FormControl('', [Validators.required]),
      weight: new FormControl('', [Validators.required]),
      fleetGuid: new FormControl('', [Validators.required]),
      shipmentFiles: new FormControl(''),
    });
  }

  /**
 * Get Fleet Lookup
 * */
  getFleetLookup() {
    this.lookupService.getfleetlookup().
      subscribe(response => {
        if (response.isSuccess === true) {
          this.fleetList = response['data'];
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
   * Manage trip with form data
   * */
  manageTrip() {

    this.checkSubmitStatus = true;
    this.checkTripStopSubmitStatus = true;

    this.tripForm.get('sourceLatitude').setValue(this.sourceLatitude);
    this.tripForm.get('sourceLongitude').setValue(this.sourceLongitude);
    this.tripForm.get('destinationLatitude').setValue(this.destinationLatitude);
    this.tripForm.get('destinationLongitude').setValue(this.destinationLongitude);

    if (this.isEdit) {
      this.tripForm.registerControl('guid', new FormControl(''));
      this.tripForm.patchValue({ "guid": this.tripGuid });
    }
    if (this.tripForm.status === "VALID" && this.tripStopForm.status === "VALID") {
      let totlaMiles = this.tripService.calculateTotalMiles(this.sourceLatitude, this.sourceLongitude, this.destinationLatitude, this.destinationLongitude);
      if (totlaMiles > 0) {
        this.tripForm.get('totalMiles').setValue(parseInt(totlaMiles.toString()));
      }
      if (this.selectedFiles.length > 0) {
        this.tripForm.get('shipmentFiles').setValue(this.selectedFiles);
      }
      this.tripForm.get('sourceLocation').setValue(document.getElementById('searchSourceLocation')['value']);
      this.tripForm.get('destinationLocation').setValue(document.getElementById('searchDestinationLocation')['value']);

      this.tripForm.value.tripStop = [];
      for (let i = 0; i < this.stopName.length; i++) {
        this.tripForm.value.tripStop.push({
          guid: this.guid.at(i).value,
          //StopName: this.stopName.at(i).value,
          StopName: document.querySelectorAll('.parent')[i]['value'],
          latitude: this.latitude.at(i).value,
          longitude: this.longitude.at(i).value,
          EndDateTime: moment(this.endDateTime.at(i).value).format('YYYY-MM-DDTHH:mm:ss')
        });
      }
      this.spinner.show();
      this._service.addTrip(this.tripForm.value).subscribe(response => {
        this.spinner.hide();
        if (response.isSuccess === true) {
          if (this.isEdit) {
            this._notificationService.add(new Notification('success', "Trip has been updated successfully."));
          } else {
            this._notificationService.add(new Notification('success', "Trip has been added successfully."));
          }
          this.router.navigate(['/trips']);
        } else {
          this._notificationService.add(new Notification('error', response.message));
        }
      });
    }
  }

  /**
   * Show hide filter
   * */
  showHideFilter() {
    this.isFilterShow = !this.isFilterShow;
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
  * Delete Refrigerator file by fileId
  * @param fileId
  */
  removeMediaImage(fileId) {
    this.deleteAlertDataModel = {
      title: "Delete File",
      message: this._appConstant.msgConfirm.replace('modulename', "Shipment File"),
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
        this._service.removeShipmentFile(this.tripGuid, fileId).subscribe(response => {
          this.spinner.hide();
          if (response.isSuccess === true) {
            this.selectedFilesObj = this.selectedFilesObj.filter(({ guid }) => guid !== fileId);
            this._notificationService.add(new Notification('success', this._appConstant.msgDeleted.replace("modulename", "Shipment File")));
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
   * Remove file from selectedFiles list
   * @param fileName
   */
  fileRemove(fileName): void {
    this.mediaFile.nativeElement.value = "";
    this.selectedFiles = this.selectedFiles.filter(({ name }) => name !== fileName);
  }

  /**
* validate end date using start date change
* @param startdate
*/
  onChangeStartDateNew(startdate) {

    if (startdate > this.endDateValidate) {
      this.tripStopForm.get("endDateTime").reset();
    }
    let date = moment(startdate).format();
    this.endDateValidate = new Date(date);
  }

  onChangeStartDate(startdate) {
    if (this.endDateValidate == "Invalid Date") {
      this.tripStopForm.get("endDateTime").reset();
    }
    let date = moment(startdate).add(this._appConstant.minGap, 'minutes').format();
    this.endDateValidate = new Date(date);
  }

  /**
   * Get fleet details by tripGuid
   * @param tripGuid
   */
  getTripDetails(tripGuid) {
    this.spinner.show();
    this._service.getTripDetails(tripGuid).subscribe(response => {
      if (response.isSuccess === true) {
        this.tripObject = response.data;
        if (this.tripObject.isCompleted == true) {
          this.router.navigate(['/trips']);
        }
        if (this.tripObject) {
          if (this.today > this.tripObject.startDateTime) {

            this.tripObject.startDateTime = moment(this.tripObject.startDateTime + 'Z').local();
            this.today = this.tripObject.startDateTime;
          }
          else {
            this.tripObject.startDateTime = moment(this.tripObject.startDateTime + 'Z').local();
          }
          this.onChangeStartDate(this.today);
        }

        if (response.data.tripStops.length > 0) {
          this.isTripStop = true;
          for (let i = 0; i < response.data.tripStops.length; i++) {
            this.updateTripStop(response.data.tripStops[i]);
          }
        }
        this.fleetGuid = this.tripObject.fleetGuid;
        this.getFleetLookup();
        this.selectedFilesObj = this.tripObject.shipmentFiles;
        this.sourceLatitude = this.tripObject.sourceLatitude;
        this.sourceLongitude = this.tripObject.sourceLongitude;
        this.destinationLatitude = this.tripObject.destinationLatitude;
        this.destinationLongitude = this.tripObject.destinationLongitude;
        if (this.tripObject.selectedFiles) {
          this.tripForm.get('shipmentFiles').setValue(this.tripObject.selectedFiles);
        }
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

  /**
   * Swap latitude longitude addredd of source and destination locations
   * */
  swapLocation() {

    let sourceLat = this.sourceLatitude;
    let sourceLng = this.sourceLongitude;
    let destinationLat = this.destinationLatitude;
    let destinationLng = this.destinationLongitude;

    /* Fetch address */
    this.sourceAddress = document.getElementById('searchSourceLocation')['value'];
    this.destinationAddress = document.getElementById('searchDestinationLocation')['value'];

    /* Swap address */
    document.getElementById('searchSourceLocation')['value'] = this.destinationAddress;
    document.getElementById('searchDestinationLocation')['value'] = this.sourceAddress;

    /* Swap latitude longitude */
    this.sourceLatitude = destinationLat;
    this.sourceLongitude = destinationLng;
    this.destinationLatitude = sourceLat;
    this.destinationLongitude = sourceLng;
  }
}
