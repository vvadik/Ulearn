function saveVisits(id, time) {
	var obj = {};
	if (typeof localStorage['visits'] !== 'undefined')
		obj = JSON.parse(localStorage['visits']);
	obj[id] = time;
	localStorage['visits'] = JSON.stringify(obj);
}

function uploadVisits(url) {
	if (typeof localStorage['visits'] === 'undefined')
		return;
	$.ajax({
			type: 'POST',
			url: url,
			data: localStorage['visits']
		})
		.done(function(ans) {
			localStorage.removeItem('visits');
		})
		.fail(function(req) {
			console.log(req.responseText);
		});
}

function displayVisits() {
	if (typeof localStorage['visits'] === 'undefined')
		return;
	var visits = JSON.parse(localStorage['visits']);
	for (var visit in visits) {
		var splittedVisit = visit.split(' ');
		var slideId = splittedVisit.length > 1 ? splittedVisit[1] : splittedVisit[0];
		var glyph = $('[data-slide-id="' + slideId + '"] > i');
		if (glyph.hasClass('glyphicon-none')) {
			glyph.removeClass('glyphicon-none');
			glyph.addClass('glyphicon-ok');
		}
		else if (!glyph.hasClass('navbar-label-success')) {
			glyph.removeClass('navbar-label-default');
			glyph.addClass('navbar-label-danger');
		}
	}
}