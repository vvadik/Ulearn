function handleRate(rate) {
	if (rate == "NotUnderstand")
		if (!$("#notunderstand").parent().hasClass("btn-danger"))
			$("#ask_question_window").click();
	$("#notwatched").removeClass("not-watched");
	$("#ratings").removeClass("bounce-effect");
	$.ajax(
	{
		type: "POST",
		url: $("#ratesBar").data("url"),
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

function cancelRate(rate) {
	var switcher = rate.toLowerCase();
	var colors = {};
	colors["good"] = "btn-success";
	colors["notunderstand"] = "btn-danger";
	colors["trivial"] = "btn-info";
	$("#notwatched").addClass("not-watched");
	$("#next_slide_button").addClass("block-next");
	$("#" + switcher).parent().button('toggle');
	$("#" + switcher).parent().removeClass(colors[switcher]).removeClass("active");
	$("#notwatched").parent().addClass("active");
};

function fillRate(rate) {
	var switcher = rate.toLowerCase();
	var colors = {};
	colors["good"] = "btn-success";
	colors["notunderstand"] = "btn-danger";
	colors["trivial"] = "btn-info";
	var rated = false;
	if (colors[switcher] != undefined)
		rated = true;
	for (var i in colors) {
		$("#" + i).parent().removeClass(colors[i]);
	}
	console.log(rated);
	if (rated)
		$("#next_slide_button").removeClass("block-next");
	else {
		$("#notwatched").addClass("not-watched");
		$("#next_slide_button").addClass("block-next");
	}
	$("#" + switcher).parent().button('toggle');
	$("#" + switcher).parent().addClass(colors[switcher]);
};