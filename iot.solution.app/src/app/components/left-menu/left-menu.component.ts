import { Component, OnInit } from '@angular/core'
import { Router } from '@angular/router'


@Component({
  selector: 'app-left-menu',
  templateUrl: './left-menu.component.html',
  styleUrls: ['./left-menu.component.css']
})

export class LeftMenuComponent implements OnInit {
  menuList: any = [];
  user_type: any;
  selectedMenu: any;
  currentUrl: any;
  isAdmin = false;
  constructor(private router: Router) { }

  ngOnInit() {
    let currentUser = JSON.parse(localStorage.getItem('currentUser'));
    this.isAdmin = currentUser.userDetail.isAdmin;
    let tempUrl = (this.router.url).split('/');
    this.currentUrl = "/" + tempUrl[1];
    if (this.isAdmin) {
      this.menuList = [
        {
          name: 'Dashboard',
          link: '/admin/dashboard',
          li_color: '',
          icon: 'icon-dashboard',
          applylink: true
        },
        {
          name: 'Users',
          link: '/admin/users',
          li_color: '',
          icon: 'icon-user-group',
          applylink: true
        },
        {
          name: 'Subscribers',
          link: '/admin/subscribers',
          li_color: '',
          icon: 'icon-subscribers',
          applylink: true
        },

      ];
    } else {
      this.menuList = [
        {
          name: 'Dashboard',
          link: '/dashboard',
          li_color: '',
          icon: 'icon-dashboard',
          applylink: true
        },
        // {
        //   name: 'Locations',
        //   link: "/locations",
        //   li_color: '',
        //   icon: 'icon-location',
        //   applylink: true
        // },
        // {
        //   name: "Assets Type",
        //   link: "/assets-types",
        //   li_color: '',
        //   icon: 'icon-asset-type',
        //   applylink: true
        // },
        // {
        //   name: 'Assets',
        //   link: '/assets',
        //   li_color: '',
        //   icon: 'icon-assets',
        //   applylink: true
        // },
        // {
        //   name: 'Maintenance',
        //   link: '/maintenance',
        //   li_color: '',
        //   icon: 'icon-maintenance',
        //   applylink: true
        // },
        // {
        //   name: "Roles",
        //   link: "/roles",
        //   li_color: '',
        //   icon: 'icon-userrole',
        //   applylink: true
        // },
        // {
        //   name: "Users",
        //   link: "/users",
        //   li_color: '',
        //   icon: 'icon-user-group',
        //   applylink: true
        // },

        {
          name: 'Fleet',
          link: '/fleet',
          li_color: '',
          icon: 'icon-fleet',
          applylink: true
        },
        {
          name: 'Devices',
          link: '/devices',
          li_color: '',
          icon: 'icon-device',
          applylink: true
        },
        {
          name: 'Trips',
          link: '/trips',
          li_color: '',
          icon: 'icon-trips',
          applylink: true
        },
        {
          name: 'Drivers',
          link: '/drivers',
          li_color: '',
          icon: 'icon-drivers',
          applylink: true
        },
        {
          name: 'Maintenance',
          link: '/maintenance',
          li_color: '',
          icon: 'icon-maintenance',
          applylink: true
        },
        {
          name: 'Roles',
          link: '/roles',
          li_color: '',
          // icon-sidebar icon-userrole
          icon: 'icon-userrole',
          applylink: true
        },
        {
          name: 'Users',
          link: '/users',
          li_color: '',
          icon: 'icon-user-group',
          applylink: true
        },
        {
          name: 'Alerts',
          link: '/alerts',
          li_color: '',
          icon: 'icon-alerts',
          applylink: true
        }
      ];
    }

  }

  manageNavigateUrl(url) {
    this.router.navigate([url]);
  }

  showSubMenu(menu) {
    menu.showSunMenu = !menu.showSunMenu;
  }

  onClickMenu(i) {
    this.currentUrl = false;
    this.selectedMenu = i;
  }
}
