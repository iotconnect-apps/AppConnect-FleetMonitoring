import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core'
import { ActivatedRoute } from '@angular/router'

@Component({
	selector: 'app-page-size-render',
	templateUrl: './page-size-render.component.html',
	styleUrls: ['./page-size-render.component.scss']
})

export class PageSizeRenderComponent implements OnInit {
	itemsPerPage: any[] = ['10', '15', '25', '50', '100'];
	limitationForPageSize: number = 10;
	navigatedUrlComponent: any = '';
	@Input() pageSize: any;
	@Input() totalItemsCount: number;
	@Output() onPageSizeChange = new EventEmitter();
	@Input() currentPage: any;

	constructor(private activatedroute: ActivatedRoute) { }

	ngOnInit() {
		this.navigatedUrlComponent = this.activatedroute.component;
	}

	onChangeItemSize(pageSize) {
		this.onPageSizeChange.emit(pageSize);
	}
}