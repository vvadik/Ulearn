$(document).ready(function() {
	var $modal = $('#ulearnUpdateInvitationsModal');
	var cookieName = 'ulearnUpdatesInvitation';

	var getCookie = function(name) {
		var matches = document.cookie.match(new RegExp(
		  "(?:^|; )" + name.replace(/([\.$?*|{}\(\)\[\]\\\/\+^])/g, '\\$1') + "=([^;]*)"
		));
		return matches ? decodeURIComponent(matches[1]) : undefined;
	}

	var setCookie = function (name, value) {
		var expires = new Date(2020, 1, 1);
		var updatedCookie = name + "=" + encodeURIComponent(value) + ";path=/;expires=" + expires.toUTCString();
		document.cookie = updatedCookie;
	}

	var showUlearnUpdatesModal = function() {
		if (getCookie(cookieName) === undefined) {
			setCookie(cookieName, 'true');
			$modal.modal();
		}
	}

	if ($modal.length)
		showUlearnUpdatesModal();
});