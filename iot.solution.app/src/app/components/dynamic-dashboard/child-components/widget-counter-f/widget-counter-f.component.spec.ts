import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { WidgetCounterFComponent } from './widget-counter-f.component';

describe('WidgetCounterFComponent', () => {
  let component: WidgetCounterFComponent;
  let fixture: ComponentFixture<WidgetCounterFComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ WidgetCounterFComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(WidgetCounterFComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
