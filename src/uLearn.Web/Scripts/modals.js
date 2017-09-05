$(document).ready(function() {
	$('.modal-header__tabs').on('click', 'a', function(e) {
		e.preventDefault();

		var $this = $(this);
		var $li = $this.closest('li');
		if ($li.hasClass('active'))
			return;

		var $tabs = $li.closest('.modal-header__tabs');
		$tabs.find('li').removeClass('active');
		$li.addClass('active');
	});
});