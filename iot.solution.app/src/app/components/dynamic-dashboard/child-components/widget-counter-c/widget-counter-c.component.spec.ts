import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { WidgetCounterAComponent } from './widget-counter-a.component';

describe('WidgetCounterAComponent', () => {
  let component: WidgetCounterAComponent;
  let fixture: ComponentFixture<WidgetCounterAComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ WidgetCounterAComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(WidgetCounterAComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
