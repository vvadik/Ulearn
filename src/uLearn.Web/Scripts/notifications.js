$(document).ready(function () {
	var $notificaticationsIconLink = $('.notifications__icon-link');
	var $counter = $('.notifications__counter');
	var $dropdownContent = $('.notifications__dropdown');
	var lastCountUpdateTimestamp = $notificaticationsIconLink.data('lastTimestamp');

	/* On mobile layout close user menu on notification area showing */
	var hideUserMenu = function() {
		$('.greeting-collapse-class').removeClass('in');
	};

	/* On mobile layout close notification area on user menu showing */
	$('.greeting-collapse-class').on('show.bs.collapse', function() {
		$('.notifications__mobile-dropdown').removeClass('in');
	});

	$notificaticationsIconLink.click(function () {
		var $self = $(this);
		var $dropdown = $self.closest('.dropdown');
		var isDesktopVersionOpened = $dropdown.hasClass('open');
		var isMobileVersionOpened = $('.notifications__mobile-dropdown').hasClass('in');
		if (!isDesktopVersionOpened && !isMobileVersionOpened) {
			var loadUrl = $self.data('notificationsUrl');
			$dropdownContent.html('<li class="notifications__info">Загружаю последние уведомления...</li>');
			$dropdownContent.load(loadUrl, function() {
				$counter.text('0').hide();
			});
		}

		hideUserMenu();
	});
	
	$(document).on('click', '.notifications__notification', function(e) {
		e.preventDefault();
		
		var link = $(this).find('> *').data('href');
		if (link) {
			window.location.href = link;
		}
	});

	$(document).on('click', '.notifications__feed-switcher a', function (e) {
		e.preventDefault();
		e.stopPropagation();

		var $self = $(this);
		var feed = $self.data('feed');
		if ($self.hasClass('active'))
			return;

		var $feeds = $('.notifications__feed');
		var $switchers = $('.notifications__feed-switcher a');
		$switchers.removeClass('active');
		$feeds.removeClass('active');

		$self.addClass('active');
		$feeds.filter('[data-feed="' + feed + '"]').addClass('active');
	});

	var updateNotificationUnreadCount = function () {
		var url = $notificaticationsIconLink.data('countUrl').replace('LAST_TIMESTAMP', lastCountUpdateTimestamp);
		$.getJSON(url, function (data) {
			if (data.status === 'ok') {
				var unreadCount = data.count;
				if (unreadCount > 0) {
					var currentCount = parseInt($counter.text());
					var newCount = Math.min(currentCount + unreadCount, 99);
					$counter.text(newCount).show();
					lastCountUpdateTimestamp = data.last_timestamp;
				}
			} else {
				alert(data.error);
			}
		});
	}

	if ($('.notifications__icon-link').length > 0)
		setInterval(updateNotificationUnreadCount, 60 * 1000);
});