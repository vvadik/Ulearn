window.likeSolution = function likeSolution(solutionId) {
	$.ajax({
		type: "POST",
		url: $("#LikeSolutionUrl").data("url"),
		data: { solutionId: solutionId }
	}).done(function (ans) {
		const $block = $("#solution_" + solutionId);
		$block.find(".likes-counter").text(ans.likesCount);
		$block.find(".like-button")
			.toggleClass("btn-default", !ans.liked)
			.toggleClass("btn-primary", ans.liked);
	});
}
