$(document).ready(function () {
	var addUserToGroupUrl = "/Admin/AddUserToGroup";
	var removeUserFromGroupUrl = "/Admin/RemoveUserFromGroup";
	var $form = $('#createOrUpdateGroupModal form');
	var token = $('input[name="__RequestVerificationToken"]').val();

	var addAntiForgeryToken = function (data) {
		var token = $('#__AjaxAntiForgeryForm input[name=__RequestVerificationToken]').val();
		if (typeof (data) === "string") {
			return data + "&__RequestVerificationToken=" + token;
		} else {
			data.__RequestVerificationToken = token;
			return data;
		}
	};

	$('#createOrUpdateGroupModal').on('shown.bs.modal', function () {
		$(this).find('[name="name"]').focus();
	});

	$('.add-user-to-group-input').each(function() {
		var $self = $(this);
		var groupId = $self.data('groupId');
		$self.autocomplete({
			source: $self.data('url'),
			select: function (event, ui) {
				var item = ui.item;
				$.ajax({
					type: 'post',
					url: addUserToGroupUrl,
					data: {
						__RequestVerificationToken: token,
						groupId: groupId,
						userId: item.id
					}
				}).done(function (data) {
					if (data.added) {
						var $members = $('.group[data-group-id=' + groupId + '] .members');

						var $newMember = $('<li class="member"><span class="member__name"></span> ' +
										   '<span class="glyphicon glyphicon-remove  remove-user-from-group-link  text-danger visible-on-parent-hover cursor-pointer" data-group-id="' + groupId + '" data-user-id="' + item.id + '" title="Удалить из группы"></span>' +
										   '</li>');
						$newMember.find(".member__name").text(item.value);
						$members.append($newMember);
					} else {
						alert("Этот пользователь уже в группе");
					}
				});

				$self.val("");
				return false;
			}
		});
	});

	$('.group .members').on('click', '.remove-user-from-group-link', function (e) {
		e.preventDefault();

		var $self = $(this);
		var groupId = $self.data('groupId');
		var userId = $self.data('userId');
		$.ajax({
			type: 'post',
			url: removeUserFromGroupUrl,
			data: {
				__RequestVerificationToken: token,
				groupId: groupId,
				userId: userId
			}
		}).fail(function() {
			alert('При удалении произошла какая-то ошибка');
		});
		$self.parent().remove();
	});

	$('.create-group-link').click(function (e) {
		e.preventDefault();

		var $modal = $('#modal__create-group__step1');
		$modal.find('.field-validation-error').text();
		$modal.find('input[name="name"]').val('');
		$modal.modal();
	});

	
	var openModalCreateGroupStep2 = function (groupId) {
		var $modal = $('#modal__create-group__step2');
		$modal.find('input[type="text"]').val();
		$modal.find('input[type="checkbox"]').prop('checked', true);
		$modal.find('input[name="groupId"]').val(groupId);
		$modal.modal();
	}

	$('.modal__create-group__step1__button').click(function(e) {
		e.preventDefault();

		var $modal = $('#modal__create-group__step1');
		var $error = $modal.find('.modal__create-group__step1__name-error');
		var url = $modal.data('createGroupUrl');
		var courseId = $modal.find('input[name="courseId"]').val();
		var name = $modal.find('input[name="name"]').val();
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

	var openModalCreateGroupStep3 = function (groupId) {
		var $modal = $('#modal__create-group__step3');
		var groupInfoUrl = $modal.data('groupInfoUrl').replace('GROUP_ID', groupId);

		$modal.find('input[type="text"]').val();
		$modal.find('input[type="checkbox"]').prop('checked', true);
		$modal.find('input[name="groupId"]').val(groupId);
		$modal.modal();

		$.getJSON(groupInfoUrl, function(data) {
			if (data.status === 'error') {
				$modal.find('.modal__create-group__step3__error').text(data.message);
				return;
			}
			$modal.find('.modal__create-group__step3__invite-link').text(data.inviteLink).attr('href', data.inviteLink);
			$modal.find('.group__accesses').html(data.accesses.join(''));
		});
	}

	$('.modal__create-group__step2__button').click(function(e) {
		e.preventDefault();

		var $modal = $('#modal__create-group__step2');
		var $form = $modal.find('form');
		var $error = $modal.find('.modal__create-group__step2__error');
		var url = $form.attr('action');

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
		var $self = $(this);

		var $form = $self.closest('form');
		var manualChecking = $self.prop('checked');
		$form.find('[name="manualCheckingForOldSolutions"]').prop('checked', manualChecking)
			.closest('.checkbox').toggle(manualChecking);
	});

	$('.modal__group__add-access-button').click(function(e) {
		e.preventDefault();

		var $form = $(this).closest('form');
		var url = $form.attr('action');
		var $error = $form.find('.modal__group__name-error');
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

			var $parent = $form.closest('.modal__group__accesses');
			var currentHtml = $parent.find('.group__accesses').html();
			$parent.find('.group__accesses').html(currentHtml + data.html);
		});
	});

	$('.group__accesses').on('click', '.group__access__revoke-link', function (e) {
		e.preventDefault();
		var $self = $(this);

		var url = $self.attr('href');
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

			var $access = $self.closest('.group__access');
			$access.remove();
		});
	});

	$('.modal__create-group__step3__button').click(function(e) {
		e.preventDefault();

		window.location.reload();
	});

	/*
	$('.create-group-link').click(function (e) {
		e.preventDefault();

		$form.attr('action', $form.data('createGroupUrl'));
		$form.find('[name="groupId"]').val("");
		$form.find('[name="name"]').val("");
		$form.find('[name="isPublic"]').prop('checked', false);
		var manualCheckingEnabled = $form.find('[name="manualChecking"]').data('defaultValue');
		$form.find('[name="manualChecking"]').prop('checked', manualCheckingEnabled);
		$form.find('[name="manualCheckingForOldSolutions"]').prop('checked', true)
			.closest('.checkbox').toggle(manualCheckingEnabled);
		$form.find('[name="isArchived"]').prop('checked', false).closest('.checkbox').hide();
		$form.find('[name="ownerId"]').val($form.find('[name="ownerId"]').data('defaultValue'));
		$form.find('.scoring-group-checkbox input').prop('checked', false);
		$form.find('button.action-button').text('Создать');
		$form.find('.remove-group-link').hide();
		$('#createOrUpdateGroupModal').modal();
	});
	*/

	$('.groups .group').click(function (e) {
		var $target = $(e.target);
		if ($target.closest('a').length > 0 || $target.closest('select').length > 0 || $target.closest('button').length > 0 || $target.closest('.dropdown-backdrop').length > 0)
			return;

		e.preventDefault();

		var $self = $(this);
		var groupId = $self.data('groupId');

		var $modal = $('#modal__edit-group');
		$modal.find('input[name="groupId"]').val(groupId);
		$modal.find('.group__accesses').html('');
		$modal.find('.modal__edit-group__members').html('');
		$modal.find('.modal-header__tabs a').first().click();
		$modal.modal();

		var url = $modal.data('groupInfoUrl').replace('GROUP_ID', groupId);
		$.getJSON(url, function(data) {
			if (data.status === 'error') {
				alert(data.message + ". Пожалуйста, обновите страницу");
				return;
			}

			$modal.find('.modal__create-group__step3__invite-link').text(data.inviteLink).attr('href', data.inviteLink);
			$modal.find('.modal__edit-group__group-name').text(data.group.name);
			$modal.find('.group__accesses').html(data.accesses.join(''));
			$modal.find('.modal__edit-group__members').html(data.members.join(''));

			$modal.find('input[name="name"]').val(data.group.name);
			$modal.find('[name="manualChecking"]').prop('checked', data.group.isManualCheckingEnabled);
			$modal.find('[name="manualCheckingForOldSolutions"]').prop('checked', data.group.isManualCheckingEnabledForOldSolutions).closest('.checkbox').toggle(data.group.isManualCheckingEnabled);

			$modal.find('.scoring-group-checkbox input').prop('checked', false);
			data.enabledScoringGroups.forEach(function (scoringGroupId) {
				$modal.find('.scoring-group-checkbox [name="scoring-group__' + scoringGroupId + '"]').prop('checked', true);
			});
		});

		/*
		var name = $self.data('name');
		var isPublic = $self.data('isPublic');
		var manualChecking = $self.data('manualChecking');
		var manualCheckingForOldSolutions = $self.data('manualCheckingForOldSolutions');
		var isArchived = $self.data('isArchived');
		var ownerId = $self.data('ownerId');
		var scoringGroupsIds = $self.data('scoringGroups').split(',');

		$form.attr('action', $form.data('updateGroupUrl'));
		$form.find('[name="groupId"]').val(groupId);
		$form.find('[name="name"]').val(name);
		$form.find('[name="isPublic"]').prop('checked', isPublic);
		$form.find('[name="manualChecking"]').prop('checked', manualChecking);
		$form.find('[name="manualCheckingForOldSolutions"]').prop('checked', manualCheckingForOldSolutions)
			.closest('.checkbox').toggle(manualChecking);
		$form.find('[name="isArchived"]').prop('checked', isArchived).closest('.checkbox').show();
		$form.find('[name="ownerId"]').val(ownerId);
		$form.find('.scoring-group-checkbox input').prop('checked', false);
		scoringGroupsIds.forEach(function (scoringGroupId) {
			$form.find('.scoring-group-checkbox [name="scoring-group__' + scoringGroupId + '"]').prop('checked', true);
		});
		$form.find('button.action-button').text('Сохранить');
		$form.find('.remove-group-link').data('groupId', groupId).show();
		$('#createOrUpdateGroupModal').modal();
		*/
	});

	$('.modal__edit-group__members').on('click', '.group__member__remove-link', function (e) {
		e.preventDefault();
		var $self = $(this);

		var url = $self.attr('href');
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

			var $member = $self.closest('.group__member');
			$member.remove();
		});
	});

	$('.modal__edit-group__button').click(function (e) {
		e.preventDefault();
		var $modal = $('#modal__edit-group');

		var $form = $modal.find('#modal__edit-group__settings form');
		var $error = $modal.find('.modal__edit-group__error');
		var url = $form.attr('action');

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
		var $form = $(this).closest('form');
		var manualChecking = $(this).prop('checked');
		$form.find('[name="manualCheckingForOldSolutions"]').prop('checked', manualChecking)
			.closest('.checkbox').toggle(manualChecking);
	});

	$('.remove-group-link').click(function(e) {
		e.preventDefault();

		var $self = $(this);
		var groupId = $self.data('groupId');
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

	var toggleCopyGroupButton = function (canSubmit) {
		var $form = $('#copyGroupModal form');
		if (canSubmit)
			$form.find('.copy-group-button').removeAttr('disabled');
		else
			$form.find('.copy-group-button').attr('disabled', true);
	}

	$('.copy-group-link').click(function(e) {
		e.preventDefault();

		var $form = $('#copyGroupModal form');
		$form.find('select').val('-1');
		$form.find('.copy-group__change-owner-block').hide();
		toggleCopyGroupButton(false);

		$('#copyGroupModal').modal();
	});

	$('#copyGroupModal form select').change(function() {
		var $form = $('#copyGroupModal form');
		var $self = $(this);
		var $option = $self.find('option:selected');

		var $changeOwnerBlock = $form.find('.copy-group__change-owner-block');
		var needToChangeOwner = $option.data('needToChangeOwner');
		$changeOwnerBlock.find('input[name="changeOwner"]').prop('checked', false);
		$changeOwnerBlock.find('.owner-name').text($option.data('owner'));
		$changeOwnerBlock.toggle(needToChangeOwner);

		var canSubmit = $self.val() !== '-1' && !needToChangeOwner;
		toggleCopyGroupButton(canSubmit);
	});

	$('#copyGroupModal form [name="changeOwner"]').change(function() {
		toggleCopyGroupButton($(this).prop('checked'));
	});


	$('.show-archived-groups-selector > .btn').click(function(e) {
		var $self = $(this);
		if ($self.hasClass('active')) {
			e.preventDefault();
			return false;
		}

		$self.parent().find('.btn').removeClass('active');
		$self.addClass('active');
		$('.groups .group').toggle();
	});
})