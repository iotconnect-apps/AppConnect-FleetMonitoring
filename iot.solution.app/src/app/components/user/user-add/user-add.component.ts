import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router'
import { FormControl, FormGroup, Validators, FormBuilder } from '@angular/forms'
import { NgxSpinnerService } from 'ngx-spinner'
import { UserService, NotificationService, Notification, LookupService } from '../../../services';
import { CustomValidators } from '../../../helpers/custom.validators';
import { AppConstant } from '../../../app.constants';

@Component({
  selector: 'app-user-add',
  templateUrl: './user-add.component.html',
  styleUrls: ['./user-add.component.css']
})
export class UserAddComponent implements OnInit {
  public contactNoError: boolean = false;
  public mask = {
    guide: true,
    showMask: false,
    keepCharPositions: true,
    mask: ['(', /[0-9]/, /\d/, ')', '-', /\d/, /\d/, /\d/, /\d/, /\d/, /\d/, /\d/, /\d/, /\d/, /\d/]
  };
  locationList = [];
  currentUser = JSON.parse(localStorage.getItem("currentUser"));
  zoneList = [];
  moduleName = "Add User";
  userObject = {};
  userGuid = '';
  isEdit = false;
  userForm: FormGroup;
  checkSubmitStatus = false;
  roleList = [];
  buttonName = 'Submit'
  timeZoneList = [];
  zoneListParameters = { pageNo: 0, pageSize: -1, searchText: "", orderBy: "", parentEntityGuid: "" };


  constructor(
    private formBuilder: FormBuilder,
    private router: Router,
    private _notificationService: NotificationService,
    private activatedRoute: ActivatedRoute,
    private spinner: NgxSpinnerService,
    public userService: UserService,
    public lookupServices: LookupService,
    public _appConstant: AppConstant
  ) { }

  ngOnInit() {
    this.activatedRoute.params.subscribe(params => {
      if (params.userGuid != 'add') {
        this.getUserDetails(params.userGuid);
        this.userGuid = params.userGuid;
        this.moduleName = "Edit User";
        this.isEdit = true;
        this.buttonName = 'Update';
      } else {
        this.userObject = { firstName: '', entityGuid: "", roleGuid: "", lastName: '', email: '', contactNo: '', timezoneGuid: '', isActive: '', isDeleted: "" };
      }
      this.createFormGroup();
      this.getLocation();
    });
  }

	/**
	 * This method is used to get the list of Location's
	 */
  getLocation() {
    this.spinner.show();

    this.lookupServices.getLocationlookup(this.currentUser.userDetail.companyId).subscribe(response => {
      this.spinner.hide();
      if (response.isSuccess) {
        this.locationList = response.data;

      } else {
        this._notificationService.add(new Notification('error', response.message));
      }
      this.getTimezoneList();
      this.getRoleList();
    }, error => {
      this.spinner.hide();
      this._notificationService.add(new Notification('error', error));
      this.getRoleList();
      this.getTimezoneList();
    });
  }

	/**
	 * This method is being used to create the form to create/update user's data. 
	 */
  createFormGroup() {
    this.userForm = this.formBuilder.group({
      firstName: ['', Validators.required],
      lastName: ['', Validators.required],
      email: ['', [Validators.required, Validators.pattern(/^(([^<>()\[\]\.,;:\s@\"]+(\.[^<>()\[\]\.,;:\s@\"]+)*)|(\".+\"))@(([^<>()[\]\.,;:\s@\"]+\.)+[^<>()[\]\.,;:\s@\"]{2,})$/)]],
      contactNo: ['', Validators.required],
      entityGuid: [''],
      // locationGuid: [''],
      isActive: ['', Validators.required],
      isDeleted: ['',],
      roleGuid: ['', Validators.required],
      timeZoneGuid: ['', Validators.required]
    }, {
      validators: CustomValidators.checkPhoneValue('contactNo')
    });
  }

	/**
	 * This method will get the list of roles that can be used to assign to a particular user.
	 */
  getRoleList() {
    this.spinner.show();
    this.lookupServices.getsensor
    this.userService.getroleList().subscribe(response => {
      this.spinner.hide();
      this.roleList = response.data;
    });
  }

	/**
	 * This will get the list of timeZone 
	 */
  getTimezoneList() {
    this.spinner.show();
    this.userService.getTimezoneList().subscribe(response => {
      this.spinner.hide();
      this.timeZoneList = response.data;
    });
  }

	/**
	 * The method creates as well as update the data value of any user.
	 */
  manageUser() {
    this.checkSubmitStatus = true;
    let contactNo = this.userForm.value.contactNo.replace("(", "")
    let contactno = contactNo.replace(")", "")
    let finalcontactno = contactno.replace("-", "")
    if (finalcontactno.match(/^0+$/)) {
      this.contactNoError = true;
      return
    } else {
      this.contactNoError = false;
    }
    if (this.isEdit) {
      this.userForm.registerControl("id", new FormControl(''));
      this.userForm.patchValue({ "id": this.userGuid });
      this.userForm.get('isActive').setValue(this.userObject['isActive']);
      this.userForm.get('isDeleted').setValue(this.userObject['isDeleted']);
    }
    else {
      this.userForm.get('isActive').setValue(true);
      this.userForm.get('isDeleted').setValue(false);
    }

    this.userForm.get('entityGuid').setValue(this.currentUser.userDetail.entityGuid);

    if (this.userForm.status === "VALID") {
      this.spinner.show();
      let successMessage = this._appConstant.msgCreated.replace("modulename", "User");
      if (this.isEdit) {
        successMessage = this._appConstant.msgUpdated.replace("modulename", "User");
      }
      this.userForm.get('contactNo').setValue(contactno);
      let submitData = this.userForm.value;
      this.userService.manageUser(submitData).subscribe(response => {
        if (response.isSuccess === true) {
          this.spinner.hide();
          this.router.navigate(['/users']);
          this._notificationService.add(new Notification('success', successMessage));
        } else {
          this.spinner.hide();
          this._notificationService.add(new Notification('error', response.message));
        }
      }, error => {
        this.spinner.hide();
        this._notificationService.add(new Notification('error', error));
      });
    }
  }

	/**
	 * Get the user's information to update
	 * @param userGuid Unique GUID of user
	 */
  getUserDetails(userGuid) {
    this.spinner.show();
    this.userService.getUserDetails(userGuid).subscribe(response => {
      if (response.isSuccess === true) {
        this.userObject = response.data;
        this.userObject['entityGuid'] = response.data.entityGuid.toUpperCase();
        this.userForm.get("entityGuid").setValue(this.userObject['entityGuid']);
        this.userObject['timezoneGuid'] = response.data.timezoneGuid.toUpperCase();
        this.userForm.get("timeZoneGuid").setValue(this.userObject['timezoneGuid']);
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
	 * MRTHOD NOT IN USE
	 * @param val 
	 */
  getdata(val) {
    return val = val.toLowerCase();
  }
}
