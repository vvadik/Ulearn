$(".tex").each(function(index, element) {
	var latex = element.innerText;
	element.title = latex;
	katex.render(latex, element);
})