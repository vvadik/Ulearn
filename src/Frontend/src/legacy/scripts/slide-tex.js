import * as katex from "katex";

const katexTransformed = 'katex';

export default function () {
	$(".tex").each(function (index, element) {
		const latex = $(element).text();
		if(latex && element.dataset.transformation !== katexTransformed) {
			element.title = latex;
			element.dataset.transformation = katexTransformed;
			katex.render(latex, element);
		}
	});
}
