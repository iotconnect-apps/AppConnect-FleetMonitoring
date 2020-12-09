import {  OnInit, Component, Input, ViewEncapsulation, EventEmitter } from '@angular/core';
import {Subscription} from 'rxjs/Subscription';

@Component({
	selector: 'app-widget-counter-d',
	templateUrl: './widget-counter-d.component.html',
	styleUrls: ['./widget-counter-d.component.css']
})
export class WidgetCounterDComponent implements OnInit {

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
