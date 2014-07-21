document.onkeydown = NavigateThrough;

var focusInInput = false;

if (document.getElementsByTagName)
	onload = function () {
		var e, i = 0;
		while (e = document.getElementsByTagName('INPUT')[i++]) {
			if (e.type == 'text' || e.type == 'search') e.onfocus = function () { focusInInput = true };
			if (e.type == 'text' || e.type == 'search') e.onblur = function () { focusInInput = false };
		}
		i = 0;
		while (e = document.getElementsByTagName('TEXTAREA')[i++]) {
			e.onfocus = function () { focusInInput = true };
			e.onblur = function () { focusInInput = false };
		}
	};

function NavigateThrough(event) {
	if (window.event) event = window.event;
	if ((event.ctrlKey || event.altKey) && !focusInInput) {
		var href = null;
		switch (event.keyCode ? event.keyCode : event.which ? event.which : null) {
			case 0x27:
				href = $('.nav-right:visible').attr('href');
				break;
			case 0x25:
				href = $('.nav-left:visible').attr('href');
				break;
		}
		if (href)
			document.location = href;
	}
}

var slideNavigation = {
	$prev: $("#prev_slide_button"),
	$next: $("#next_slide_button"),
	$noPrev: $("#no_prev_slide"),
	$noNext: $("#no_next_slide"),
	$nextSolutions: $("#next_solutions_button"),
	update: function (hasNext, hasPrev) {
		this.$next.toggle(hasNext);
		this.$noNext.toggle(!hasNext);
		this.$prev.toggle(hasPrev);
		this.$noPrev.toggle(!hasPrev);
		this.$nextSolutions.toggle(false);
	},
	makeShowSolutionsNext: function () {
		this.$next.toggle(false);
		this.$noNext.toggle(false);
		this.$nextSolutions.toggle(true);
	}
}
var $parent = $("#nav_arrows");
slideNavigation.update($parent.data("hasnext"), !!$parent.data("hasprev"));
