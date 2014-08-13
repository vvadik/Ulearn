function showHintForUser(courseId, slideIndex) {
	$.ajax({
		type: "POST",
		url: $("#GetHintButton").data("url"),
		data: {courseId: courseId, slideIndex: slideIndex, isNeedNewHint: true}
	}).success(function (ans) {
		if (ans.Text == "Для слайда нет подсказок") {
			$("#GetHintButton").notify("Для слайда нет подсказок", "noHints");
			return;
		}
		else if (ans.Text == "Подсказок больше нет") {
			$("#GetHintButton").notify("Подсказок больше нет :(", "noHints");
			return;
		} else {
			$('#hints-place').html(ans);
			var container = $('body'),
			scrollTo = $('#hints-place');
			container.animate({ scrollTop: scrollTo.offset().top - container.offset().top + container.scrollTop() - 300 });
		}
	}).fail(function (req) {
    	console.log(req.responseText);
    }).always(function (ans) {
    });
}

function getHints(courseId, slideIndex) {
	$.ajax({
		type: "POST",
		url: $("#GetHintButton").data("url"),
		data: { courseId: courseId, slideIndex: slideIndex, isNeedNewHint: false }
	}).success(function (ans) {
		if (ans.Text == "Для слайда нет подсказок") {
			return;
		}
		else if (ans.Text == "Подсказок больше нет") {
			return;
		} else {
			$('#hints-place').html(ans);
		}
	}).fail(function (req) {
		console.log(req.responseText);
	}).always(function (ans) {
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