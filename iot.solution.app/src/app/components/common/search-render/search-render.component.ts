import { Component, ElementRef, EventEmitter, Input, OnInit, 
	Output, ViewChild, AfterViewInit, ChangeDetectorRef } from '@angular/core'
import { ActivatedRoute } from '@angular/router'

@Component({
	selector: 'app-search-render',
	templateUrl: './search-render.component.html',
	styleUrls: ['./search-render.component.scss']
})

export class SearchRenderComponent implements OnInit, AfterViewInit {
	navigatedUrlComponent: any = '';
	searchText = '';
	@Input() searchValue: any;
	@Output() searchEvent = new EventEmitter();
	@Output() changeSearchtext = new EventEmitter();
	@ViewChild('searchFocus', { static: false }) private elementRef: ElementRef;

	constructor(
		private activatedroute: ActivatedRoute,
		private cd: ChangeDetectorRef
		) { }

	ngOnInit() {
		if (this.searchValue !== '') {
			this.searchText = this.searchValue;
		}
		this.navigatedUrlComponent = this.activatedroute.component;
	}

	ngAfterViewInit() {
		this.elementRef.nativeElement.focus();
		this.cd.detectChanges(); // manual change detection
	}

	search(searchText) {

		this.searchEvent.emit(searchText);
	}

	changeSearch(searchText) {
		this.changeSearchtext.emit(searchText);
	}
}