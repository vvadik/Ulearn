import { RefObject } from "react";

export default function scrollToView(element: RefObject<Element>, animationDuration = 500): void {
	const header = document.getElementById("header");
	if(!header) {
		return;
	}
	const headerHeight = header.getBoundingClientRect().height;
	const curElem = element.current;

	if(curElem && curElem.getBoundingClientRect().top > 0) {
		animate(() => offsetTop(curElem) - headerHeight, animationDuration);
	}
}

function getScrollTop() {
	return window.pageYOffset || document.documentElement.scrollTop;
}

function offsetTop(el: Element) {
	const rect = el.getBoundingClientRect();
	return rect.top + getScrollTop();
}

function animate(getToPosition: () => number, duration: number, increment = 20) {
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
function easeInOutQuad(t: number, b: number, c: number, d: number): number {
	t /= d / 2;
	if(t < 1) {
		return c / 2 * t * t + b;
	}
	t--;
	return -c / 2 * (t * (t - 2) - 1) + b;
}
