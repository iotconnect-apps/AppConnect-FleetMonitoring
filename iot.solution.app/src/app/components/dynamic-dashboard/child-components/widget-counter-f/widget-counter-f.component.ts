import {  OnInit, OnDestroy, Component, Input, ViewEncapsulation, EventEmitter } from '@angular/core';
import {Subscription} from 'rxjs/Subscription';

@Component({
	selector: 'app-widget-counter-f',
	templateUrl: './widget-counter-f.component.html',
	styleUrls: ['./widget-counter-f.component.css']
})
export class WidgetCounterFComponent implements OnInit,OnDestroy {
	@Input() widget;
	@Input() count;
	@Input() resizeEvent: EventEmitter<any>;
	resizeSub: Subscription;

	constructor(){

	}

	ngOnInit() {
		this.resizeSub = this.resizeEvent.subscribe((widget) => {
		});
	}

	ngOnDestroy() {
		this.resizeSub.unsubscribe();
	}
}
