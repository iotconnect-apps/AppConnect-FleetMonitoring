import { Component, EventEmitter, Input, OnInit, Output, Inject } from '@angular/core'
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';

@Component({
  selector: 'app-message-dialog',
  templateUrl: './message-dialog.component.html',
  styleUrls: ['./message-dialog.component.css']
})
export class MessageDialogComponent implements OnInit {

  messageAlertDataModel: {
    title: string,
    message: string,
    message2: string,
    okButtonName: string
  };

  constructor(public dialogRef: MatDialogRef<MessageDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any
  ) {
    this.messageAlertDataModel = data;
  }

  ngOnInit() {
  }

  /**
	 */
  onOkClick(): void {
    this.dialogRef.close();
  }
}
