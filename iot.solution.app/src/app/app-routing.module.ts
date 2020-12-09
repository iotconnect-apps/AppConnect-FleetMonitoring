import { NgModule } from '@angular/core'
import { RouterModule, Routes } from '@angular/router'

import { SelectivePreloadingStrategy } from './selective-preloading-strategy'
import { PageNotFoundComponent } from './page-not-found.component'
import {
  DynamicDashboardComponent,CallbackComponent,HomeComponent, UserListComponent, UserAddComponent, DashboardComponent,
  LoginComponent, RegisterComponent, MyProfileComponent, ResetpasswordComponent, SettingsComponent,
  ChangePasswordComponent, AdminLoginComponent, SubscribersListComponent,   UserAdminListComponent, AdminUserAddComponent, AdminDashboardComponent, SubscriberDetailComponent,
   RolesListComponent, RolesAddComponent, AlertsComponent,
  MaintenanceListComponent, ScheduleMaintenanceComponent, 
  FleetComponent, AddFleetComponent, DashboardFleetComponent, TripsComponent, AddTripsComponent, DashboardTripsComponent, DeviceComponent, AddDeviceComponent,
  AddDriverComponent, DriverComponent, DriverDashboardComponent
} from './components/index';


import { AuthService, AdminAuthGuard } from './services/index';

const appRoutes: Routes = [
  {
    path: 'admin',
    children: [
      {
        path: '',
        component: AdminLoginComponent
      },
      {
        path: 'dashboard',
        component: AdminDashboardComponent,
        canActivate: [AuthService]
      },
      {
        path: 'subscribers/:email/:productCode/:companyId',
        component: SubscriberDetailComponent,
        canActivate: [AuthService]
      },
      {
        path: 'subscribers',
        component: SubscribersListComponent,
        canActivate: [AuthService]
      },
      {
        path: 'users',
        component: UserAdminListComponent,
        canActivate: [AuthService]
      },
      {
        path: 'users/adduser',
        component: AdminUserAddComponent,
        canActivate: [AuthService]
      },
      {
        path: 'users/:userGuid',
        component: AdminUserAddComponent,
        canActivate: [AuthService]
      },

    ]
  },
  {
    path: '',
    component: HomeComponent
  },
  {
    path: 'callback',
    component: CallbackComponent
  },
  {
    path: 'login',
    component: LoginComponent
  },
  {
    path: 'register',
    component: RegisterComponent
  },
  //App routes goes here
  {
    path: 'my-profile',
    component: MyProfileComponent,
    //canActivate: [AuthService]
  },
  {
    path: 'change-password',
    component: ChangePasswordComponent,
    //canActivate: [AuthService]
  },
  {
    path: 'dashboard',
    component: DashboardComponent,
    canActivate: [AdminAuthGuard]
  },
  {
    path: 'fleet',
    component: FleetComponent,
    canActivate: [AdminAuthGuard]
  },
  {
    path: 'fleet/:fleetGuid',
    component: AddFleetComponent,
    canActivate: [AdminAuthGuard]
  },
  {
    path: 'fleet/add',
    component: AddFleetComponent,
    canActivate: [AdminAuthGuard]
  },
  {
    path: 'drivers/:driverGuid',
    component: AddDriverComponent,
    canActivate: [AdminAuthGuard]
  },
  {
    path: 'drivers/dashboard/:driverGuid',
    component: DriverDashboardComponent,
    canActivate: [AdminAuthGuard]
  },
  {
    path: 'drivers',
    component: DriverComponent,
    canActivate: [AdminAuthGuard]
  },
  {
    path: 'fleet/dashboard/:fleetGuid',
    component: DashboardFleetComponent,
    canActivate: [AdminAuthGuard]
  },
  {
    path: 'devices',
    component: DeviceComponent,
    canActivate: [AdminAuthGuard]
  },
  {
    path: 'devices/:deviceGuid',
    component: AddDeviceComponent,
    canActivate: [AdminAuthGuard]
  },
  {
    path: 'trips',
    component: TripsComponent,
    canActivate: [AdminAuthGuard]
  },
  {
    path: 'trips/:tripGuid',
    component: AddTripsComponent,
    canActivate: [AdminAuthGuard]
  },
  {
    path: 'trips/add',
    component: AddTripsComponent,
    canActivate: [AdminAuthGuard]
  },
  {
    path: 'trips/dashboard/:tripGuid',
    component: DashboardTripsComponent,
    canActivate: [AdminAuthGuard]
  },
  {
    path: 'maintenance',
    component: MaintenanceListComponent,
    canActivate: [AdminAuthGuard]
  },
  {
    path: 'maintenance/:maintenanceGuid',
    component: ScheduleMaintenanceComponent,
    canActivate: [AdminAuthGuard]
  },
  {
    path: 'maintenance/add',
    component: ScheduleMaintenanceComponent,
    canActivate: [AdminAuthGuard]
  },
  {
    path: 'alerts',
    component: AlertsComponent,
    canActivate: [AdminAuthGuard]
  },
  {
    path: 'alerts/location/:entityGuid',
    component: AlertsComponent,
    canActivate: [AdminAuthGuard]
  },
  {
    path: 'alerts/asset/:assetGuid',
    component: AlertsComponent,
    canActivate: [AdminAuthGuard]
  },
  {
    path: 'users/:userGuid',
    component: UserAddComponent,
    canActivate: [AdminAuthGuard]
  }, {
    path: 'users/add',
    component: UserAddComponent,
    canActivate: [AdminAuthGuard]
  }, {
    path: 'users',
    component: UserListComponent,
    canActivate: [AdminAuthGuard]
  },
  {
    path: 'roles/:deviceGuid',
    component: RolesAddComponent,
    canActivate: [AdminAuthGuard]
  }, {
    path: 'roles',
    component: RolesListComponent,
    canActivate: [AdminAuthGuard]
  },
  {
    path: 'dynamic-dashboard',
    component: DynamicDashboardComponent,
    canActivate: [AdminAuthGuard]
  },
  {
    path: '**',
    component: PageNotFoundComponent
  },
  
];

@NgModule({
  imports: [
    RouterModule.forRoot(
      appRoutes, {
      preloadingStrategy: SelectivePreloadingStrategy
    }
    )
  ],
  exports: [
    RouterModule
  ],
  providers: [
    SelectivePreloadingStrategy
  ]
})

export class AppRoutingModule { }
