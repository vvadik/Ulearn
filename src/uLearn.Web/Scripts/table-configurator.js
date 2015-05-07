$('.tablesorter').tablesorter({
	widthFixed: true,
	widgets: ['zebra', 'stickyHeaders'],

	onRenderHeader: function () {
		$(this).find('span').addClass('headerSpan');
	},
	selectorHeaders: 'thead th',

	cssAsc: "headerSortUp",
	cssDesc: "headerSortDown",
	cssHeader: "header",
	tableClass: 'tablesorter',

	widgetOptions: {
		columns: ["primary", "secondary", "tertiary"],
		stickyHeaders_offset: "#header",
	},

	textAttribute: 'data-sort-value'
});