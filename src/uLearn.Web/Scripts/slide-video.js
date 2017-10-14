function onYouTubeIframeAPIReady() {
	var $videoBlocks = $('.youtube-video');
	var rateCookieName = 'youtube-video-rate';

	var isMobile = /Android|webOS|iPhone|iPad|iPod|BlackBerry/i.test(navigator.userAgent) ? true : false;

	$videoBlocks.each(function() {
		var $videoBlock = $(this);

		var width = $videoBlock.data('width') || '390';
		var height = $videoBlock.data('height') || '640';
		var videoId = $videoBlock.data('videoId');
		var autoplay = $videoBlock.data('autoplay') && ! isMobile;
		var elementId = 'youtube-video__' + videoId;
		$videoBlock.attr('id', elementId);

		var player = new YT.Player(elementId, {
			height: width,
			width: height,
			videoId: videoId,
			events: {
				'onReady': function (e) {
					var rate = parseFloat(Cookies.get(rateCookieName) || '1');
					e.target.setPlaybackRate(rate);
					if (autoplay)
						e.target.playVideo();
				},
				'onPlaybackRateChange': function(e) {
					var newRate = e.data;
					Cookies.set(rateCookieName, newRate);
				}
			},
			playerVars: {
				/* Disable related videos */
				rel: 0,
			},
		});
	});
}

$(document).ready(function () {
	var enableYoutubeIframeApi = function() {
		var tag = document.createElement('script');
		tag.src = "https://www.youtube.com/iframe_api";
		var firstScriptTag = document.getElementsByTagName('script')[0];
		firstScriptTag.parentNode.insertBefore(tag, firstScriptTag);
	};

	var $videoBlocks = $('.youtube-video');

	if ($videoBlocks.length > 0)
		enableYoutubeIframeApi();
});