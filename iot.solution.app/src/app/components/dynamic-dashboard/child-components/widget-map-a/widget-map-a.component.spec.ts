import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { WidgetMapAComponent } from './widget-map-a.component';

describe('WidgetMapAComponent', () => {
  let component: WidgetMapAComponent;
  let fixture: ComponentFixture<WidgetMapAComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ WidgetMapAComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(WidgetMapAComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
