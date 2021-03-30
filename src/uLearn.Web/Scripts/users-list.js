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
	var elements = dropdownElement.siblings().addBack();
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

	var $object = $(target);
	var userName = $object.data("toggleUsername");
	var roleOrAccess = $object.data("toggleRole");
	var isRole = $object.data("toggleIsrole") === 'True';
	var courseTitle = $object.data("toggleCoursetitle");
	var isGrant = true;
	var className = ((target)).className;
	if (className.indexOf("btn-success") > -1 || className.indexOf("btn-warning") > -1 || className.indexOf("btn-info") > -1) {
		isGrant = false;
	}

	if (className === "li-info" || className === "li-warning" || className === "li-success") {
		isGrant = false;
	}

	window.toggleRoles = {
		target: target,
		toggleClass: toggleClass,
		userName: userName,
		role: roleOrAccess,
		isRole: isRole,
		courseTitle: courseTitle,
		isGrant: isGrant
	};

	var root = document.querySelector('.react-rendered');
	root.setAttribute('modalOpened', true);
}

function closePopup() {
	var root = document.querySelector('.react-rendered');
	root.setAttribute('modalOpened', false);
}

