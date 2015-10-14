function saveVisiters(url, time) {
	var obj = {};
	if (typeof localStorage['visiters'] !== "undefined")
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