$(document).ready(function () {
	var $scoreBlock = $('.exercise__score');
	var $otherScoreInput = $scoreBlock.find('[name=exerciseScore]');
	$scoreBlock.on('click', 'a', function (e) {
		e.preventDefault();
		$scoreBlock.find('.btn-group .btn').removeClass('active');
		$otherScoreInput.show();
		$otherScoreInput.focus();
	});

	$scoreBlock.find('.btn-group').on('click', '.btn', function () {
		var isActive = $(this).hasClass('active');
		$scoreBlock.find('.btn-group .btn').removeClass('active');
		$(this).toggleClass('active', !isActive);

		$otherScoreInput.hide();
		$otherScoreInput.val($(this).data('value'));
	});
});