window.setTimeout(function () {
	var request = new XMLHttpRequest();
	request.open('GET', '/needRefresh/?location=' + location.pathname, true);
	request.onreadystatechange = function () {
		if (request.readyState == 4) {
			if (request.status == 200) {
				alert(request.responseText);
				if (request.responseText == "true")
					location.reload();
			}
		}
	};
	request.send(null);
}, 1000);
