const mobileRegex = /Android|webOS|iPhone|iPad|iPod|BlackBerry/i;
const rateCookieName = 'youtube-video-rate';

function onYouTubeIframeAPIReady() {
	const videoBlocks = document.getElementsByClassName('youtube-video');
	const isMobile = mobileRegex.test(navigator.userAgent);
	
	for(let i =0;i<videoBlocks.length;i++){
		const block = videoBlocks[i];
		const width = block.getAttribute('data-width') || '390';
		const height = block.getAttribute('data-height') || '640';
		const videoId = block.getAttribute('data-video-id');
		const isFirstVideoOnSlide = i === 0;
		const dataAutoPlay = block.getAttribute('data-autoplay') === 'true';
		const autoplay = dataAutoPlay && !isMobile && isFirstVideoOnSlide;
		const elementId = 'youtube-video__' + videoId;
		block.id = elementId;
		const frameLoaded = false;
		let playerReady = false;
		let initialized = false;
		const intervalId = setInterval( () => {
			const element = document.getElementById(elementId);
			if(element && element.contentWindow != null) {
				if(initialized) {
					clearInterval(intervalId);
					return;
				}
				if(!playerReady) {
					return;
				}
				init(player);
				if(autoplay){
					player.playVideo();
				}
				clearInterval(intervalId);
			}
		}, 10);
		const player = new YT.Player(elementId, {
			height,
			width,
			videoId,
			events: {
				'onReady': function (e) {
					playerReady = true;
					if(!frameLoaded) {
						return;
					}
					init(e.target);
				},
				'onPlaybackRateChange': function (e) {
					const newRate = e.data;
					Cookies.set(rateCookieName, newRate);
				}
			},
			playerVars: {
				/* Disable related videos */
				rel: 0,
			},
		});
		
		function init(p) {
			const rate = parseFloat(Cookies.get(rateCookieName) || '1');
			p.setPlaybackRate(rate);
			initialized = true;
		}
	}
}

window.documentReadyFunctions = window.documentReadyFunctions || [];

window.documentReadyFunctions.push(function () {
	const enableYoutubeIframeApi = () => {
		window['YT'] = undefined;
		
		const tag = document.createElement('script');
		tag.src = "https://www.youtube.com/iframe_api";
		const firstScriptTag = document.getElementsByTagName('script')[0];
		firstScriptTag.parentNode.insertBefore(tag, firstScriptTag);
	};

	const videoBlocks = document.getElementsByClassName('youtube-video');

	if (videoBlocks.length > 0)
		enableYoutubeIframeApi();
});