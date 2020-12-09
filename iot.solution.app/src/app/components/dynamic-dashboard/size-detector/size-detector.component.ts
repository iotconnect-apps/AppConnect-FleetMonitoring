import { Component, AfterViewInit, HostListener, ElementRef, Output, EventEmitter } from '@angular/core';

@Component({
	selector: 'app-size-detector',
	templateUrl: './size-detector.component.html',
	styleUrls: ['./size-detector.component.css']
})
export class SizeDetectorComponent implements AfterViewInit  {
	@Output() deviceSizeChange: EventEmitter<any> = new EventEmitter<any>();
	public prefix = 'visible-';
	sizes = [
		{
			id: SCREEN_SIZE.XS, name: 'xs', css: 'd-block d-sm-none'
		},
		{
			id: SCREEN_SIZE.SM, name: 'sm', css: 'd-none d-sm-block d-md-none'
		},
		{
			id: SCREEN_SIZE.MD, name: 'md', css: 'd-none d-md-block d-lg-none'
		},
		{
			id: SCREEN_SIZE.LG, name: 'lg', css: 'd-none d-lg-block d-xl-none'
		},
		{
			id: SCREEN_SIZE.XL, name: 'xl', css: 'd-none d-xl-block'
		}
	];

	constructor(private elementRef: ElementRef) {
	}

	@HostListener("window:resize", [])
	private onResize() {
		this.detectScreenSize();
	}

	ngAfterViewInit() {
		this.detectScreenSize();
	}

	private detectScreenSize(){
		let currentSize = this.sizes.find(x => {
			// get the HTML element
			const el = this.elementRef.nativeElement.querySelector('.size-detector-div'+x.name);
			// check its display property value
			const isVisible = window.getComputedStyle(el).display != 'none';
			return isVisible;
		});
		this.deviceSizeChange.emit(currentSize);
	}
}
export enum SCREEN_SIZE {
	XS,
	SM,
	MD,
	LG,
	XL
}
