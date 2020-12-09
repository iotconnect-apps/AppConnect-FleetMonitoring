import { Injectable } from "@angular/core";
import { HttpInterceptor, HttpRequest, HttpHandler, HttpEvent } from "@angular/common/http";
import { Observable, throwError } from "rxjs";
import { HttpClient } from '@angular/common/http';
import 'rxjs/add/operator/mergeMap';
import 'rxjs/add/operator/catch';
import { Notification, AuthService, NotificationService } from 'app/services';
import { AppConstant } from '../app.constants';
import { Router } from '@angular/router'
import { ThrowStmt } from "@angular/compiler";

/*
The JWT interceptor intercepts the incoming requests from the application/user and adds JWT token to the request's Authorization header, only if the user is logged in.
This JWT token in the request header is required to access the SECURE END API POINTS on the server 
*/

@Injectable()
export class JwtInterceptor implements HttpInterceptor {
  constructor(
    private router: Router,
    private http: HttpClient,
    private _notificationService: NotificationService,
    private _appConstant: AppConstant,
    private authService: AuthService

  ) { }

  intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    // check if the current user is logged in
    // if the user making the request is logged in, he will have JWT token in it's local storage, which is set by Authorization Service during login process
    let currentUser = JSON.parse(localStorage.getItem('currentUser'));
    if (currentUser && currentUser.access_token) {

      // clone the incoming request and add JWT token in the cloned request's Authorization Header
      //var image_url_device = request.url.includes("api/device/manage");
      var image_url_entity = request.url.includes("api/entity/manage");
      var image_url_fleet = request.url.includes("api/fleet/manage");
      var image_url_trip = request.url.includes("api/trip/manage");
      var image_url_driver = request.url.includes("api/driver/manage");
      if (image_url_entity || image_url_fleet || image_url_trip || image_url_driver) {
        request = request.clone({
          setHeaders: {
            Authorization: `Bearer ${currentUser.access_token}`,
           // 'Content-Type': 'multipart/form-data',
            'company-id': currentUser.userDetail.companyId,
          }
        });

      } else {
        request = request.clone({
          setHeaders: {
            Authorization: `Bearer ${currentUser.access_token}`,
            'Content-Type': 'application/json',
            'company-id': currentUser.userDetail.companyId,
          }
        });
      }
    }

    // handle any other requests which went unhandled
    return next.handle(request).catch(err => {
      // add error message
      let error = (err.error) ? ((err.error.Message) ? err.error.Message : this._appConstant.serverErrorMessage) : ((err.statusText) ? err.statusText : this._appConstant.serverErrorMessage);
      if (error == 'Unauthorized' && !this._notificationService.refreshTokenInProgress) {
        if (currentUser.userDetail.isAdmin) {
          this.authService.logout();
          this.router.navigate(['/admin']);
        } else {
         // this._notificationService.refreshTokenInProgress = true;
          //Genrate params for token refreshing
          let params = {
            token: currentUser.refresh_token
          };
          /* token refreshing request
          return this.http.post(this._notificationService.apiBaseUrl + "api/account/refreshtoken", params).flatMap(
            (response: any) => {
              this._notificationService.refreshTokenInProgress = false;
              //If reload successful update tokens
              if (response.isSuccess == true) {
                let currentUser = JSON.parse(localStorage.getItem('currentUser'));
                if (response.data) {
                  currentUser.access_token = response.data.access_token;
                  currentUser.refresh_token = response.data.refresh_token;
                  localStorage.setItem('currentUser', JSON.stringify(currentUser));
                }
                //Clone our fields request and try to resend it
                request = request.clone({
                  setHeaders: {
                    Authorization: `Bearer ${currentUser.access_token}`,
                    'Content-Type': 'application/json',
                    'company-id': currentUser.userDetail.companyId,
                  }
                });

                return next.handle(request).catch((err: any) => {
                  //Catch another error
                  error = (err.error) ? err.error.Message : (err.statusText) ? err.statusText : this._appConstant.serverErrorMessage;
                  return throwError(error);
                });

              } else {
                //Logout from account
                localStorage.removeItem('currentUser');
                this._notificationService.add(new Notification('error', this._appConstant.tokenInValidMessage));
                location.reload(true);
                return;
              }
            }
          ); */
        }
      }
      if (err.status === 401 || !currentUser) {
        // auto logout on unauthorized response
        localStorage.removeItem('currentUser');
        this._notificationService.add(new Notification('error', this._appConstant.unauthorizedMessage));
        location.reload(true);
        return;
      }
      return throwError(error);
    });

  }

}
