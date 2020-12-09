import { Component, OnInit } from '@angular/core';
import { Notification, NotificationService } from 'app/services';

/* @Component({
  selector: 'app-flash-message',
  templateUrl: './flash-message.component.html',
  styleUrls: ['./flash-message.component.css']
}) */
/* export class FlashMessageComponent implements OnInit {

  constructor() { }

  ngOnInit() {
  }

} */

@Component({
	selector: 'app-flash-message',
	template: `
    <div class="notifications">
        <div (click)="hide(note)" class="{{ note.type }}"
                *ngFor="let note of _notes">
            <span class="closebtn">&times;</span>
            <span [innerHTML]="note.message"></span>
        </div>
    </div>    `,
	styles: [`
        .notifications {
            //left: calc(50% - 150px);
            right: 10px;
            position: fixed;
            bottom: 10px;
            z-index:9999;
            color:#fff;
        }

        .notifications div{
            text-overflow: initial;
            box-shadow: 0 7px 10px 0 rgba(0, 0, 0, .22),
                        0 12px 15px 0 rgba(0, 0, 0, .12);
            cursor: pointer;
            margin-bottom: 10px;
            min-height: 50px;
            padding: 5%;
            min-width: 340px;
            color: #fff!important;
            border-left: solid 4px #343434!important;
        }

        .notifications .success {
            background-color: #21a788;
            border-radius: 4px;
            border-left: 5px solid #197c65!important;
        }

        .notifications .error {
            background-color: #fa424a;
            border-radius: 4px;
            border-left-color: #da020b !important
        }

        .notifications .warn {
            background-color: #f29824;
            border-radius: 4px;
            border-left: 4px solid #d67e0d !important;
        }

        .notifications .info {
            background-color: #00a8ff;
            border-radius: 4px;
            border-left: 4px solid #0086cc !important;
        }

        .closebtn{
            float:right;
            padding-right:5px;
            padding-left:5px;
            margin-top:-9px;
            font-size: 30px;
            color:#fff;
        }

    `]
})

export class FlashMessageComponent {
	public _notes: Notification[];

	constructor(private _notifications: NotificationService) {
		this._notes = new Array<Notification>();
		this._notifications.noteAdded.subscribe(note => {
			this._notes.push(note);
			setTimeout(() => { this.hide.bind(this)(note) }, 7000);
		});
	}

	/* For hide notification */
	private hide(note) {
		let index = this._notes.indexOf(note);

		if (index >= 0) {
			this._notes.splice(index, 1);
		}
	}
}


