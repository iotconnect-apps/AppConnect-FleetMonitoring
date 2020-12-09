import {  OnInit, Component, Input, ViewEncapsulation, EventEmitter } from '@angular/core';
import {Subscription} from 'rxjs/Subscription';

@Component({
	selector: 'app-widget-counter-b',
	templateUrl: './widget-counter-b.component.html',
	styleUrls: ['./widget-counter-b.component.css']
})
export class WidgetCounterBComponent implements OnInit {

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
