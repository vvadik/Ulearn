var slideNavigation = {
	$prev: $("#prev_slide_button"),
	$next: $("#next_slide_button"),
	$nextSolutions: $("#next_solutions_button"),
	$nextButtons: $(".next_button"),
	update: function (hasNext, hasPrev, isAccepted) {
		this.$next.removeClass("block-next");
		this.$next.toggle(hasNext);
		if (!hasNext) {
			this.$nextButtons.toggle(false);
		}
		this.$prev.toggle(hasPrev);
		if (!hasPrev) {
			this.$prev.toggle(false);
		}
		this.$nextSolutions.toggle(false);
		if (isAccepted) {
			this.$nextSolutions.toggle(true);
			this.$next.toggle(false);
		}
	},
	makeShowSolutionsNext: function () {
		this.$next.toggle(false);
		this.$nextSolutions.toggle(true);
	}
}
var $parent = $("#nav_arrows");
slideNavigation.update($parent.data("hasnext"), !!$parent.data("hasprev"), $parent.data("isaccepted"));

function HandleNextButtonClick(event) {
	var rated = !($("#notwatched").hasClass("not-watched"));
	if (rated)
		$(event.currentTarget).removeClass("block-next");
	else {
		setTimeout(function () {
			$("#ratings").removeClass("bounce-effect");
		}, 1000);
		$("#ratings").addClass("bounce-effect");
	}
	return rated;
}

slideNavigation.$nextButtons.click(HandleNextButtonClick);