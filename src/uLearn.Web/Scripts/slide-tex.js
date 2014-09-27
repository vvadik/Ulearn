$(".tex").each(function(index, element) {
	var latex = element.innerHTML;
	element.title = latex;
	katex.render(latex, element);
})