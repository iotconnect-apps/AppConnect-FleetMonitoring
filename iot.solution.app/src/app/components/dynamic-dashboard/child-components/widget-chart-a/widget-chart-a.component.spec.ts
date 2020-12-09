import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { WidgetChartAComponent } from './widget-chart-a.component';

describe('WidgetChartOneComponent', () => {
  let component: WidgetChartAComponent;
  let fixture: ComponentFixture<WidgetChartAComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ WidgetChartAComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(WidgetChartAComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
