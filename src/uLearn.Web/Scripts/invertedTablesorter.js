$(".invertedTablesorter").each(function () {
	var opt = {};
	var table = $(this);
	table.find("th").each(function (ind) {
		var sort = $(this).data("sorter");
		if (!sort) {
			opt[ind] = { sorter: false };
		}
	});

	table
		.addClass("tablesorter")
		.tablesorter({ headers: opt });
});