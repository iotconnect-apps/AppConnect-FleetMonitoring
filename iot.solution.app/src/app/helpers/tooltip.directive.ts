import { Directive, Input, ElementRef, HostListener, Renderer2 } from '@angular/core';

@Directive({
  selector: '[appTooltip]'
})
export class TooltipDirective {

  @Input('appTooltip') toolTipTitle: string;
  @Input() placement: string;
  @Input() delay: number;
  tooltip: HTMLElement;
  offset = 10;
  // @Output() deleteTooltipOnScroll = new EventEmitter<any>();
  constructor(
    private elRef: ElementRef,
    private renderer: Renderer2
  ) { }

  // event listeners for tooltip  
  @HostListener('load') OnLoad() {
    if(this.tooltip)
      this.renderer.removeChild(document.body, this.tooltip);
  }
  @HostListener('mouseenter') onMouseEnter() {
    if(!this.tooltip) {
      this.showTooltip();      
    }
  }
  @HostListener('mouseleave') onMouseLeave() {
    if(this.tooltip)
      this.hideTooltip();
  }  
  @HostListener('click') OnClick() {
    if(this.tooltip) {
      this.renderer.removeChild(document.body, this.tooltip);
      // this.tooltip = null;
    }
  }
  @HostListener('window:scroll') OnScroll() {
    if(this.tooltip) {
      // this.renderer.removeChild(document.body, this.tooltip);
      // this.tooltip = null;
      this.setTooltipPos();      
    }
  }

  // create and show / hide tooltips
  showTooltip() {
    this.createTooltip();
    this.setTooltipPos();
    this.renderer.addClass(this.tooltip, 'ng-tooltip-show');
  }
  hideTooltip() {
    this.renderer.removeClass(this.tooltip, 'ng-tooltip-show');
    window.setTimeout(() => {
      this.renderer.removeChild(document.body, this.tooltip);
      this.tooltip = null;
    }, this.delay);
  }

  deleteTooltip() {
    if(this.tooltip) {
      this.renderer.removeChild(document.body, this.tooltip);
      this.tooltip = null;
    }
  }
  createTooltip() {
    this.tooltip = this.renderer.createElement('span');
    this.renderer.appendChild(this.tooltip, this.renderer.createText(this.toolTipTitle));

    this.renderer.appendChild(document.body, this.tooltip);
    this.renderer.addClass(this.tooltip, 'ng-tooltip');
    this.renderer.addClass(this.tooltip, `ng-tooltip-${this.placement}`);

    this.renderer.setStyle(this.tooltip, '-webkit-transition', `opacity ${this.delay}ms`);
    this.renderer.setStyle(this.tooltip, '-moz-transition', `opacity ${this.delay}ms`);
    this.renderer.setStyle(this.tooltip, '-o-transition', `opacity ${this.delay}ms`);
    this.renderer.setStyle(this.tooltip, 'transition', `opacity ${this.delay}ms`);
  }

  // set tooltip position according to it's parent element
  setTooltipPos() {
    const linkPos = this.elRef.nativeElement.getBoundingClientRect();
    const tooltipPos = this.tooltip.getBoundingClientRect();
    const scrollPos = window.pageYOffset || document.documentElement.scrollTop || document.body.scrollTop || 0;
    let top, left;

    if (this.placement === 'top') {
      top = linkPos.top - tooltipPos.height - this.offset;
      left = linkPos.left + (linkPos.width - tooltipPos.width) / 2;      
    }
    if (this.placement === 'bottom') {
      top = linkPos.bottom + this.offset;
      left = linkPos.left + (linkPos.width - tooltipPos.width) / 2;      
    }
    if (this.placement === 'left') {
      top = linkPos.top + (linkPos.height - tooltipPos.height) / 2;
      left = linkPos.left - tooltipPos.width - this.offset;      
    }
    if (this.placement === 'right') {
      top = linkPos.top + (linkPos.height - tooltipPos.height) / 2;
      left = linkPos.right + this.offset;    
    }
    this.renderer.setStyle(this.tooltip, 'top', `${top + scrollPos}px`);
    this.renderer.setStyle(this.tooltip, 'left', `${left}px`);
  }

}
