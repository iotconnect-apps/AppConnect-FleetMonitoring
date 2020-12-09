import { Component, EventEmitter, Input, OnInit, Output, Inject } from '@angular/core'
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';

@Component({
	selector: 'app-delete-dialog',
	templateUrl: './delete-dialog.component.html',
	styleUrls: ['./delete-dialog.component.scss']
})

export class DeleteDialogComponent implements OnInit {

	deleteAlertDataModel : {
		title : string, 
		message: string,
		okButtonName: string,
		cancelButtonName: string 
	};

	constructor(
		public dialogRef: MatDialogRef<DeleteDialogComponent>,
		@Inject(MAT_DIALOG_DATA) public data: any
	) { 
		this.deleteAlertDataModel = data;
	}

	ngOnInit() { }

	deleteItem() {
		let resps = {
			result : true
		};
		this.dialogRef.close(resps);
	}
	/**
	 */
	onNoClick(): void {
		this.dialogRef.close();
	}
}
