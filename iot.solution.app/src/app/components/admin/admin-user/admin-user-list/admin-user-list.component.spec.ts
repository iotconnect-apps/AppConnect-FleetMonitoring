import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { UserAdminListComponent } from './admin-user-list.component';

describe('UserAdminListComponent', () => {
  let component: UserAdminListComponent;
  let fixture: ComponentFixture<UserAdminListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ UserAdminListComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(UserAdminListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
