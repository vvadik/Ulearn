function ToggleRoleOrCourseAccess(event, target, toggleClass, comment) {
	event.stopPropagation();
	var $object = $(target);

	var url = $object.data("toggleUrl");

	var token = $('#AntiForgeryTokenContainer input[name="__RequestVerificationToken"]').val();


	$.ajax({
		url: url,
		method: "POST",
		data: {
			__RequestVerificationToken: token,
			isEnabled: !$object.data('hasAccess'),
			comment: comment
		},
		dataType: 'json',
	})
		.done(function (result) {
			if (result.status !== 'ok') {
				alert(result.message);
				return;
			}
			$object.data('hasAccess', !$object.data('hasAccess'));
			toggleClass($object);
		});
}

function ToggleButtonClass(button) {
	button.toggleClass(button.data("css-class"));
}

function ToggleDropDownClass(dropdownElement) {
	var parent = $(dropdownElement.parents()[0]);
	var cssClass = parent.data("css-class");
	dropdownElement.toggleClass(cssClass);
	var elements = dropdownElement.siblings().andSelf();
	var button = $(parent.siblings("button")[0]);
	var buttonCss = button.data("css-class");
	button.removeClass(buttonCss);
	for (var i = 0; i < elements.length; ++i) {
		var elem = $(elements[i]);
		if (elem.hasClass(cssClass)) {
			button.addClass(buttonCss);
			break;
		}
	}
}

function OpenPopup(event, target, toggleClass) {
	if (($(target)).data("css-class") === "btn-danger") {
		ToggleRoleOrCourseAccess(event, target, toggleClass, "");
		return;
	}
	$('.grantCommentField').val("");
	$('.popup-fade').fadeIn();
	$('.popup-fade').data["event"] = event;
	$('.popup-fade').data["target"] = target;
	$('.popup-fade').data["toggleClass"] = toggleClass;


}

function SubmitComment(targ) {

	var event = $('.popup-fade').data["event"];
	var target = $('.popup-fade').data["target"];
	var toggleClass = $('.popup-fade').data["toggleClass"];
	var comment = $('.grantCommentField').val();
	if (comment.length !== 0) {
		var lineLength = $('.grantCommentField').attr('cols');

		
		//comment = GetFormattedStringLines(lineLength,comment);
		console.log(comment);
		ToggleRoleOrCourseAccess(event, target, toggleClass, comment);
		ClosePopup()
	}


}

function ClosePopup() {

	$("div").parents('.popup-fade').fadeOut();


}

function GetFormattedStringLines(lineLength, str) {
	var array = str.match(/[^\s]+/g);
	var result = "";
	var current = 0;
	for (var e in array) {
		current += array[e].length;
		if (current > lineLength) {
			result += '\n';
			current = 0;
		}
		result += array[e] + ' ';
	}
	return result;

}