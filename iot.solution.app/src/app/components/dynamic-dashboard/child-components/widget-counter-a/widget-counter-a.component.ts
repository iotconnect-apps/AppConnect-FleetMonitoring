import {  OnInit, Component, Input, ViewEncapsulation, EventEmitter } from '@angular/core';
import {Subscription} from 'rxjs/Subscription';

@Component({
	selector: 'app-widget-counter-a',
	templateUrl: './widget-counter-a.component.html',
	styleUrls: ['./widget-counter-a.component.css']
})
export class WidgetCounterAComponent implements OnInit {

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
