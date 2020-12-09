import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class ApiConfigService {

  static settings: IAppConfig;

  constructor(private http: HttpClient) { }
  load() {

    const jsonFile = `assets/config.json`;
    return new Promise<void>((resolve, reject) => {
      this.http.get(jsonFile).toPromise().then((response: IAppConfig) => {
        ApiConfigService.settings = <IAppConfig>response;
        resolve();
      }).catch((response: any) => {
        reject(`Could not load the config file`);
      });
    });
  }
}
export interface IAppConfig {
  env: {
    name: string
  }
  apiServer: {
    baseUrl: string,
  },
   IdsServer: {
    IDSAuthority: string,
    IDSClientID: string,
    IDSScope: string,
    IDSArcValue: string,
    webUrl: string,
  }
}


