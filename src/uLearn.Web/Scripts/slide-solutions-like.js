function likeSolution(solutionId) {
	$.ajax({
		type: "POST",
		url: $("#LikeSolutionUrl").data("url"),
		data: String(solutionId)
	}).success(function (ans) {
		var likerCounterId = "#counter" + solutionId;
		var likeCounter = $(likerCounterId);;
		if (ans == "success") {
			likeCounter.text(String(parseInt(likeCounter.val()) + 1));
		} else {
			likeCounter.text("already like from you");
		}
	});
}