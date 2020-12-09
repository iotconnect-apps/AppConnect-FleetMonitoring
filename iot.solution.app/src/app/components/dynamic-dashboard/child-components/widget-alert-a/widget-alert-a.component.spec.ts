import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { WidgetAlertAComponent } from './widget-alert-a.component';

describe('WidgetAlertAComponent', () => {
  let component: WidgetAlertAComponent;
  let fixture: ComponentFixture<WidgetAlertAComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ WidgetAlertAComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(WidgetAlertAComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
