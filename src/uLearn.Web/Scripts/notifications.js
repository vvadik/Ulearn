$(document).ready(function() {
	$('.notifications__icon-link').click(function() {
		var $self = $(this);
		var $dropdown = $self.closest('.dropdown');
		if (!$dropdown.hasClass('open')) {
			var loadUrl = $self.data('notificationsUrl');
			var $dropdownMenu = $dropdown.find('.notifications__dropdown');
			$dropdownMenu.text('.');
			$dropdownMenu.load(loadUrl);
		}
	});

	$('.notifications__notification').click(function(e) {
		e.preventDefault();
		var link = $(this).find('> *').data('href');
		if (link) {
			window.location.href = link;
		}
	});
});