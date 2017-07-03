$(document).ready(function () {
	var $notificaticationsIconLink = $('.notifications__icon-link');
	var $counter = $('.notifications__counter');
	var lastCountUpdateTimestamp = $notificaticationsIconLink.data('lastTimestamp');

	$notificaticationsIconLink.click(function() {
		var $self = $(this);
		var $dropdown = $self.closest('.dropdown');
		if (!$dropdown.hasClass('open')) {
			var loadUrl = $self.data('notificationsUrl');
			var $dropdownMenu = $dropdown.find('.notifications__dropdown');
			$dropdownMenu.html('<li class="notifications__info">Загружаю последние уведомления...</li>');
			$dropdownMenu.load(loadUrl, function() {
				$counter.text('0').hide();
			});
		}
	});
	
	$('.notifications__dropdown').on('click', '.notifications__notification', function(e) {
		e.preventDefault();
		var link = $(this).find('> *').data('href');
		if (link) {
			window.location.href = link;
		}
	});

	var updateNotificationUnreadCount = function () {
		var url = $notificaticationsIconLink.data('countUrl').replace('LAST_TIMESTAMP', lastCountUpdateTimestamp);
		$.getJSON(url, function (data) {
			if (data.status === 'ok') {
				var unreadCount = data.count;
				if (unreadCount > 0) {
					var currentCount = parseInt($counter.text());
					$counter.text(currentCount + unreadCount).show();
					lastCountUpdateTimestamp = data.last_timestamp;
				}
			} else {
				alert(data.error);
			}
		});
	}

	if ($('.notifications__icon-link').length > 0)
		setInterval(updateNotificationUnreadCount, 10 * 1000);
});