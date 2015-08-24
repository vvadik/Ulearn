window.setTimeout(onTimeout, 1000);

function onTimeout() {
	window.setTimeout(onTimeout, 1000);
	var request = new XMLHttpRequest();
	request.open('GET', location.pathname + "?query=needRefresh", true);
	request.onreadystatechange = function () {
		if (request.readyState == 4)
			if (request.status == 200)
				if (JSON.parse(request.responseText))
					location.reload();
	};
	request.send(null);
}