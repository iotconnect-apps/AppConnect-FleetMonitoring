import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { DashboardFleetComponent } from './dashboard-fleet.component';

describe('DashboardFleetComponent', () => {
  let component: DashboardFleetComponent;
  let fixture: ComponentFixture<DashboardFleetComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ DashboardFleetComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(DashboardFleetComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
