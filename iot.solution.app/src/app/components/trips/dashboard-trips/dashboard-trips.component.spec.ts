import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { DashboardTripsComponent } from './dashboard-trips.component';

describe('DashboardTripsComponent', () => {
  let component: DashboardTripsComponent;
  let fixture: ComponentFixture<DashboardTripsComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ DashboardTripsComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(DashboardTripsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
