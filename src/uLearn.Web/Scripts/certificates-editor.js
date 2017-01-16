$(document).ready(function () {
	var $form = $('#createOrUpdateCertificateTemplateModal form');
	var token = $('input[name="__RequestVerificationToken"]').val();

	$('.create-template-link').click(function (e) {
		e.preventDefault();

		$form.attr('action', $form.data('createTemplateUrl'));
		$form.find('[name="templateId"]').val('');
		$form.find('[name="name"]').val('');
		$form.find('[name="archive"]').attr('required', 'required');
		$form.find('button.action-button').text('Загрузить');
		$form.find('.remove-template-link').hide();
		$('#createOrUpdateCertificateTemplateModal').modal();
	});

	$('.edit-template-link').click(function (e) {
		e.preventDefault();

		var $self = $(this);
		var templateId = $self.data('templateId');
		var templateName = $self.data('templateName');

		$form.attr('action', $form.data('editTemplateUrl'));
		$form.find('[name="templateId"]').val(templateId);
		$form.find('[name="name"]').val(templateName);
		$form.find("[name=archive]").removeAttr('required');
		$form.find('button.action-button').text('Обновить');
		$form.find('.remove-template-link').show();
		$('#createOrUpdateCertificateTemplateModal').modal();
	});

	$('.remove-template-link').click(function (e) {
		e.preventDefault();

		var $self = $(this);
		var templateId = $self.closest('form').find('[name=templateId]').val();
		$.ajax({
			type: 'post',
			url: $self.data('url'),
			data: {
				__RequestVerificationToken: token,
				templateId: templateId,
			}
		}).success(function () {
			window.location.reload(true);
		});
	});

	$('.add-certificate-input').each(function () {
		var $self = $(this);
		var $form = $self.closest('form');
		var url = $self.data('url');
		$self.autocomplete({
			source: url,
			select: function (event, ui) {
				var item = ui.item;
				$self.val(item.value);
				$form.find('[name="userId"]').val(item.id);
				$form.find('.add-certificate-button').text('Выдать сертификат: ' + item.value);
				$form.find('.certificate-template__parameter').show();
				$form.find('.add-certificate-button').show();
				return false;
			}
		});
	});

	$('.certificate').on('click', '.remove-certificate-link', function (e) {
		e.preventDefault();

		var $self = $(this);
		var certificateId = $self.data('certificateId');
		$.ajax({
			type: 'post',
			url: $self.data('url'),
			data: {
				__RequestVerificationToken: token,
				certificateId: certificateId
			}
		}).fail(function () {
			alert('При отзыве сертификата произошла какая-то ошибка');
		});
		$self.parent().remove();
	});
});