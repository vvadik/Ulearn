function showHintForUser(courseId, slideId, hintsCountStr) {
	var hintsCount = parseInt(hintsCountStr) - 1;
    var index = parseInt(($("#currentHint").text()));
    index++;
    if (index > hintsCount) {
	    $("#GetHintButton").notify("Подсказок больше нет :(", "noHints");
	    return;
    }
    $("#currentHint").text(index);
    $.ajax(
{
    type: "POST",
    url: $("#hint" + index).data("url"),
    data: {
        courseId: courseId, slideId: slideId, hintId: index
    }
}).success(function (ans) {
            $('#hint' + (index)).show();
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
    data: {courseId:courseId, slideId:slideId}
}).success(function (ans) {
    var hints = ans.split(' ');
    if (hints[0] != "")
            $("#currentHint").text(hints.length-1);
            for (var hint in hints)
                $('#hint' + hints[hint]).show();
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