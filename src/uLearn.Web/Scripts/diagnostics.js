$(document).ready(function () {
	$('.slide-diff-table').each(function () {
		var $self = $(this);
		$self.html(diffHtml($self.data('original'), $self.data('changed'), 'Старый блок', 'Новый блок', true));
	});

	$('.expand-slide-diff-link').click(function(e) {
		e.preventDefault();
	});
});