import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core'
import { ActivatedRoute } from '@angular/router'

@Component({
	selector: 'app-pagination-render',
	templateUrl: './pagination-render.component.html',
	styleUrls: ['./pagination-render.component.scss']
})

export class PaginationRenderComponent implements OnInit {
	navigatedUrlComponent: any = '';
	@Input() pageNumber: any;
	@Input() pageSize: number;
	@Input() id: any;
	@Output() pagination = new EventEmitter();

	constructor(private activatedroute: ActivatedRoute) { }

	ngOnInit() {
		this.navigatedUrlComponent = this.activatedroute.component;
	}

	CallPageChange(currentpage) {
		this.pagination.emit(currentpage);
	}
}