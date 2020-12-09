import { TestBed } from '@angular/core/testing';

import { DynamicDashboardService } from './dynamic-dashboard.service';

describe('DynamicDashboardService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: DynamicDashboardService = TestBed.get(DynamicDashboardService);
    expect(service).toBeTruthy();
  });
});
