import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router'
import { FormControl, FormGroup, Validators } from '@angular/forms'
import { NgxSpinnerService } from 'ngx-spinner'
import { Notification, NotificationService, RolesService } from 'app/services';
import { AppConstant, StatusList } from 'app/app.constants';


@Component({
  selector: 'app-roles-add',
  templateUrl: './roles-add.component.html',
  styleUrls: ['./roles-add.component.css']
})
export class RolesAddComponent implements OnInit {

  fileUrl: any;
	fileName = '';
	fileToUpload: any = null;
	status;
  moduleName = "Add Role";
  buttonName = "Submit"
	roleObject = {};
	deviceGuid = '';
	isEdit = false;
	roleForm: FormGroup;
  checkSubmitStatus = false;
  statusList: StatusList[] = [
		{
			id: true,
			status: 'Active'
		},
		{
			id: false,
			status: 'In-active'
		}
		
  ];
  


  constructor(
    //private deviceService: DeviceService,
    private router: Router,
    //private _notificationService: NotificationService,
    private activatedRoute: ActivatedRoute,
    private spinner: NgxSpinnerService,
    private rolesService : RolesService,
    private _notificationService: NotificationService,
    public _appConstant : AppConstant
  ) {
    this.createFormGroup();
    this.activatedRoute.params.subscribe(params => {
      if (params.deviceGuid != 'add') {
        this.getRoleDetails(params.deviceGuid);
        this.deviceGuid = params.deviceGuid;
        this.moduleName = "Edit Role";
        this.buttonName = "Update";
        this.isEdit = true;
      } else {
				this.roleObject = {}
      }
    });
  }

  ngOnInit() {
  //  this.getTemplateLookup();
  }

  createFormGroup() {
    this.roleForm = new FormGroup({
			name: new FormControl('', [Validators.required, Validators.minLength(4)]),
			description: new FormControl('', [Validators.required, Validators.minLength(4)]),
      isActive: new FormControl('', [Validators.required]),
  	});
  }

  
  addRole() {
    this.checkSubmitStatus = true;
    if (this.isEdit) {
      this.roleForm.patchValue({"isActive" : 	this.roleObject['isActive']});
    }else{
      this.roleForm.patchValue({"isActive" : true});
    }
    if (this.roleForm.status === "VALID") {
      this.spinner.show();
      let successMessage = this._appConstant.msgCreated.replace("modulename","Role");
      if (this.isEdit) {
        this.roleForm.registerControl("guid", new FormControl(''));
        this.roleForm.patchValue({"guid" : this.deviceGuid});
        successMessage = this._appConstant.msgUpdated.replace("modulename","Role");
      }
      this.rolesService.addUpdateRole(this.roleForm.value).subscribe( response => {
        this.spinner.hide();
        if (response.isSuccess === true) {
          this.router.navigate(['/roles']);
          this._notificationService.add(new Notification('success', successMessage));
        }
        else {
          this._notificationService.add(new Notification('error', response.message));
        }
      }, error => {
        this.spinner.hide();
        this._notificationService.add(new Notification('error', error ));
      });
    }
  }

  getRoleDetails(deviceGuid) {
    this.spinner.show();
    this.rolesService.getRoleDetails(deviceGuid).subscribe(response => {
    	this.spinner.hide();
    	if (response.isSuccess === true) {
      		this.roleObject = response.data;
    	} else {
        this._notificationService.add(new Notification('error', response.message));
      }
    }, error => {
      this.spinner.hide();
      this._notificationService.add(new Notification('error', error ));
    });
  }

}
