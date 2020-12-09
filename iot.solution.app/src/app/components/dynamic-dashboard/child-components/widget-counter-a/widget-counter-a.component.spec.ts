import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { WidgetCounterBComponent } from './widget-counter-b.component';

describe('WidgetCounterBComponent', () => {
  let component: WidgetCounterBComponent;
  let fixture: ComponentFixture<WidgetCounterBComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ WidgetCounterBComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(WidgetCounterBComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
