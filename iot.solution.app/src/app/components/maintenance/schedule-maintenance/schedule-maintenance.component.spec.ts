import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ScheduleMaintenanceComponent } from './schedule-maintenance.component';

describe('ScheduleMaintenanceComponent', () => {
  let component: ScheduleMaintenanceComponent;
  let fixture: ComponentFixture<ScheduleMaintenanceComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ScheduleMaintenanceComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ScheduleMaintenanceComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
