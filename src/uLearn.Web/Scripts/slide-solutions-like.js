function likeSolution(solutionId) {
	$.ajax({
		type: "POST",
		url: $("#LikeSolutionUrl").data("url"),
		data: String(solutionId)
	}).success(function (ans) {
		var likerCounterId = "#counter" + solutionId;
		var likeCounter = $(likerCounterId);
		if (ans == "success") {
		    var lastVal = likeCounter.text();
		    var newVal = parseInt(lastVal) + 1;
		    var strNewVal = "   " + String(newVal);
			likeCounter.text(strNewVal);
		} else {
			likeCounter.text("already like from you");
		}
	});
}