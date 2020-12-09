import { async, ComponentFixture, TestBed } from '@angular/core/testing'

import { PageSizeRenderComponent } from './page-size-render.component'

describe('PageSizeRenderComponent', () => {
	let component: PageSizeRenderComponent;
	let fixture: ComponentFixture<PageSizeRenderComponent>;

	beforeEach(async(() => {
		TestBed.configureTestingModule({
			declarations: [PageSizeRenderComponent]
		})
			.compileComponents();
	}));

	beforeEach(() => {
		fixture = TestBed.createComponent(PageSizeRenderComponent);
		component = fixture.componentInstance;
		fixture.detectChanges();
	});

	it('should create', () => {
		expect(component).toBeTruthy();
	});
});