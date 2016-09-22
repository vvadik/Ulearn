function updateExerciseVersionUrl(versionId) {
	var newSearch = $.query.set('version', versionId).toString();
	var newurl = window.location.protocol + "//" + window.location.host + window.location.pathname + newSearch;
	if (history.pushState)
		window.history.pushState({ path: newurl }, '', newurl);
}

function setExerciseVersion(versionId) {
	var url = $('.exercise__submission').data('version-update-url');
	url = url.replace('VERSION_ID', versionId);

	saveExerciseCodeDraft();

	var $hints = $('#hints-accordion');
	$('.exercise__submission > *:not(.exercise__submissions-panel)').hide();
	var $loadingPanel = $('<p class="exercise-loading">Загрузка...</p>');
	$('.exercise__submission').append($loadingPanel);
	$hints.hide();

	$.get(url, function(data) {
		var $submission = $('.exercise__submission');
		$loadingPanel.hide();
		$submission.html($(data).html());
		initCodeEditor($submission);
		$submission.find('.select-auto-width').each(function() {
			selectSetAutoWidth($(this));
		});
		refreshPreviousDraft();

		updateExerciseVersionUrl(versionId);
		$hints.show();
	});
}

$(document).ready(function () {
	$('.exercise__submission').on('click', '.exercise-version-link', function (e) {
		e.preventDefault();

		var $self = $(this);
		var versionId = $self.data('version-id');
		setExerciseVersion(versionId);
	});

	$('.exercise__submission').on('change', '[name=version]', function() {
		var $self = $(this);
		setExerciseVersion($self.val());
	});
});