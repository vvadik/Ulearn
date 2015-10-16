function saveVisiters(url, time) {
	var obj = {};
	if (typeof localStorage['visiters'] !== 'undefined')
		obj = JSON.parse(localStorage['visiters']);
	obj[url] = time;
	localStorage['visiters'] = JSON.stringify(obj);
}

function uploadVisiters(url) {
	if (typeof localStorage['visiters'] === 'undefined')
		return;
	$.ajax({
			type: 'POST',
			url: url,
			data: localStorage['visiters']
		})
		.success(function(ans) {
			localStorage.removeItem('visiters');
		})
		.fail(function(req) {
			console.log(req.responseText);
		});
}

function displayVisits(courseId) {
	if (typeof localStorage['visiters'] === 'undefined')
		return;
	var slidesGlyphs = $('.slide-list-item > .glyphicon:not(.navbar-label-privileged)');
	var visits = JSON.parse(localStorage['visiters']);
	for (var visit in visits) {
		var parts = visit.split('/', 2);
		if (courseId != parts[0] || !$.isNumeric(parts[1]))
			continue;
		var ind = parseInt(parts[1]);
		var glyph = $(slidesGlyphs[ind]);
		if (glyph.hasClass('glyphicon-none')) {
			glyph.removeClass('glyphicon-none');
			glyph.addClass('glyphicon-ok');
		}
	}
}