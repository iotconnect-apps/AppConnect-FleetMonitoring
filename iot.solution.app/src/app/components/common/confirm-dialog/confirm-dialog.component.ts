import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core'

@Component({
	selector: 'app-confirm-dialog',
	templateUrl: './confirm-dialog.component.html',
	styleUrls: ['./confirm-dialog.component.scss']
})

export class ConfirmDialogComponent implements OnInit {
	@Input() name: any;
	@Input() moduleName: any;
	@Input() dialogId: any;
	@Input() status: any;
	@Input() msgType: any;
	@Output() onSave = new EventEmitter<string>();

	constructor() { }

	ngOnInit() { }

	clickOnSave() {
		this.onSave.emit();
	}
}
