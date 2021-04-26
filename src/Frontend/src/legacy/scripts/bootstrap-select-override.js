import 'bootstrap-select';
import 'bootstrap-select/dist/css/bootstrap-select.min.css';

export default function () {
	const picker = $('.selectpicker');
	picker.selectpicker();

	picker.on('rendered.bs.select', function () {
		$(this).removeClass('form-control');
	});

	picker.on('changed.bs.select', function (e, index, newValue, oldValue) {
		const $self = $(this);

		const $selectedOption = $($self.find('option')[index]);
		if($selectedOption.data['exclusive'] && newValue) {
			$self.selectpicker('deselectAll');
			$self.val($selectedOption.val());
			$self.selectpicker('render');
			/* Close menu */
			$self.trigger('click');
		}

		if(!$selectedOption.data['exclusive'] && newValue) {
			if($self.find('option[data-exclusive="true"]:selected').length) {
				$self.selectpicker('deselectAll');
				$self.val($selectedOption.val());
				$self.selectpicker('render');
			}
		}
	});
}
