function saveVisiters(id, time) {
	var obj = {};
	if (typeof localStorage['visiters'] !== 'undefined')
		obj = JSON.parse(localStorage['visiters']);
	obj[id] = time;
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

function displayVisits() {
	if (typeof localStorage['visiters'] === 'undefined')
		return;
	var visits = JSON.parse(localStorage['visiters']);
	for (var visit in visits) {
		var glyph = $('[data-slide-id="' + visit + '"] > i');
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