function ToggleRole(event, target) {
	event.stopPropagation();
	var obj = $(target);
	var url = obj.data("toggle-url");
	var token = $('#AntiForgeryTokenContainer input[name="__RequestVerificationToken"]').val();
	$.ajax({
			url: url,
			method: "POST",
			data: {
				__RequestVerificationToken: token
			}
		})
		.success(function() {
			ToggleClasses(obj);
		});
}

function ToggleClasses(obj) {
	obj.toggleClass(obj.data("css-class"));
	var parents = obj.parents();
	for (var ind = 0; ind < parents.length; ++ind) {
		var parent = $(parents[ind]);
		if (parent.hasClass("role-btn-group"))
			break;
		if (!parent.data("css-class"))
			continue;
		parent.removeClass(parent.data("css-class"));
		var children = parent.children("[data-css-class]");
		for (var i = 0; i < children.length; ++i) {
			var child = $(children[i]);
			if (child.hasClass(child.data("css-class")))
				parent.addClass(parent.data("css-class"));
		}
	}
}