window.documentReadyFunctions = window.documentReadyFunctions || [];

window.documentReadyFunctions.push(function() {
	const $modal = $('#suggest-mail-transport__modal');
	if ($modal.length)
		$modal.modal();
});
