$(document).ready(function () {
	var addUserToGroupUrl = "/Admin/AddUserToGroup";
	var removeUserFromGroupUrl = "/Admin/RemoveUserFromGroup";
	var $form = $('#createOrUpdateGroupModal form');
	var token = $('input[name="__RequestVerificationToken"]').val();

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
				}).success(function (data) {
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

		$form.attr('action', $form.data('createGroupUrl'));
		$form.find('[name="groupId"]').val("");
		$form.find('[name="name"]').val("");
		$form.find('[name="isPublic"]').prop('checked', false);
		$form.find('[name="manualChecking"]').prop('checked', $form.find('[name="manualChecking"]').data('defaultValue'));
		$form.find('[name="isArchived"]').prop('checked', false).closest('.checkbox').hide();
		$form.find('[name="ownerId"]').val($form.find('[name="ownerId"]').data('defaultValue'));
		$form.find('.scoring-group-checkbox input').prop('checked', false);
		$form.find('button.action-button').text('Создать');
		$form.find('.remove-group-link').hide();
		$('#createOrUpdateGroupModal').modal(); 
	});

	$('.edit-group-link').click(function(e) {
		e.preventDefault();

		var $self = $(this);
		var groupId = $self.data('groupId');
		var name = $self.data('name');
		var isPublic = $self.data('isPublic');
		var manualChecking = $self.data('manualChecking');
		var isArchived = $self.data('isArchived');
		var ownerId = $self.data('ownerId');
		var scoringGroupsIds = $self.data('scoringGroups').split(',');

		$form.attr('action', $form.data('updateGroupUrl'));
		$form.find('[name="groupId"]').val(groupId);
		$form.find('[name="name"]').val(name);
		$form.find('[name="isPublic"]').prop('checked', isPublic);
		$form.find('[name="manualChecking"]').prop('checked', manualChecking);
		$form.find('[name="isArchived"]').prop('checked', isArchived).closest('.checkbox').show();
		$form.find('[name="ownerId"]').val(ownerId);
		$form.find('.scoring-group-checkbox input').prop('checked', false);
		scoringGroupsIds.forEach(function (scoringGroupId) {
			$form.find('.scoring-group-checkbox [name="scoring-group__' + scoringGroupId + '"]').prop('checked', true);
		});
		$form.find('button.action-button').text('Сохранить');
		$form.find('.remove-group-link').data('groupId', groupId).show();
		$('#createOrUpdateGroupModal').modal();
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
		}).success(function() {
			window.location.reload(true);
		});
	});

	$('.copy-group-link').click(function(e) {
		e.preventDefault();

		var $form = $('#copyGroupModal form');
		$form.find('button').attr('disabled', true);

		$('#copyGroupModal').modal();
	});

	$('#copyGroupModal form select').change(function() {
		var $form = $('#copyGroupModal form');
		var canSubmit = $(this).val() !== "-1";
		if (canSubmit)
			$form.find('button').removeAttr('disabled');
		else
			$form.find('button').attr('disabled', true);
	});
})