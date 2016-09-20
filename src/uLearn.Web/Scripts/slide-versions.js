function setExerciseVersion(versionId) {
	var url = $('.exercise__submission').data('version-update-url');
	url = url.replace('VERSION_ID', versionId);

	saveExerciseCodeDraft();

	$.get(url, function(data) {
		var $submission = $('.exercise__submission');
		$submission.html($(data).html());
		initCodeEditor($submission);
		selectSetAutoWidth($submission.find('.select-auto-width'));
		setAutoUpdater($submission.find('.js__auto-update'));
		refreshPreviousDraft();
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