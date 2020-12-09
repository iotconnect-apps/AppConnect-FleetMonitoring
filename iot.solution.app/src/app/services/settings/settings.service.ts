import 'rxjs/add/operator/map'

import { Injectable } from '@angular/core'
import { HttpClient } from '@angular/common/http'
import { CookieService } from 'ngx-cookie-service'

import { ApiConfigService, NotificationService } from 'app/services';
@Injectable({
	providedIn: 'root'
})

export class SettingsService {
	cookieName = 'FM';
	protected apiServer = ApiConfigService.settings.apiServer;
	constructor(
		private cookieService: CookieService,
		private httpClient: HttpClient,
		private _notificationService: NotificationService
	) {
		this._notificationService.apiBaseUrl = this.apiServer.baseUrl;
	}

	getConfigurationData() {
		var configHeader = {
			headers: {
				'Content-Type': 'application/json',
				'Authorization': 'Bearer ' + this.cookieService.get(this.cookieName + 'access_token')
			}
		};

		return this.httpClient.get<any>(this.apiServer.baseUrl + 'api/settings/getConfigurationData', configHeader).map(response => {
			return response;
		});
	}

	getOfficeSignInUrl() {
		var configHeader = {
			headers: {
				'Content-Type': 'application/json',
				'Authorization': 'Bearer ' + this.cookieService.get(this.cookieName + 'access_token')
			}
		};

		return this.httpClient.get<any>(this.apiServer.baseUrl + 'api/settings/getOfficeSignInUrl', configHeader).map(response => {
			return response;
		});
	}

	unRegisterOffice() {
		var configHeader = {
			headers: {
				'Content-Type': 'application/json',
				'Authorization': 'Bearer ' + this.cookieService.get(this.cookieName + 'access_token')
			}
		};

		return this.httpClient.get<any>(this.apiServer.baseUrl + 'api/settings/unRegisterOffice', configHeader).map(response => {
			return response;
		});
	}

	getGoogleSignInUrl() {
		var configHeader = {
			headers: {
				'Content-Type': 'application/json',
				'Authorization': 'Bearer ' + this.cookieService.get(this.cookieName + 'access_token')
			}
		};

		return this.httpClient.get<any>(this.apiServer.baseUrl + 'api/settings/getGoogleSignInUrl', configHeader).map(response => {
			return response;
		});
	}

	unRegisterGoogle() {
		var configHeader = {
			headers: {
				'Content-Type': 'application/json',
				'Authorization': 'Bearer ' + this.cookieService.get(this.cookieName + 'access_token')
			}
		};

		return this.httpClient.get<any>(this.apiServer.baseUrl + 'api/settings/unRegisterGoogle', configHeader).map(response => {
			return response;
		});
	}

	getSettingDetails() {
		var configHeader = {
			headers: {
				'Content-Type': 'application/json',
				'Authorization': 'Bearer ' + this.cookieService.get(this.cookieName + 'access_token')
			}
		};

		return this.httpClient.get<any>(this.apiServer.baseUrl + 'api/settings/getSettingDetails', configHeader).map(response => {
			return response;
		});
	}

	apiIntegrationInfo() {
		var configHeader = {
			headers: {
				'Content-Type': 'application/json',
				'Authorization': 'Bearer ' + this.cookieService.get(this.cookieName + 'access_token')
			}
		};

		return this.httpClient.get<any>(this.apiServer.baseUrl + 'api/settings/apiIntegrationInfo', configHeader).map(response => {
			return response;
		});
	}

	createMeetingInOffice(data) {
		var configHeader = {
			headers: {
				'Content-Type': 'application/json',
				'Authorization': 'Bearer ' + this.cookieService.get(this.cookieName + 'access_token')
			}
		};

		return this.httpClient.post<any>(this.apiServer.baseUrl + 'api/settings/createMeetingInOffice', data, configHeader).map(response => {
			return response;
		});
	}

	createMeetingInGoogle(data) {
		var configHeader = {
			headers: {
				'Content-Type': 'application/json',
				'Authorization': 'Bearer ' + this.cookieService.get(this.cookieName + 'access_token')
			}
		};

		return this.httpClient.post<any>(this.apiServer.baseUrl + 'api/settings/createMeetingInGoogle', data, configHeader).map(response => {
			return response;
		});
	}

	deleteMeetingInGoogle(data) {
		var configHeader = {
			headers: {
				'Content-Type': 'application/json',
				'Authorization': 'Bearer ' + this.cookieService.get(this.cookieName + 'access_token')
			}
		};

		return this.httpClient.post<any>(this.apiServer.baseUrl + 'api/settings/deleteMeetingInGoogle', data, configHeader).map(response => {
			return response;
		});
	}

	deleteMeetingInOffice(data) {
		var configHeader = {
			headers: {
				'Content-Type': 'application/json',
				'Authorization': 'Bearer ' + this.cookieService.get(this.cookieName + 'access_token')
			}
		};

		return this.httpClient.post<any>(this.apiServer.baseUrl + 'api/settings/deleteMeetingInOffice', data, configHeader).map(response => {
			return response;
		});
	}

	syncGoogleEvents(data) {
		var configHeader = {
			headers: {
				'Content-Type': 'application/json',
				'Authorization': 'Bearer ' + this.cookieService.get(this.cookieName + 'access_token')
			}
		};

		return this.httpClient.post<any>(this.apiServer.baseUrl + 'api/settings/syncGoogleEvents', data, configHeader).map(response => {
			return response;
		});
	}

	syncOffice365Events(data) {
		var configHeader = {
			headers: {
				'Content-Type': 'application/json',
				'Authorization': 'Bearer ' + this.cookieService.get(this.cookieName + 'access_token')
			}
		};

		return this.httpClient.post<any>(this.apiServer.baseUrl + 'api/settings/syncOffice365Events', data, configHeader).map(response => {
			return response;
		});
	}
}