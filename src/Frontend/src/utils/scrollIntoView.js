export default function scrollIntoView(element) {
	const header = document.getElementById('header');
	let headerHeight = header.getBoundingClientRect().height;
	const divider = document.getElementById('headerContentDivider');
	const dividerHeight = divider.getBoundingClientRect().height;

	window.scrollTo({
		left: 0,
		top: element.current.offsetTop - headerHeight - dividerHeight,
		behavior: "smooth"
	});
}