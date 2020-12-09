import { async, ComponentFixture, TestBed } from '@angular/core/testing'

import { PaginationRenderComponent } from './pagination-render.component'

describe('PaginationRenderComponent', () => {
	let component: PaginationRenderComponent;
	let fixture: ComponentFixture<PaginationRenderComponent>;

	beforeEach(async(() => {
		TestBed.configureTestingModule({
			declarations: [PaginationRenderComponent]
		})
			.compileComponents();
	}));

	beforeEach(() => {
		fixture = TestBed.createComponent(PaginationRenderComponent);
		component = fixture.componentInstance;
		fixture.detectChanges();
	});

	it('should create', () => {
		expect(component).toBeTruthy();
	});
});