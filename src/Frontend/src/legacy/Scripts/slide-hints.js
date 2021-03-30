window.documentReadyFunctions = window.documentReadyFunctions || [];

window.documentReadyFunctions.push(function() {
	$(".exercise__submission").on('click', '#GetHintButton', function() {
		showHintForUser($(this));
	});
});

function showHintForUser($button) {
	const slideId = $button.data("slide-id");
	$.ajax({
        type: "POST",
        url: $button.data("url"),
        data: { courseId: $button.data("course-id"), slideId: slideId, isNeedNewHint: true }
    }).done(function(ans) {
        if (ans.Text === "Для слайда нет подсказок") {
        	$("#GetHintButton").notify(ans.Text, "noHints");
            return;
        } else if (ans.Text === "Подсказок больше нет") {
        	$("#GetHintButton").notify(ans.Text, "noHints");
            return;
        } else {
            $('#hints-place').html(ans);
            if (!isScrolledIntoView('#hints-place')) {
				const container = $('body');
				container.animate({
                    scrollTop: $('#hints-place').offset().top - container.offset().top + container.scrollTop() - 200
                });
            }
        }
        buttonNameChange($button.data("hints-count"));
    }).fail(function(req) {
        console.log("FAIL", req.responseText);
    }).always(function(ans) {
    });
}

function isScrolledIntoView(elem) {
	const docViewTop = $(window).scrollTop();
	const docViewBottom = docViewTop + $(window).height();
	const elemTop = $(elem).offset().top;
	const elemBottom = elemTop + $(elem).height();

	return ((elemBottom < docViewBottom) && (elemTop > docViewTop));
}

window.getHints = function getHints(courseId, slideId, hintsCount) {
	/* Exit if no hint button */
	if (!$('#GetHintButton').length)
		return;
    $.ajax({
        type: "POST",
        url: $("#GetHintButton").data("url"),
        data: { courseId: courseId, slideId: slideId, isNeedNewHint: false }
    }).done(function (ans) {
        if (ans.Text === "Для слайда нет подсказок") {
        	$("#GetHintButton").text(ans.Text);
            $("#GetHintButton").attr('disabled', 'disabled');
            return;
        } else if (ans.Text === "Подсказок больше нет") {
        	$("#GetHintButton").text(ans.Text);
            return;
        } else {
            $('#hints-place').html(ans);
        }
        buttonNameChange(hintsCount);
    }).fail(function (req) {
        console.log("FAIL", req.responseText);
    }).always(function (ans) {
    });

}


function buttonNameChange(hintsCount) {
	let showedHintsCount = 0;
	$(".like-button").each(function () {
        showedHintsCount++;
    });
    if (showedHintsCount < hintsCount && showedHintsCount != 0)
        $("#GetHintButton").text("Следующая подсказка");
    else if (showedHintsCount != 0) {
    	$("#GetHintButton").text("Подсказок больше нет");
        $("#GetHintButton").attr('disabled', 'disabled');
    }
}

window.likeHint = function likeHint(courseId, slideId, hintId) {
    $.ajax({
        type: "POST",
        url: $("#" + hintId + "likeHint").data("url"),
        data: { courseId: courseId, slideId: slideId, hintId: hintId }
    }).done(function (ans) {
        if (ans == "success") {
            $("#" + hintId + "likeHint").removeClass("btn-default").addClass("btn-primary");
        }
        if (ans == "cancel") {
            $("#" + hintId + "likeHint").removeClass("btn-primary").addClass("btn-default");
        }
    });
}
