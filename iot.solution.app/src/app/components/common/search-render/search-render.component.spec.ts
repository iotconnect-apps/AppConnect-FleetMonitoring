import { async, ComponentFixture, TestBed } from '@angular/core/testing'

import { SearchRenderComponent } from './search-render.component'

describe('SearchRenderComponent', () => {
	let component: SearchRenderComponent;
	let fixture: ComponentFixture<SearchRenderComponent>;

	beforeEach(async(() => {
		TestBed.configureTestingModule({
			declarations: [SearchRenderComponent]
		})
			.compileComponents();
	}));

	beforeEach(() => {
		fixture = TestBed.createComponent(SearchRenderComponent);
		component = fixture.componentInstance;
		fixture.detectChanges();
	});

	it('should create', () => {
		expect(component).toBeTruthy();
	});
});