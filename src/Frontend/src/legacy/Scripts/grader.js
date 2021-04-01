export default function () {
	const $modal = $('.grader-submissions__details__modal');

	$('.grader-submissions__details__link').click(function () {
		const $self = $(this);
		const $loadingIcon = $self.parent().find('.loading-icon');
		$loadingIcon.css('visibility', 'visible');

		$.get($self.data('url')).done(function (data) {
			$modal.find('.modal-body').html(data);
			$modal.modal('show');
		}).always(function () {
			$loadingIcon.css('visibility', 'hidden');
		});
	});
}
