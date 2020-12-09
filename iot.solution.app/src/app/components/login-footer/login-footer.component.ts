import { Component, OnInit, ViewEncapsulation } from '@angular/core'
import { TitleCasePipe } from '@angular/common'

@Component({
	selector: 'app-login-footer',
	templateUrl: './login-footer.component.html',
	styleUrls: ['./login-footer.css'],
	providers: [TitleCasePipe]
})

export class LoginFooterComponent implements OnInit {
	cookieName = 'FM';
	currentDate  = new Date();
	constructor(
	) { }

	ngOnInit() {

	}


}