function ToggleRoleOrCourseAccess(event, target, toggleClass) {
	event.stopPropagation();
	var $object = $(target);
	var url = $object.data("toggleUrl");
	var token = $('#AntiForgeryTokenContainer input[name="__RequestVerificationToken"]').val();
	$.ajax({
			url: url,
			method: "POST",
			data: {
				__RequestVerificationToken: token,
				isEnabled: ! $object.data('hasAccess')
			},
			dataType: 'json',
		})
		.done(function (result) {
			if (result.status !== 'ok') {
				alert(result.message);
				return;
			}
			$object.data('hasAccess', ! $object.data('hasAccess'));
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
