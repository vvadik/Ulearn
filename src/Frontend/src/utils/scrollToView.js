export default function scrollToView(element) {
	const header = document.getElementById("header");
	let headerHeight = header.getBoundingClientRect().height;
	const threadScrollHeight =  element.current.offsetTop - headerHeight;
	const replyScrollHeight = element.current.getBoundingClientRect().top - element.current.clientHeight + headerHeight;

	window.scrollTo({
		left: 0,
		top: element.current.offsetParent.localName === "body" ? threadScrollHeight : replyScrollHeight,
		behavior: "smooth",
	});
}