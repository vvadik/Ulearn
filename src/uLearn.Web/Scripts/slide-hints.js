function showHintForUser(courseId, slideId, hintsCountStr) {
	var hintsCount = parseInt(hintsCountStr);
	var index = parseInt(($("#currentHint").text()));
	index++;
	if (hintsCount == 0) {
		$("#GetHintButton").notify("Для слайда нет подсказок", "noHints");
		return;
	}
	if (index >= hintsCount) {
		$("#GetHintButton").notify("Подсказок больше нет :(", "noHints");
		return;
	}
	$("#currentHint").text(index);
	(function ($) {
		$.fn.goTo = function () {
			$('html, body').animate({
				scrollTop: $(this).offset() + 'px'
			}, 'slow');
			return this; // for chaining...
		};
	})(jQuery);

	$.ajax(
{
	type: "POST",
	url: $("#hint" + index).data("url"),
	data: {
		courseId: courseId, slideId: slideId, hintId: index
	}
}).success(function (ans) {
	$('#hint' + (index)).show();
	var container = $('body'),
    scrollTo = $('#hint' + (index));
	container.animate({
		scrollTop: scrollTo.offset().top - container.offset().top + container.scrollTop() - 300
	});
})
    .fail(function (req) {
    	console.log(req.responseText);
    })
    .always(function (ans) {
    });
}

function getHints(courseId, slideId) {
	$.ajax(
{
	type: "POST",
	url: $("#hintsStore").data("url"),
	data: { courseId: courseId, slideId: slideId }
}).success(function (ans) {
	if (ans[0] != "")
		$("#currentHint").text(ans.length - 1);
	for (var hint in ans)
		$('#hint' + ans[hint]).show();
})
    .fail(function (req) {
    	console.log(req.responseText);
    })
    .always(function (ans) {
    });
}

function likeHint(courseId, slideId, hintId) {
	$.ajax({
		type: "POST",
		url: $("#" + hintId + "likeHint").data("url"),
		data: { courseId: courseId, slideId: slideId, hintId: hintId }
	}).success(function (ans) {
		if (ans == "success") {
			$("#" + hintId + "likeHint").removeClass("btn-default").addClass("btn-primary");
		}
		if (ans == "cancel") {
			$("#" + hintId + "likeHint").removeClass("btn-primary").addClass("btn-default");
		}
	});
}