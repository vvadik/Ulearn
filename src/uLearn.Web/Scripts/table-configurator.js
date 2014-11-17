$('.tablesorter').tablesorter({
	widthFixed: true,
	widgets: ['zebra'],
	dateFormat: "ddmmyyyy",

	sortInitialOrder: "asc",
	sortForce: null,
	sortList: [],
	sortAppend: null,
	sortLocaleCompare: false,
	sortReset: false,
	sortRestart: false,
	sortMultiSortKey: "shiftKey",

	onRenderHeader: function () {
		$(this).find('span').addClass('headerSpan');
	},
	selectorHeaders: 'thead th',

	cssAsc: "headerSortUp",
	cssChildRow: "expand-child",
	cssDesc: "headerSortDown",
	cssHeader: "header",
	tableClass: 'tablesorter',

	widgetColumns: { css: ["primary", "secondary", "tertiary"] },
	widgetUitheme: {
		css: [
			"ui-icon-arrowthick-2-n-s",
			"ui-icon-arrowthick-1-s",
			"ui-icon-arrowthick-1-n"
		]
	},
	widgetZebra: { css: ["ui-widget-content", "ui-state-default"] },
	
	cancelSelection: true,
	
	debug: false,
	
	textAttribute: 'data-sort-value'
});