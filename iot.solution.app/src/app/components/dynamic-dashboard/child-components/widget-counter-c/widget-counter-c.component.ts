import {  OnInit, Component, Input, ViewEncapsulation, EventEmitter } from '@angular/core';
import {Subscription} from 'rxjs/Subscription';

@Component({
	selector: 'app-widget-counter-c',
	templateUrl: './widget-counter-c.component.html',
	styleUrls: ['./widget-counter-c.component.css']
})
export class WidgetCounterCComponent implements OnInit {

	@Input() widget;
	@Input() count;
	@Input() resizeEvent: EventEmitter<any>;
	resizeSub: Subscription;
	constructor() { }

	ngOnInit() {
		this.resizeSub = this.resizeEvent.subscribe((widget) => {
		});
	}

	ngOnDestroy() {
		this.resizeSub.unsubscribe();
	}

}
