$(document).ready(function () {
	var $exerciseScoreForm = $('.exercise__score-form');
	var $exerciseSimpleScoreForm = $('.exercise__simple-score-form');
	var isSimpleScoreForm = $exerciseSimpleScoreForm.length > 0;
	var $scoreBlock = $('.exercise__score');
	var $otherScoreLink = $scoreBlock.find('.exercise__other-score-link');
	// TODO: multiple $otherScoreInput on page with simple score forms
	var $otherScoreInput = $scoreBlock.find('[name=exerciseScore]');

	var setLockTimeout = function ($lock) {
		$lock[0].lockTimeout = setTimeout(function () {
			$lock.animate({ left: 15 });
			$lock.closest('.user-submission').find('.status').text('');
		}, 8000);
	}

	var clearLockTimeout = function ($lock) {
		var lockTimeout = $lock[0].lockTimeout;
		if (lockTimeout)
			clearTimeout(lockTimeout);
	}

	var sendSimpleScore = function ($scoreForm, ignoreNewestSubmission) {
		ignoreNewestSubmission = ignoreNewestSubmission || false;
		var $status = $scoreForm.find('.status');
		var $userSubmissionInfo = $scoreForm.closest('.user-submission').find('.user-submission__info');
		$status.removeClass('success').removeClass('error').text('Сохраняем..').addClass('waiting');

		var postData = $scoreForm.serialize();
		if (ignoreNewestSubmission)
			postData += "&ignoreNewestSubmission=true";
		if ($scoreForm.data('checkingId'))
			postData += "&updateCheckingId=" + parseInt($scoreForm.data('checkingId'));

		clearLockTimeout($userSubmissionInfo);

		$.ajax({
			type: 'post',
			url: $scoreForm.attr('action'),
			data: postData
		}).success(function (data) {
			if (data.status && data.status !== 'ok') {
				$status.addClass('error');
				var error = '';
				if (data.error === 'has_newest_submission') {
					error = 'Пользователь успел отправить ещё одно решение по&nbsp;этой задаче {NEW_SUBMISSION_DATE}. Поставить баллы ' +
						'<a href="#" data-submission-id="{SUBMISSION}" class="simple-score-link internal-page-link">старому решению</a>, ' +
						'<a href="#" data-submission-id="{NEW_SUBMISSION}" class="simple-score-link internal-page-link">новому</a> ' +
						'или <a href="#" class="cancel-link internal-page-link">отменить</a>?';
					error = error.replace('{SUBMISSION}', $scoreForm.find('[name=submissionId]').val())
						.replace('{NEW_SUBMISSION}', data.submissionId)
						.replace('{NEW_SUBMISSION_DATE}', data.submissionDate);
				} else if (data.error === 'has_greatest_score') {
					error =
						'Пользователь имеет за&nbsp;код-ревью по&nbsp;этой задаче больше баллов: {SCORE}. Новыми баллами вы&nbsp;<strong>не&nbsp;понизите</strong> его суммарную оценку. <a href="{URL}" target="_blank">Предыдущие код-ревью</a>.';
					error = error.replace('{SCORE}', data.score).replace('{URL}', data.checkedQueueUrl);
				}
				$status.html(error);
			} else {
				$status.addClass('success').text('Сохранено: ' + data.score);
				$scoreForm.data('checkingId', data.checkingId);
				$userSubmissionInfo.find('.total-score').text(data.totalScore);
				/* Lock after one second */
				setTimeout(function () {
					$status.text('');
					$userSubmissionInfo.animate({ left: 15 });
				}, 1000);
			}
		}).always(function() {
			$status.removeClass('waiting');
		});
	}

	$scoreBlock.on('click', '.simple-score-link', function(e) {
		e.preventDefault();
		var $self = $(this);
		var submissionId = $self.data('submissionId');
		var $form = $self.closest('.exercise__simple-score-form');
		clearLockTimeout($self.closest('.user-submission').find('.user-submission__info'));
		$form.find('[name=submissionId]').val(submissionId);
		sendSimpleScore($form, true);
	});

	$scoreBlock.on('click', '.cancel-link', function (e) {
		e.preventDefault();
		$(this).closest('.status').text('');
	});

	$scoreBlock.on('click', '.exercise__other-score-link', function (e) {
		e.preventDefault();
		$scoreBlock.find('.btn-group .btn').removeClass('active');
		$otherScoreInput.show();
		$otherScoreInput.focus();
		$otherScoreLink.addClass('active');
	});

	$scoreBlock.find('.btn-group').on('click', '.btn', function () {
		var $self = $(this);
		var wasActive = $self.hasClass('active');
		var $btnGroup = $self.closest('.btn-group');

		$btnGroup.find('.btn').removeClass('active');
		if (isSimpleScoreForm) {
			$otherScoreInput.val($self.data('value'));
			$self.addClass('active');

			var $scoreForm = $self.closest('.exercise__simple-score-form');
			sendSimpleScore($scoreForm);
		} else {
			$self.toggleClass('active', !wasActive);

			$otherScoreInput.hide();
			$otherScoreLink.removeClass('active');
			$otherScoreInput.val(wasActive ? "" : $self.data('value'));
		}
	});

	$exerciseScoreForm.find('input[type=submit]').click(function() {
		if ($otherScoreInput.is(':invalid')) {
			$otherScoreInput.show();
			$otherScoreLink.addClass('active');
		}
	});

	$('.exercise__add-review').each(function () {
		var $topReviewComments = $('.exercise__top-review-comments');
		if ($topReviewComments.find('.comment').length == 0) {
			$(this).addClass('without-comments');
			return;
		}
		var $topComments = $topReviewComments.clone(true).removeClass('hidden');
		$('.exercise__add-review__top-comments').append($topComments);
	});

	$('.exercise__top-review-comments .comment a').click(function(e) {
		e.preventDefault();
		$('.exercise__add-review__comment')
			.val($(this).data('value'))
			.trigger('input');
	});
	
	$('.user-submission__info').bind('move', function (e) {
		var $self = $(this);
		var left = parseInt($self.css('left'));
		$self.css({ left: left + e.deltaX });
	}).bind('moveend', function(e) {
		var $self = $(this);
		if (e.distX < 50)
			$self.animate({ left: 15 });
		else {
			/* Unlock */
			$self.animate({ left: $(window).width() + 15 });
			/* And lock after 8 seconds */
			setLockTimeout($self);
		}
	});

	$('.exercise__simple-score-form').bind('move', function(e) {
		var $submissionInfo = $(this).closest('.user-submission').find('.user-submission__info');
		var left = parseInt($submissionInfo.css('left'));
		$submissionInfo.css({ left: left + e.deltaX });
	}).bind('moveend', function (e) {
		var $submissionInfo = $(this).closest('.user-submission').find('.user-submission__info');
		clearLockTimeout($submissionInfo);
		/* Unlock or lock */
		if (Math.abs(e.distX) < 50) {
			$submissionInfo.animate({ left: $(window).width() + 15 });
			setLockTimeout($submissionInfo);
		} else {
			$submissionInfo.animate({ left: 15 });
			$submissionInfo.closest('.user-submission').find('.status').text('');
		}
	});
});