$(document).ready(function () {
    var $modal = $('#notifications__transport-settings__modal');
    var $form = $modal.find('form');
    var $panel = $modal.find('.notifications__transport-settings__panel');
    var $status = $modal.find('.status');

    function getNotificationSettings(courseId, transportId, notificationType) {
        for (var idx in window.notificationTransportsSettings) {
            if (!window.notificationTransportsSettings.hasOwnProperty(idx))
                continue;

            var setting = window.notificationTransportsSettings[idx];
            if (setting.courseId === courseId &&
                setting.transportId === transportId &&
                setting.notificationType === notificationType)
                return setting.isEnabled;
        }
        return undefined;
    }

    $('.notifications__transport-settings__link').click(function() {
        var $self = $(this);
        var transportId = $self.data('transportId');
        var modalTitle = $self.data('modalTitle');

        $form.find('[name="transportId"]').val(transportId);
        $modal.find('.modal-title').text(modalTitle);

        $('.notifications__transport-settings__course__link').removeClass('active');
        $panel.removeClass('active');

        $modal.modal();
    });

    $('.notifications__transport-settings__course__link').click(function () {
        var $self = $(this);
        var courseId = $self.data('courseId');
        var courseTitle = $self.data('courseTitle');
        var transportId = parseInt($form.find('[name="transportId"]').val());

        $form.find('[name="courseId"]').val(courseId);
        $modal.find('.course-title').text(courseTitle);
        $('.notifications__transport-settings__course__link').removeClass('active');
        $self.addClass('active');
        $panel.addClass('active');

        var notificationTypesForCourse = window.notificationTypesByCourse[courseId];
        var $checkboxes = $panel.find('input[type="checkbox"]');
        $checkboxes.each(function() {
            var $checkbox = $(this);
            var $checkboxWrapper = $checkbox.closest('.checkbox');
            var notificationType = $checkbox.data('notificationType');
            $checkboxWrapper.toggle(notificationTypesForCourse.indexOf(notificationType) !== -1);

            var isEnabled = getNotificationSettings(courseId, transportId, notificationType);
            if (isEnabled === undefined)
                isEnabled = $checkbox.data('default');

            $checkbox.prop('checked', isEnabled);
        });
    });

    $form.submit(function (e) {
        e.preventDefault();
        var $button = $form.find('.btn');
        $button.attr('disabled', true);

        $.post($form.attr('action'), $form.serialize(), null, 'json')
            .done(function (result) {
                if (result.status === 'ok') {
                    window.notificationTransportsSettings = result.notificationTransportsSettings;

                    $status.addClass('text-success').text('Сохранено');
                    setTimeout(function() {
                        $status.text('').removeClass('text-success');
                    },
                    1000);
                }
            })
            .fail(function(result) {
                $status.addClass('text-danger').text(result);
            })
            .always(function() {
                $button.attr('disabled', false);
            });

        return false;
    });
});