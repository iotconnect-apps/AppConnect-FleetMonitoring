import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core'

@Component({
	selector: 'app-extend-meeting',
	templateUrl: './extend-meeting.component.html',
	styleUrls: ['./extend-meeting.component.scss']
})

export class ExtendMeetingComponent implements OnInit {
	@Input() dialogId: any;
	@Output() onSave = new EventEmitter<string>();

	constructor() { }

	ngOnInit() { }

	clickOnSave() {
		this.onSave.emit();
	}
}
