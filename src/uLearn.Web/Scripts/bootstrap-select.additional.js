$(document).ready(function () {
	$('.selectpicker').removeClass('form-control');

	$('.selectpicker').on('changed.bs.select', function(e, index, newValue, oldValue) {
		var $self = $(this);

		var $selectedOption = $($self.find('option')[index]);
		if ($selectedOption.data('exclusive') && newValue) {
			$self.selectpicker('deselectAll');
			$self.val($selectedOption.val());
			$self.selectpicker('render');
			$self.selectpicker('hide');
		};

		if (!$selectedOption.data('exclusive') && newValue) {
			if ($self.find('option[data-exclusive="true"]:selected').length) {
				$self.selectpicker('deselectAll');
				$self.val($selectedOption.val());
				$self.selectpicker('render');
			}
		}
	});
});