$(document).ready(function() {
	$('.profile__change-email__link').click(function (e) {
		e.preventDefault();

		var $self = $(this);
		var $formGroup = $self.closest('.form-group');
		var $emailInput = $('.profile__email-input');

		$formGroup.hide();
		$emailInput.show();
	});
});