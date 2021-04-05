export default function () {
	$('.slide-diff-table').each(function () {
		const $self = $(this);
		$self.html(diffHtml($self.data('original'), $self.data('changed'), 'Старый блок', 'Новый блок', true));
	});

	$('.expand-slide-diff-link').click(function (e) {
		e.preventDefault();
	});
}
