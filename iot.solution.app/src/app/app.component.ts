import * as _ from 'lodash'
import { Title } from '@angular/platform-browser'
import { Component, Renderer2 } from '@angular/core'
import {
	Event as RouterEvent, NavigationCancel, NavigationEnd, NavigationError, NavigationStart,
	Router, ActivatedRoute
} from '@angular/router'
import { ConfigService } from './services/config/config.service';
import { ReactiveFormConfig } from '@rxweb/reactive-form-validators';


@Component({
	selector: 'app-root',
	templateUrl: './app.component.html',
	styleUrls: ['./app.component.css']
})

export class AppComponent {
	loading: boolean = true;
	isLoginURL = false;
	loginURL = '';
	previousUrl: string;

	isAdminLoading = false;
	isAdmin = true;
	checkedAfterLogin = false;

	constructor(
		private renderer: Renderer2,
		private router: Router,
		private titleService: Title,
		private configService: ConfigService) {

		//this is used for to configure validation message globally. https://www.rxweb.io/api/reactive-form-config
		ReactiveFormConfig.set({
			"internationalization": {
				"dateFormat": "dmy",
				"seperator": "/"
			},
			"validationMessage": {
				"alpha": "Only alphabelts are allowed.",
				"alphaNumeric": "Only alphabet and numbers are allowed.",
				"compare": "inputs are not matched.",
				"contains": "value is not contains in the input",
				"creditcard": "creditcard number is not correct",
				"digit": "Only digit are allowed",
				"email": "email is not valid",
				"greaterThanEqualTo": "please enter greater than or equal to the joining age",
				"greaterThan": "please enter greater than to the joining age",
				"hexColor": "please enter hex code",
				"json": "please enter valid json",
				"lessThanEqualTo": "please enter less than or equal to the current experience",
				"lessThan": "please enter less than or equal to the current experience",
				"lowerCase": "Only lowercase is allowed",
				"maxLength": "maximum length is 10 digit",
				"maxNumber": "enter value less than equal to 3",
				"minNumber": "enter value greater than equal to 1",
				"password": "please enter valid password",
				"pattern": "please enter valid zipcode",
				"range": "please enter age between 18 to 60",
				"required": "this field is required",
				"time": "Only time format is allowed",
				"upperCase": "Only uppercase is allowed",
				"url": "Only url format is allowed",
				"zipCode": "enter valid zip code",
				"minLength": "minimum length is 10 digit"
			}
		});
		// Below will result in MyComponent
		let routesArray = ['forgotPassword', 'login', 'resetpassword'];
		this.router.events.subscribe((event: RouterEvent) => {
			this.navigationInterceptor(event);
			if (event instanceof NavigationStart) {
				if (this.previousUrl) {
					this.renderer.removeClass(document.body, this.previousUrl);
				}
				let currentUrlSlug = event.url.slice(1)
				if (!localStorage.getItem('currentUser')) {
					currentUrlSlug = "login"
				}
				if (currentUrlSlug) {
					this.renderer.addClass(document.body, currentUrlSlug);
				}
				this.previousUrl = currentUrlSlug;
			}

			if (event instanceof NavigationEnd) {
				var title = this.getTitle(this.router.routerState, this.router.routerState.root);
				if (title[0]) {
					this.titleService.setTitle('Smart Fleet Monitoring' + ' - ' + title[0]);
				} else {
					this.titleService.setTitle('Smart Fleet Monitoring ');
				}

				this.loginURL = event.url;
				let newRoute = _.toString(_.split(this.loginURL, '/')[1]) || '/';
				if (routesArray.indexOf(newRoute) > -1) {
					this.isLoginURL = false;
					this.checkedAfterLogin = false;
				} else {
					this.isLoginURL = true;
				}
				if (!localStorage.getItem('currentUser')) {
					this.isLoginURL = false;
					this.checkedAfterLogin = false;
				}
			}
		});


	}
	getTitle(state, parent) {
		var data = [];
		if (parent && parent.snapshot.data && parent.snapshot.data.title) {
			data.push(parent.snapshot.data.title);
		}

		if (state && parent) {
			data.push(... this.getTitle(state, state.firstChild(parent)));
		}
		return data;
	}

	// Shows and hides the loading spinner during RouterEvent changes
	navigationInterceptor(event: RouterEvent): void {
		// if (event instanceof NavigationStart) {
		// 	this.loading = true;
		// }
		// if (event instanceof NavigationEnd) {
		// 	this.loading = false;
		// }

		// // Set loading state to false in both of the below events to hide the spinner in case a request fails
		// if (event instanceof NavigationCancel || event instanceof NavigationError) {
		// 	this.loading = false;
		// }
		// // if (event instanceof NavigationError) {
		// // 	this.loading = false;
		// // }
	}

}
