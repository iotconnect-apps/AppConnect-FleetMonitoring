import { Component, OnInit } from '@angular/core'
import { FormControl, FormGroup, Validators } from '@angular/forms'
import { ActivatedRoute, Router } from '@angular/router'
import { NgxSpinnerService } from 'ngx-spinner'


@Component({
	selector: 'app-resetpassword',
	templateUrl: './resetpassword.component.html',
	styleUrls: ['./resetpassword.component.css']
})

export class ResetpasswordComponent implements OnInit {
	resetPasswordForm: FormGroup;
	checkSubmitStatus = false;
	invitationGuid = '';

	constructor(
		private spinner: NgxSpinnerService,
		public router: Router,
		private activatedRoute: ActivatedRoute
	) {
		this.activatedRoute.params.subscribe(params => {
			if (params.invitationGuid) {
				this.invitationGuid = params.invitationGuid;
			}
		});
	}

	ngOnInit() {
		
	}

}