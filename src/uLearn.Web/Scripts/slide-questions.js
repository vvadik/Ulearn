function sendQuestion(title, unitName) {
	var quest = $("#questField").val();
	$.ajax(
		{
			type: "POST",
			url: $("#Ask").data("url"),
			data: {
			    title: title, unitName:unitName, question:quest}
		}).success(function (ans) {
			$("#questField").val("");
		})
		.fail(function (req) {
			console.log(req.responseText);
		})
		.always(function (ans) {
		});
};

function printAllQuestions(courseName) {
	$.ajax(
		{
			type: "POST",
			url: $("#WatchQuestions").data("url"),
			data: {
			    courseName:courseName}
		}).success(function (ans) {
		    $questionLog.html(makeTableForQuestions(ans));
	    })
		.fail(function (req) {
			console.log(req.responseText);
		})
		.always(function (ans) {
		});
};

function makeTableForQuestions(input) {
	var content = input.split("\n***");
	var table = "<table class=\"answer-table\" border=1px>";
	for (var i = 0; i < content.length - 1; i += 5) {
		table += "<tr>";
		for (var j = 0; j < 4; j++)
			table += ("<td class=\"answer-table-headers\">" + content[i + j] + "</td>");
		table += "</tr>";
		table += "<tr><td colspan=\"4\">" + content[i + 4] + "</tr></td>";
	}
	table += "</table>";
	return table;
}