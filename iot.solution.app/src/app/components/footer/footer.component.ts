import * as $ from 'jquery'
import { AfterViewInit, Component, OnInit } from '@angular/core'

@Component({
	selector: 'app-footer',
	templateUrl: './footer.component.html',
	styleUrls: ['./footer.component.css'],
})

export class FooterComponent implements OnInit, AfterViewInit {
	currentDate  = new Date();
	
	constructor() { }

	ngOnInit() { 
	}


	ngAfterViewInit() {
	

		// $('.dropdown-more').each(function () {
		// 	var parent = $(this);
		// 	var more = parent.find('.dropdown-more-caption');
		// 	var classOpen = 'opened';

		// 	more.click(function () {
		// 		if (parent.hasClass(classOpen)) {
		// 			parent.removeClass(classOpen);
		// 		} else {
		// 			parent.addClass(classOpen);
		// 		}
		// 	});
		// });


		
		function fileManagerHeight() {
			$('.files-manager').each(function () {
				var box = $(this),
					boxColLeft = box.find('.files-manager-side'),
					boxSubHeader = box.find('.files-manager-header'),
					boxCont = box.find('.files-manager-content-in'),
					boxColRight = box.find('.files-manager-aside');

				var paddings = parseInt($('.page-content').css('padding-top')) +
					parseInt($('.page-content').css('padding-bottom')) +
					parseInt(box.css('margin-bottom')) + 2;

				boxColLeft.height('auto');
				boxCont.height('auto');
				boxColRight.height('auto');

				if (boxColLeft.height() <= ($(window).height() - paddings)) {
					boxColLeft.height(
						$(window).height() - paddings
					);
				}

				if (boxColRight.height() <= ($(window).height() - paddings - boxSubHeader.outerHeight())) {
					boxColRight.height(
						$(window).height() -
						paddings -
						boxSubHeader.outerHeight()
					);
				}

				boxCont.height(
					boxColRight.height()
				);
			});
		}

		fileManagerHeight();

		$(window).resize(function () {
			fileManagerHeight();
		});

		function mailBoxHeight() {
			$('.mail-box').each(function () {
				var box = $(this),
					boxHeader = box.find('.mail-box-header'),
					boxColLeft = box.find('.mail-box-list'),
					boxSubHeader = box.find('.mail-box-work-area-header'),
					boxColRight = box.find('.mail-box-work-area-cont');

				boxColLeft.height(
					$(window).height() -
					parseInt($('.page-content').css('padding-top')) -
					parseInt($('.page-content').css('padding-bottom')) -
					parseInt(box.css('margin-bottom')) - 2 -
					boxHeader.outerHeight()
				);

				boxColRight.height(
					$(window).height() -
					parseInt($('.page-content').css('padding-top')) -
					parseInt($('.page-content').css('padding-bottom')) -
					parseInt(box.css('margin-bottom')) - 2 -
					boxHeader.outerHeight() -
					boxSubHeader.outerHeight()
				);
			});
		}

		mailBoxHeight();

		$(window).resize(function () {
			mailBoxHeight();
		});

		$('.dd-handle').hover(function () {
			$(this).prev('button').addClass('hover');
			$(this).prev('button').prev('button').addClass('hover');
		}, function () {
			$(this).prev('button').removeClass('hover');
			$(this).prev('button').prev('button').removeClass('hover');
		});

		function stepsProgresMarkup() {
			$('.steps-icon-progress').each(function () {
				var parent = $(this),
					cont = parent.find('ul'),
					padding = 0,
					padLeft = (parent.find('li:first-child').width() - parent.find('li:first-child .caption').width()) / 2,
					padRight = (parent.find('li:last-child').width() - parent.find('li:last-child .caption').width()) / 2;

				padding = padLeft;

				if (padLeft > padRight) padding = padRight;

				cont.css({
					marginLeft: -padding,
					marginRight: -padding
				});
			});
		}

		stepsProgresMarkup();

		$(window).resize(function () {
			stepsProgresMarkup();
		});

		$('.control-panel-toggle').on('click', function () {
			var self = $(this);

			if (self.hasClass('open')) {
				self.removeClass('open');
				$('.control-panel').removeClass('open');
			} else {
				self.addClass('open');
				$('.control-panel').addClass('open');
			}
		});

		$('.icon-toggle').on('click', function () {
			var self = $(this);

			if (self.hasClass('open')) {
				self.removeClass('open');
				$('.control-panel').removeClass('open');
			} else {
				self.addClass('open');
				$('.control-panel').addClass('open');
			}
		});

		$('.control-item-header .icon-toggle').on('click', function () {
			var content = $(this).closest('li').find('.control-item-content');

			if (content.hasClass('open')) {
				content.removeClass('open');
			} else {
				$('.control-item-content.open').removeClass('open');
				content.addClass('open');
			}
		});
	}
}