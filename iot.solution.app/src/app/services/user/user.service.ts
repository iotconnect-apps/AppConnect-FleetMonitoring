import 'rxjs/add/operator/map'

import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http'
import { CookieService } from 'ngx-cookie-service'
import { ApiConfigService, NotificationService } from 'app/services';



@Injectable({
  providedIn: 'root'
})

export class UserService {
  protected apiServer = ApiConfigService.settings.apiServer;
  cookieName = 'FM';
  constructor(private cookieService: CookieService,
    private httpClient: HttpClient,
    private _notificationService: NotificationService) {
    this._notificationService.apiBaseUrl = this.apiServer.baseUrl;
  }

  getUserDetailByToken(token) {
    const parameter = {
      "token": token
    }
    return this.httpClient.post<any>(this.apiServer.baseUrl + 'api/account/identity', parameter).map(response => {
      return response;
    });
  }

  getUserlist(parameters) {
    const parameter = {
      'pageNo': parameters.pageNo,
      'pageSize': parameters.pageSize,
      'searchText': parameters.searchText,
      'orderBy': parameters.orderBy
    };

    return this.httpClient.post<any>(this.apiServer.baseUrl + 'api/user/search', parameter).map(response => {
      return response;
    });
  }

  getUserDetails(userGuid) {
    let currentUser = JSON.parse(localStorage.getItem('currentUser'));
    let isAdmin = currentUser.userDetail.isAdmin;
    if (isAdmin) {
      return this.httpClient.get<any>(this.apiServer.baseUrl + 'api/adminuser/' + userGuid).map(response => {
        return response;
      });
    } else {
      return this.httpClient.get<any>(this.apiServer.baseUrl + 'api/user/' + userGuid).map(response => {
        return response;
      });
    }
  }

  deleteUser(userGuid) {

    return this.httpClient.put<any>(this.apiServer.baseUrl + 'api/user/delete?id=' + userGuid, "").map(response => {
      return response;
    });
  }
  // login
  login(data) {

    return this.httpClient.post<any>(this.apiServer.baseUrl + 'api/account/login', data).map(response => {
      if (response.isSuccess === true && response.data.access_token) {
        // store user details and jwt token in the local storage to keep the user logged in between page refreshes
        localStorage.setItem('currentUser', JSON.stringify(response.data));
      }
      return response;
    });

  }

  // logout
  logout() {
    // remove user from the local storage
    localStorage.removeItem('currentUser');
  }

  getroleList() {

    return this.httpClient.get<any>(this.apiServer.baseUrl + 'api/role/company').map(response => {
      return response;
    });
  }

  getTimezoneList() {


    return this.httpClient.get<any>(this.apiServer.baseUrl + 'api/Lookup/timezone').map(response => {
      return response;
    });
  }

  manageUser(data) {

    return this.httpClient.post<any>(this.apiServer.baseUrl + 'api/user/manage', data).map(response => {
      return response;
    });
  }

  changeStatus(id, isActive) {
    let status = isActive == true ? false : true;
    return this.httpClient.post<any>(this.apiServer.baseUrl + 'api/user/updatestatus?id=' + id + '&status=' + status, {}).map(response => {
      return response;
    });
  }

	/**
	 * Change password of a user
	*/
  changePassword(data) {
    return this.httpClient.post<any>(this.apiServer.baseUrl + 'api/account/changepassword', data).map(response => {
      return response;
    });
  }

	/**
 * Change password of a user
*/
  changeAdminPassword(data) {
    return this.httpClient.put<any>(this.apiServer.baseUrl + 'api/adminuser/changepassword', data).map(response => {
      return response;
    });
  }

  // adminlogin
  loginadmin(data) {

    return this.httpClient.post<any>(this.apiServer.baseUrl + 'api/account/adminlogin', data).map(response => {
      if (response.isSuccess === true && response.data.access_token) {
        // store user details and jwt token in the local storage to keep the user logged in between page refreshes
        localStorage.setItem('currentUser', JSON.stringify(response.data));
        // api/account/adminlogin
        this.cookieService.set(this.cookieName + 'access_token', response.data.access_token);
        this.cookieService.set(this.cookieName + 'company_id', response.data.userDetail.companyId);
        // get(this.cookieName + 'access_token')
      }
      return response;
    });

  }
  addadminUser(data) {
    var configHeader = {
      headers: {
        'Content-Type': 'application/json',
        'Authorization': 'Bearer ' + this.cookieService.get(this.cookieName + 'access_token'),
        'company-id': this.cookieService.get(this.cookieName + 'company_id')
      }
    };
    return this.httpClient.post<any>(this.apiServer.baseUrl + 'api/adminuser/manage', data, configHeader).map(response => {
      return response;
    });
  }
  getadminUserDetails(userGuid) {

    return this.httpClient.get<any>(this.apiServer.baseUrl + 'api/adminuser/' + userGuid).map(response => {
      return response;
    });
  }
  deleteadminUser(userGuid) {

    var configHeader = {
      headers: {
        'Content-Type': 'application/json',
        'Authorization': 'Bearer ' + this.cookieService.get(this.cookieName + 'access_token'),
        'company-id': this.cookieService.get(this.cookieName + 'company_id')
      }
    };

    return this.httpClient.put<any>(this.apiServer.baseUrl + 'api/adminuser/delete?id=' + userGuid, configHeader).map(response => {
      return response;
    });
  }
  adminchangeStatus(id, isActive) {
    let status = isActive == true ? false : true;
    return this.httpClient.put<any>(this.apiServer.baseUrl + 'api/adminuser/updatestatus/' + id + '/' + status, {}).map(response => {
      return response;
    });
  }
  getAdminUserlist(parameters) {
    let currentUser = JSON.parse(localStorage.getItem('currentUser'));
    var configHeader = {
      headers: {
        'Content-Type': 'application/json',
      }
    };

    const parameter = {
      params: {
        'pageNo': parameters.pageNumber + 1,
        'pageSize': parameters.pageSize,
        'searchText': parameters.searchText,
        'orderBy': parameters.sortBy
      },
      timestamp: Date.now()
    };
    var reqParameter = Object.assign(parameter, configHeader);

    return this.httpClient.post<any>(this.apiServer.baseUrl + 'api/adminuser/list', reqParameter).map(response => {
      return response;
    });
  }

}
