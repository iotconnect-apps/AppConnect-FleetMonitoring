import 'rxjs/add/operator/map'
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http'
import { ApiConfigService, NotificationService } from 'app/services';

@Injectable({
	providedIn: 'root'
})

export class RolesService {
	protected apiServer = ApiConfigService.settings.apiServer;
	constructor(
		private httpClient: HttpClient,
		private _notificationService: NotificationService) {
		this._notificationService.apiBaseUrl = this.apiServer.baseUrl;
	}


	getRoles(parameters) {
		const parameter = {
			params: {
				'pageNo': parameters.pageNumber + 1,
				'pageSize': parameters.pageSize,
				'searchText': parameters.searchText,
				'orderBy': parameters.sortBy
			}
		};

		return this.httpClient.post<any>(this.apiServer.baseUrl + 'api/role/search', parameter).map(response => {
			return response;
		});
	}

	addUpdateRole(data) {

		let currentUser = JSON.parse(localStorage.getItem('currentUser'));
		const appendParams = {
			"solutionGuid": currentUser.userDetail.solutionGuid
		};

		var reqParameter = Object.assign(data, appendParams);
		return this.httpClient.post<any>(this.apiServer.baseUrl + 'api/role/manage', reqParameter).map(response => {
			return response;
		});
	}

	getRoleDetails(roleId) {

		return this.httpClient.get<any>(this.apiServer.baseUrl + 'api/role/' + roleId).map(response => {
			return response;
		});
	}

	deleteRole(roleId) {

		return this.httpClient.put<any>(this.apiServer.baseUrl + 'api/role/delete?id=' + roleId, {}).map(response => {
			return response;
		});
	}

	changeStatus(roleId, isActive) {
		let status = isActive == true ? false : true;
		return this.httpClient.post<any>(this.apiServer.baseUrl + 'api/role/updatestatus?id=' + roleId + '&status=' + status, {}).map(response => {
			return response;
		});
	}




}
