window.toggleRoleOrCourseAccess = function ToggleRoleOrCourseAccess(comment) {
	var target = window.toggleRoles.target;
	var toggleClass = window.toggleRoles.toggleClass;
	
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
};

function ToggleSystemRoleOrAccess(target, toggleClass) {

	var $object = $(target);

	var url = $object.data("toggleUrl");

	var token = $('#AntiForgeryTokenContainer input[name="__RequestVerificationToken"]').val();


	$.ajax({
		url: url,
		method: "POST",
		data: {
			__RequestVerificationToken: token,
			isEnabled: !$object.data('hasAccess'),
		},
		dataType: 'json',
	})
		.done(function (result) {
			if (result.status !== 'ok') {
				console.error(result.message);
				return;
			}
			$object.data('hasAccess', !$object.data('hasAccess'));
			toggleClass($object);
		});
};

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

function openPopup(target, toggleClass) {

	if (($(target)).data("css-class") === "btn-danger") {
		ToggleSystemRoleOrAccess(target, toggleClass);
		return;
	}


	var root = document.querySelector('.react-rendered');
	
	root.setAttribute('modalOpened', true);

	window.toggleRoles = {
		target: target,
		toggleClass: toggleClass,
	};
}

function SubmitComment(targ) {
	closePopup();

	var event = $('.popup-fade').data["event"];
	var target = $('.popup-fade').data["target"];
	var toggleClass = $('.popup-fade').data["toggleClass"];
	var comment = $('.grantCommentField').val();
	if (comment.length !== 0) {
		var lineLength = $('.grantCommentField').attr('cols');


		//comment = GetFormattedStringLines(lineLength,comment);
		console.log(comment);
		ToggleRoleOrCourseAccess(event, target, toggleClass, comment);
		closePopup()
	}


}

function closePopup() {
	var root = document.querySelector('.react-rendered');
	root.setAttribute('modalOpened', false);
}

