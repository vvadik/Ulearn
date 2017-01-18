$(document).ready(function () {
	var $modal = $('#createOrUpdateCertificateTemplateModal');
	var $form = $('#createOrUpdateCertificateTemplateModal form');
	var token = $('input[name="__RequestVerificationToken"]').val();

	$('.create-template-link').click(function (e) {
		e.preventDefault();

		$form.attr('action', $form.data('createTemplateUrl'));
		$form.find('[name="templateId"]').val('');
		$form.find('[name="name"]').val('');
		$form.find('[name="archive"]').attr('required', 'required');
		$form.find('button.action-button').text('Загрузить');
		$form.find('.current-archive-download').hide();
		$form.find('.remove-template-link').hide();
		$modal.find('.modal-title').text('Новый шаблон');
		$modal.modal();
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
		$form.find('button.action-button').text('Сохранить');
		$form.find('.current-archive-download-link').attr('href', $self.data('templateUrl'));
		$form.find('.current-archive-download').show();
		$form.find('.remove-template-link').show();
		$modal.find('.modal-title').text('Редактировать шаблон');
		$modal.modal();
	});

	$('.remove-template-link').click(function (e) {
		e.preventDefault();

		var $self = $(this);
		var $form = $self.closest('form');
		var courseId = $form.find('[name=courseId]').val();
		var templateId = $form.find('[name=templateId]').val();
		$.ajax({
			type: 'post',
			url: $self.data('url'),
			data: {
				__RequestVerificationToken: token,
				courseId: courseId,
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
				$form.find('.preview-certificate-button').show();
				return false;
			}
		});
	});

	$('.certificate').on('click', '.remove-certificate-link', function (e) {
		e.preventDefault();

		var $self = $(this);
		$.ajax({
			type: 'post',
			url: $self.data('url'),
			data: {
				__RequestVerificationToken: token,
			}
		}).fail(function () {
			alert('При отзыве сертификата произошла какая-то ошибка');
		});
		$self.parent().remove();
	});

	$('.add-certificate-button').click(function () {
		var $form = $(this).closest('form');
		$form.find('[name="isPreview"]').val("false");
		$form.attr('target', '');
		return true;
	});

	$('.preview-certificate-button').click(function () {
		var $form = $(this).closest('form');
		$form.find('[name="isPreview"]').val("true");
		$form.attr('target', '_blank');
		return true;
	});
});