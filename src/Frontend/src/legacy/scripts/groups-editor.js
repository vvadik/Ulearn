﻿export default function () {
	const updateMassOperationsControlsState = function () {
		const $countChecked = $('.modal__edit-group__members').find('.group__member.checked').length;
		const $massOperationsPanel = $('.modal__edit-group__mass-operations');
		const $massOperationsLabel = $massOperationsPanel.find('.group__member__remove-all-checkbox + label');
		const $removeAllCheckbox = $massOperationsPanel.find('.group__member__remove-all-checkbox');
		const $massOperationsInputs = $massOperationsPanel.find('button,select');
		if ($countChecked > 0) {
            $massOperationsInputs.show();
            $massOperationsLabel.text('');

            /* If no group selected for copying disable button anymore */
			const copyToGroupId = $('.modal__edit-group__mass-operations select[name="copyToGroupId"]').val();
			const $copyButton = $massOperationsInputs.filter('.modal__edit-group__mass-copy-button');
			if (copyToGroupId === '-1')
                $copyButton.attr('disabled', 'disabled');
            else
                $copyButton.removeAttr('disabled');
        }
        else {
            $removeAllCheckbox.prop('checked', false);
            $massOperationsInputs.hide();
            $massOperationsLabel.text($massOperationsLabel.data('text'));
        }
    };
	const addUserToGroupUrl = "/Admin/AddUserToGroup";
	const token = $('input[name="__RequestVerificationToken"]').val();

	$('input[name="__RequestVerificationToken"]').each(function() {
		if ($(this).val() === '')
			$(this).val(token);
	});

	const addAntiForgeryToken = function (data) {
		const token = $('#__AjaxAntiForgeryForm input[name=__RequestVerificationToken]').val();
		if(typeof (data) === 'string') {
			return data + "&__RequestVerificationToken=" + token;
		} else {
			data.__RequestVerificationToken = token;
			return data;
		}
	};

	$('.add-user-to-group__input').each(function() {
		const $self = $(this);
		const $error = $('.add-user-to-group__error');
		$self.autocomplete({
			source: $self.data('url'),
			select: function (event, ui) {
				const item = ui.item;
				const groupId = $self.data('groupId');
				$error.text('');
				$.ajax({
					type: 'post',
					url: addUserToGroupUrl,
					data: addAntiForgeryToken({
						groupId: groupId,
						userId: item.id
					})
				}).done(function (data) {
					if (data.status === 'error') {
						$error.text(data.message);
						return;
					}
					const $members = $('#modal__edit-group .modal__edit-group__members');
					$members.find('.modal__edit-group__no-members').remove();
                    $members.append(data.html);
				});

				$self.val('');
				return false;
			}
		});
	});

	$('.create-group-link').click(function (e) {
		e.preventDefault();

		const $modal = $('#modal__create-group__step1');
		$modal.find('.field-validation-error').text();
		$modal.find('input[name="name"]').val('');
		$modal.modal();
	});

	const openModalCreateGroupStep2 = function (groupId) {
		const $modal = $('#modal__create-group__step2');
		$modal.find('input[type="text"]').val();
		$modal.find('input[type="checkbox"]').prop('checked', true);
		$modal.find('input[name="groupId"]').val(groupId);
		$modal.modal();
	};

	$('.modal__create-group__step1__button').click(function(e) {
		e.preventDefault();

		const $modal = $('#modal__create-group__step1');
		const $error = $modal.find('.modal__create-group__step1__name-error');
		const url = $modal.data('createGroupUrl');
		const courseId = $modal.find('input[name="courseId"]').val();
		const name = $modal.find('input[name="name"]').val();
		$.ajax({
			type: 'post',
			url: url,
			data: addAntiForgeryToken({
				courseId: courseId,
				name: name
			}),
			dataType: 'json'
		}).done(function (data) {
			if (data.status === 'error') {
				$error.text(data.message);
				return;
			}

			$modal.modal('hide');
			openModalCreateGroupStep2(data.groupId);
		});
	});

	const openModalCreateGroupStep3 = function (groupId) {
		const $modal = $('#modal__create-group__step3');
		const groupInfoUrl = $modal.data('groupInfoUrl').replace('GROUP_ID', groupId);

		$modal.find('input[type="text"]').val();
		$modal.find('input[type="checkbox"]').prop('checked', true);
		$modal.find('input[name="groupId"]').val(groupId);
		$modal.find('.add-user-to-group__input').data('groupId', groupId);
		$modal.find('.add-user-to-group__error').text('');
		$modal.modal();

		$.getJSON(groupInfoUrl, function (data) {
			if(data.status === 'error') {
				$modal.find('.modal__create-group__step3__error').text(data.message);
				return;
			}
			$modal.find('.modal__create-group__step3__invite-link').text(data.inviteLink).attr('href', data.inviteLink);

			$modal.find('.modal__create_group__step3__invite-link-block').hide();
			$modal.find('.modal__create_group__step3__invite-link-block').filter('[data-enabled="' + data.group.isInviteLinkEnabled + '"]').show();
			$modal.find('.modal__create-group__step3__enable-invite_link').data('groupId', groupId);

			$modal.find('.group__accesses').html(data.accesses.join(''));
		});
	};

	$('.modal__create-group__step2__button').click(function(e) {
		e.preventDefault();

		const $modal = $('#modal__create-group__step2');
		const $form = $modal.find('form');
		const $error = $modal.find('.modal__create-group__step2__error');
		const url = $form.attr('action');

		$.ajax({
			type: 'post',
			url: url,
			data: $form.serialize(),
			dataType: 'json'
		}).done(function (data) {
			if (data.status === 'error') {
				$error.text(data.message);
				return;
			}

			$modal.modal('hide');
			openModalCreateGroupStep3(data.groupId);
		});
	});

	$('#modal__create-group__step2').add('#modal__edit-group').on('change', '[name=manualChecking]', function () {
		const $self = $(this);

		const $form = $self.closest('form');
		const manualChecking = $self.prop('checked');
		$form.find('[name="manualCheckingForOldSolutions"]').prop('checked', manualChecking)
			.closest('.checkbox').toggle(manualChecking);
	});

	$('.modal__group__add-access__input').each(function() {
		const $self = $(this);

		$self.autocomplete({
			source: $self.data('url'),
			select: function (event, ui) {
				const item = ui.item;

				const $form = $self.closest('form');
				$form.find('[name="userId"]').val(item.id);
				const url = $form.attr('action');
				const $error = $form.find('.modal__group__name-error');
				$.ajax({
					type: 'post',
					url: url,
					data: $form.serialize(),
					dataType: 'json'
				}).done(function (data) {
					if (data.status === 'error') {
						$error.text(data.message);
						return;
					}

					$error.text('');
					$form.find('input[type="text"]').val('');

					const $parent = $form.closest('.modal__group__accesses');
					const currentHtml = $parent.find('.group__accesses').html();
					$parent.find('.group__accesses').html(currentHtml + data.html);
				});

				return false;
			}
		});
	});

	$('.group__accesses').on('click', '.group__access__revoke-link', function (e) {
		e.preventDefault();
		const $self = $(this);

		const url = $self.attr('href');
		$.ajax({
			type: 'post',
			url: url,
			data: {},
			dataType: 'json'
		}).done(function(data) {
			if (data.status === 'error') {
				alert(data.message);
				return;
			}

			const $access = $self.closest('.group__access');
			$access.remove();
		});
	});

	$('.modal__create-group__step3__button').click(function(e) {
		e.preventDefault();

		window.location.reload();
	});

	const openModalForEditingGroup = function (groupId) {
		const $modal = $('#modal__edit-group');
		$modal.find('input[name="groupId"]').val(groupId);
		$modal.find('.group__accesses').html('');
		$modal.find('.modal__edit-group__members').html('');
		$modal.find('.modal-header__tabs a').first().click();
		$modal.find('.modal__edit-group__mass-operations').show();
		updateMassOperationsControlsState();
		if(!$modal.is(':visible'))
			$modal.modal();

		const url = $modal.data('groupInfoUrl').replace('GROUP_ID', groupId);
		$.getJSON(url, function (data) {
			if(data.status === 'error') {
				alert(data.message + ". Пожалуйста, обновите страницу");
				return;
			}

			$modal.find('.modal__create-group__step3__invite-link').text(data.inviteLink).attr('href', data.inviteLink);
			$modal.find('.modal__edit-group__group-name').text(data.group.name);
			$modal.find('.group__accesses').html(data.accesses.join(''));

			const $members = $modal.find('.modal__edit-group__members');
			if(data.members.length > 0)
				$members.html(data.members.join(''));
			else {
				$('.modal__edit-group__mass-operations').hide();
				$members.html('<p class="modal__edit-group__no-members">В&nbsp;группе пока нет студентов</p>');
			}

			$modal.find('.modal__create_group__step3__invite-link-block').hide();
			$modal.find('.modal__create_group__step3__invite-link-block').filter('[data-enabled="' + data.group.isInviteLinkEnabled + '"]').show();
			$modal.find('.modal__create-group__step3__enable-invite_link').data('groupId', groupId);

			$modal.find('input[name="name"]').val(data.group.name);
			$modal.find('[name="manualChecking"]').prop('checked', data.group.isManualCheckingEnabled);
			$modal.find('[name="manualCheckingForOldSolutions"]').prop('checked', data.group.isManualCheckingEnabledForOldSolutions).closest('.checkbox').toggle(data.group.isManualCheckingEnabled);
			$modal.find('[name="defaultProhibitFutherReview"]').prop('checked', data.group.defaultProhibitFutherReview);
			$modal.find('[name="canUsersSeeGroupProgress"]').prop('checked', data.group.canUsersSeeGroupProgress);

			$modal.find('.scoring-group-checkbox input').prop('checked', false);
			data.enabledScoringGroups.forEach(function (scoringGroupId) {
				$modal.find('.scoring-group-checkbox [name="scoring-group__' + scoringGroupId + '"]').prop('checked', true);
			});

			$modal.find('.add-user-to-group__input').data('groupId', groupId);
			$modal.find('.add-user-to-group__error').text('');
		});
	};

	$('.groups .group').click(function (e) {
		const $target = $(e.target);
		if ($target.closest('a').length > 0 || $target.closest('select').length > 0 || $target.closest('button').length > 0 || $target.closest('.dropdown-backdrop').length > 0)
			return;

		e.preventDefault();

		const $self = $(this);
		const groupId = $self.data('groupId');

		openModalForEditingGroup(groupId);
	});

	$('.modal__edit-group__members').on('click', '.group__member__remove-link', function (e) {
		e.preventDefault();
		const $self = $(this);

		const url = $self.attr('href');
		$.ajax({
			type: 'post',
			url: url,
			data: {},
			dataType: 'json'
		}).done(function (data) {
			if (data.status === 'error') {
				alert(data.message);
				return;
			}

			const $member = $self.closest('.group__member');
			$member.remove();
		});
	});


	$('.modal__edit-group__members').on('change', '.group__member__remove-checkbox', function (e) {
		const $self = $(this);
		const $groupMember = $self.closest('.group__member');
		$groupMember.toggleClass('checked');

        updateMassOperationsControlsState();
    });
	
	$('.group__member__remove-all-checkbox').change(function (e) {
		const $self = $(this);
		const $checkboxes = $self.closest('#modal__edit-group__members').find('.group__member__remove-checkbox');

		const isChecked = $self.prop('checked');
		$checkboxes.prop('checked', isChecked);
		const $groupMembers = $checkboxes.closest('.group__member');
		$groupMembers.toggleClass('checked', isChecked);
		
        updateMassOperationsControlsState();
    });
	
	$('.modal__edit-group__mass-remove-button').click(function(e) {
	    e.preventDefault();
		const $self = $(this);

		const url = $self.data('url');
		const $modal = $('#modal__edit-group');
		const groupId = $modal.find('input[name="groupId"]').val();
		const $membersToRemove = $('.modal__edit-group__members').find('.group__member.checked');
		$.ajax({
            type: 'post',
            url: url,
            data: {
                'groupId': groupId,
                'userId': $membersToRemove.map(function (_, el) { return $(el).data('userId'); }).get().join(",")
            },
            dataType: 'json'
        }).done(function (data) {
            if (data.status === 'error') {
                alert(data.message);
                return;
            }

            $membersToRemove.remove();
            updateMassOperationsControlsState()
        });
    });

    $('.modal__edit-group__mass-copy-button').click(function(e) {
        e.preventDefault();
		const $self = $(this);

		const url = $self.data('url');
		const $modal = $('#modal__edit-group');
		const fromGroupId = $modal.find('input[name="groupId"]').val();
		const toGroupId = $modal.find('.modal__edit-group__mass-operations').find('select[name="copyToGroupId"]').val();
		const $membersToCopy = $('.modal__edit-group__members').find('.group__member.checked');
		$.ajax({
            type: 'post',
            url: url,
            data: {
                'fromGroupId': fromGroupId,
                'toGroupId': toGroupId,
                'userId': $membersToCopy.map(function (_, el) { return $(el).data('userId'); }).get().join(",")
            },
            dataType: 'json'
        }).done(function (data) {
            if (data.status === 'error') {
                alert(data.message);
                return;
            }

			const $status = $('.modal__edit-group__mass-copy-status');
			$status.addClass('text-success').text('Скопировано!');
            setTimeout(function () {
                $status.text('').removeClass('text-success');
            },
            1000);
        });
    });
    
    $('.modal__edit-group__mass-operations select[name="copyToGroupId"]').change(function (e) {
		const $self = $(this);
		const val = $self.val();
		const $button = $self.closest('.modal__edit-group__mass-operations').find('.modal__edit-group__mass-copy-button');
		if (val === '-1')
            $button.attr('disabled', 'disabled');
        else
            $button.removeAttr('disabled');
    });
	
	$('.modal__edit-group__button').click(function (e) {
		e.preventDefault();
		const $modal = $('#modal__edit-group');

		const $form = $modal.find('#modal__edit-group__settings form');
		const $error = $modal.find('.modal__edit-group__error');
		const url = $form.attr('action');

		$.ajax({
			type: 'post',
			url: url,
			data: $form.serialize(),
			dataType: 'json'
		}).done(function (data) {
			if (data.status === 'error') {
				$error.text(data.message);
				return;
			}

			$modal.modal('hide');
			window.location.reload();
		});

	});

	$('#createOrUpdateGroupModal').on('change', '[name=manualChecking]', function() {
		const $form = $(this).closest('form');
		const manualChecking = $(this).prop('checked');
		$form.find('[name="manualCheckingForOldSolutions"]').prop('checked', manualChecking)
			.closest('.checkbox').toggle(manualChecking);
	});

	$('.remove-group-link').click(function(e) {
		e.preventDefault();

		const $self = $(this);
		const groupId = $self.data('groupId');
		$.ajax({
			type: 'post',
			url: $self.data('url'),
			data: {
				__RequestVerificationToken: token,
				groupId: groupId,
			}
		}).done(function() {
			window.location.reload(true);
		});
	});

	$('.group__accesses').on('click', '.group__access__change-owner-link', function(e) {
		e.preventDefault();

		const $self = $(this);
		const $form = $self.closest('form');
		const url = $form.attr('action');

		$.ajax({
			type: 'post',
			url: url,
			data: $form.serialize(),
			dataType: 'json'
		}).done(function (data) {
			if (data.status === 'error') {
				alert(data.message);
				return;
			}

			openModalForEditingGroup(data.groupId);
		});
	});
	
	$('.modal__create-group__step3__enable-invite_link').click(function (e) {
	    e.preventDefault();

		const $self = $(this);
		const groupId = $self.data('groupId');
		const url = $self.data('url').replace('GROUP_ID', groupId);
		const $modal = $self.closest('.modal');

		$.ajax({
            type: 'post',
            url: url, 
            dataType: 'json',
        }).done(function (data) {
            if (data.status !== 'ok') {
                alert(data.message);
                return;
            }

            $modal.find('.modal__create_group__step3__invite-link-block').hide();
            $modal.find('.modal__create_group__step3__invite-link-block').filter('[data-enabled="' + data.isEnabled + '"]').show();
        });
    });

	/* Group copying */

	const toggleCopyGroupButton = function (canSubmit) {
		const $form = $('#copyGroupModal form');
		if(canSubmit)
			$form.find('.copy-group-button').removeAttr('disabled');
		else
			$form.find('.copy-group-button').attr('disabled', true);
	};

	$('.copy-group-link').click(function(e) {
		e.preventDefault();

		const $form = $('#copyGroupModal form');
		$form.find('select').val('-1');
		$form.find('.copy-group__change-owner-block').hide();
		toggleCopyGroupButton(false);

		$('#copyGroupModal').modal();
	});

	$('#copyGroupModal form select').change(function() {
		const $form = $('#copyGroupModal form');
		const $self = $(this);
		const $option = $self.find('option:selected');

		const $changeOwnerBlock = $form.find('.copy-group__change-owner-block');
		const needToChangeOwner = $option.data('needToChangeOwner');
		$changeOwnerBlock.find('input[name="changeOwner"]').prop('checked', false);
		$changeOwnerBlock.find('.owner-name').text($option.data('owner'));
		$changeOwnerBlock.toggle(needToChangeOwner);

		const canSubmit = $self.val() !== '-1' && !needToChangeOwner;
		toggleCopyGroupButton(canSubmit);
	});

	$('#copyGroupModal form [name="changeOwner"]').change(function() {
		toggleCopyGroupButton($(this).prop('checked'));
	});


	/* Archived groups */

	$('.show-archived-groups-selector > .btn').click(function(e) {
		const $self = $(this);
		if ($self.hasClass('active')) {
			e.preventDefault();
			return false;
		}

		$self.parent().find('.btn').removeClass('active');
		$self.addClass('active');
		$('.groups .group').toggle();
	});
}
