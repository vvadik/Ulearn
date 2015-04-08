function handleRate(rate) {
	if (rate == "NotUnderstand")
		if (!$("#notunderstand").hasClass("btn-danger"))
			$("#ask_question_window").click();
	$("#notwatched").removeClass("not-watched");
	$("#ratings").removeClass("bounce-effect");
	$.ajax(
	{
		type: "POST",
		url: $("#ApplyRate").data("url"),
		data: { rate: rate }
	}).success(function (ans) {
		if (ans == "success") {
			fillRate(rate);
		}
		if (ans == "cancel") {
			cancelRate(rate);
		}
	})
	.fail(function (req) {
	})
	.always(function (ans) {
	});
};

var rateButtons = {
	colors: {
		good: "btn-success",
		notunderstand: "btn-danger",
		trivial: "btn-info"
	}
};

function cancelRate(rate) {
	var switcher = rate.toLowerCase();
	$("#notwatched").addClass("not-watched");
	slideNavigation.$nextButtons.addClass("block-next");
	$("#" + switcher).button('toggle');
	$("#" + switcher).removeClass(rateButtons.colors[switcher]).removeClass("active");
	$("#notwatched").addClass("active");
};

function fillRate(rate) {
	var switcher = rate.toLowerCase();
	var rated = false;
	if (rateButtons.colors[switcher] != undefined)
		rated = true;
	for (var i in rateButtons.colors) {
		$("#" + i).removeClass(rateButtons.colors[i]);
	}
	if (rated) {
		slideNavigation.$nextButtons.removeClass("block-next");
	} else {
		$("#notwatched").addClass("not-watched");
		slideNavigation.$nextButtons.addClass("block-next");
	}
	$("#" + switcher).button('toggle');
	$("#" + switcher).addClass(rateButtons.colors[switcher]);
};