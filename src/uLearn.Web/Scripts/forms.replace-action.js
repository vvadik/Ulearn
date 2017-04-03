$(document).ready(function() {
	$(document).on('click', 'button[data-replace-action],a[data-replace-action]', function (e) {
		var $self = $(this);

		var $form = $self.closest('form');
		if ($form.length === 0)
			return;

		e.preventDefault();
		var newAction = $self.data('replaceAction');
		$form.attr('action', newAction);
		$form.submit();
	});
});