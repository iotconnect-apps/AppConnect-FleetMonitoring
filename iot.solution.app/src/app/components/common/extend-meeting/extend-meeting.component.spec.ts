import { async, ComponentFixture, TestBed } from '@angular/core/testing'

import { ExtendMeetingComponent } from './extend-meeting.component'

describe('ExtendMeetingComponent', () => {
	let component: ExtendMeetingComponent;
	let fixture: ComponentFixture<ExtendMeetingComponent>;

	beforeEach(async(() => {
		TestBed.configureTestingModule({
			declarations: [ExtendMeetingComponent]
		})
			.compileComponents();
	}));

	beforeEach(() => {
		fixture = TestBed.createComponent(ExtendMeetingComponent);
		component = fixture.componentInstance;
		fixture.detectChanges();
	});

	it('should create', () => {
		expect(component).toBeTruthy();
	});
});