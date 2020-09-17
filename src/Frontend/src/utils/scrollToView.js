export default function scrollToView(element, animationDuration = 500) {
	const header = document.getElementById("header");
	const headerHeight = header.getBoundingClientRect().height;

	if(element.current.getBoundingClientRect().top > 0) {
		animate(() => offsetTop(element.current) - headerHeight, animationDuration);
	}
}

function getScrollTop() {
	return window.pageYOffset || document.documentElement.scrollTop;
}

function offsetTop(el) {
	const rect = el.getBoundingClientRect();
	return rect.top + getScrollTop();
}

function animate(getToPosition, duration, increment = 20) {
	const getChange = () => getToPosition() - getScrollTop();
	let currentTime = 0;

	const animateScroll = function () {
		currentTime += increment;
		const scrollPosition = easeInOutQuad(currentTime, getScrollTop(), getChange(), duration);
		window.scrollTo({
			left: 0,
			top: scrollPosition,
			behavior: "smooth",
		});
		if(currentTime < duration) {
			setTimeout(animateScroll, increment);
		}
	};
	animateScroll();
}

//t = current time
//b = start value
//c = change in value
//d = duration
function easeInOutQuad(t, b, c, d) {
	t /= d / 2;
	if(t < 1) return c / 2 * t * t + b;
	t--;
	return -c / 2 * (t * (t - 2) - 1) + b;
}
