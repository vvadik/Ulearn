window.setTimeout(onTimeout, 1000);

function onTimeout() {
	var request = new XMLHttpRequest();
	request.open('GET', location.pathname + "?query=needRefresh", true);
	request.onreadystatechange = function () {
		if (request.readyState == 4)
			if (request.status == 200) {
				window.setTimeout(onTimeout, 1000);
				if (JSON.parse(request.responseText))
					location.reload();
			}
	};
	request.send(null);
}