import { Component, OnInit, ViewEncapsulation } from '@angular/core'
import { TitleCasePipe } from '@angular/common'

@Component({
	encapsulation: ViewEncapsulation.None,
	selector: 'app-login-header',
	templateUrl: './login-header.component.html',
	styleUrls: ['./login-header.css'],
	providers: [TitleCasePipe]
})

export class LoginHeaderComponent implements OnInit {
	cookieName = 'FM';
	login:any;
	constructor(
	) { }

	ngOnInit() {
	}


}