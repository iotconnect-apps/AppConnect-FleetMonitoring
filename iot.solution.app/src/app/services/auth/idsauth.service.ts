import { Injectable } from '@angular/core';
import { Log, User, UserManager, UserManagerSettings } from 'oidc-client';
import { Observable } from 'rxjs/Rx';
import { ApiConfigService } from "../api-config/api-config.service";

export function getClientSettings(): UserManagerSettings {

  return {
    authority: ApiConfigService.settings.IdsServer.IDSAuthority,
    client_id: ApiConfigService.settings.IdsServer.IDSClientID,
    redirect_uri: ApiConfigService.settings.IdsServer.webUrl + '/callback',
    post_logout_redirect_uri: ApiConfigService.settings.IdsServer.webUrl + '/login',
    response_type: 'id_token token',
    scope: ApiConfigService.settings.IdsServer.IDSScope,
    filterProtocolClaims: true,
    loadUserInfo: true,
    clockSkew: 86400, // (for 24 hours) = 24 hour  60 minute  60 second
    accessTokenExpiringNotificationTime: 4,
    acr_values: ApiConfigService.settings.IdsServer.IDSArcValue,
  };

}

Log.logger = console;
Log.level = Log.DEBUG;

@Injectable({
  providedIn: 'root'
})

export class IDSAuthService {
  private mgr = new UserManager(getClientSettings());
  private user: User = null;
  browserCallBackURL: string = '';

  constructor(/*private _commonService:CommonService*/) {
    this.mgr.getUser().then(user => {
      this.user = user;
    });

    this.mgr.events.addUserSignedOut(() => {
      this.endSignoutMainWindow(true);
    });

    this.mgr.events.addAccessTokenExpired((_data)=>{
      setTimeout(()=>{
        localStorage.clear();
        this.endSignoutMainWindow(true);
        //this._commonService.toasterErrorMessage("Session Timeout");
      },5000)
    });


    this.mgr.events.addUserLoaded((data) => {

      if (data['state'] != undefined && data['state'] != null && data['state'] != '') {
        this.browserCallBackURL = data['state'];
      }
    });

  }

  isLoggedIn(): boolean {
    return this.user != null && !this.user.expired;
  }

  getClaims(): any {
    return this.user.profile;
  }

  getLogedInUserIDSData(): Observable<User> {
    return Observable.fromPromise(this.mgr.getUser()).map<User, User>((user) => {

      if (user) {
        return user;
      } else {
        return null;
      }
    });
  }

  isLoggedInObs(): Observable<boolean> {

    return Observable.fromPromise(this.mgr.getUser()).map<User, boolean>((user) => {

      this.user = user;
      if (user) {
        return true;
      } else {
        return false;
      }
    });
  }

  endSigninMainWindow(): Observable<User> {
    return Observable.fromPromise(this.mgr.signinRedirectCallback()).map<User, User>((user) => {

      if (user) {
        return user;
      } else {
        return null;
      }
    });
  }

  endSignoutMainWindow(_isReloadPage: boolean = false): Promise<void> {
    return this.mgr.signoutRedirect().then(_user => {
      setTimeout(() => {
        localStorage.removeItem('currentUser');
      },5000)

    }).catch((_err) => { });
  }


  getAuthorizationHeaderValue(): string {
    return `${this.user.token_type} ${this.user.access_token}`;
  }

  startSigninMainWindow(url: string= ''): Promise<void> {
    return this.mgr.signinRedirect({ data: url });
  }

  completeAuthentication(): Promise<void> {
    return this.mgr.signinRedirectCallback().then(user => {
      this.user = user;
    });
  }

}


