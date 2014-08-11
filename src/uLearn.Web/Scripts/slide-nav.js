var slideNavigation = {
	$prev: $("#prev_slide_button"),
	$next: $("#next_slide_button"),
	$noPrev: $("#no_prev_slide"),
	$noNext: $("#no_next_slide"),
	$nextSolutions: $("#next_solutions_button"),
	update: function (hasNext, hasPrev, isAccepted) {
		$("#next_slide_button").removeClass("block-next");
		this.$next.toggle(hasNext);
		if (!hasNext) {
			this.$prev.width("100%");
			this.$next.toggle(false);
		}
		this.$prev.toggle(hasPrev);
		if (!hasPrev) {
			$("#next_solutions_button").width("100%");
			$("#next_slide_button").width("100%");
		}
		this.$nextSolutions.toggle(false);
		if (isAccepted) {
			this.$nextSolutions.toggle(true);
			this.$next.toggle(false);
		}
	},
	makeShowSolutionsNext: function () {
		this.$next.toggle(false);
		this.$noNext.toggle(false);
		this.$nextSolutions.toggle(true);
	}
}
var $parent = $("#nav_arrows");
slideNavigation.update($parent.data("hasnext"), !!$parent.data("hasprev"), $parent.data("isaccepted"));

document.getElementById('next_slide_button').onclick = function () {
	var rated = !($("#notwatched").hasClass("not-watched"));
	if (rated)
		$("#next_slide_button").removeClass("block-next");
	else {
		setTimeout(function () {
			$("#ratings").removeClass("bounce-effect");
		}, 1000);
		$("#ratings").addClass("bounce-effect");
	}
	return rated;
};