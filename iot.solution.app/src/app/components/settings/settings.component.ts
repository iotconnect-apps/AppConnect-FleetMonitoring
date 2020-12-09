import { Component, OnInit } from '@angular/core'
import { ActivatedRoute } from '@angular/router'
import { NgxSpinnerService } from 'ngx-spinner'

import { SettingsService } from './../../services/index'

@Component({
	selector: 'app-settings',
	templateUrl: './settings.component.html',
	styleUrls: ['./settings.component.css']
})

export class SettingsComponent implements OnInit {
	googleConnected = false;
	officeConnected = false;
	constructor(
		private settingsService: SettingsService,
		private spinner: NgxSpinnerService,
		private activatedRoute: ActivatedRoute
	) { }

	ngOnInit() {
	}


}