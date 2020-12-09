import { TestBed } from '@angular/core/testing';

import { LeftMenuService } from './left-menu.service';

describe('LeftMenuService', () => {
	beforeEach(() => TestBed.configureTestingModule({}));

	it('should be created', () => {
		const service: LeftMenuService = TestBed.get(LeftMenuService);
		expect(service).toBeTruthy();
	});
});
