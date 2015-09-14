function diffHtml(actual, expected) {
	this.getEoln = function (s) {
		var good = ["\r\n", "\n", "\r"].filter(function(eoln) { return s.indexOf(eoln) >= 0 });
		return good.length > 0 ? good[0] : "\n";
	}

	this.split = function (s) {
		return s.split(this.getEoln(s));
	}

	this.toString = function(s) {
		return s === undefined ? "" : $('<div/>').text(s).html();
	}

	var actualList = this.split(actual);
	var expectedList = this.split(expected);

	var linesCount = Math.max(actualList.length, expectedList.length);

	var res = '<table class="diff"> <thead> <th></th> <th>Вывод вашей программы</th> <th>Ожидаемый вывод</th> </thead>';

	for (var i = 0; i < linesCount; ++i) {
		var c = actualList[i] === expectedList[i] ? "equals" : "different";
		res += '<tr class="' + c + '"> <th> ' + (i + 1) + '</th>';
		res += '<td>' + this.toString(actualList[i]) + '</td>';
		res += '<td>' + this.toString(expectedList[i]) + '</td>';
		res += '</tr>';
	}
	res += '</table>';
	return res;
}