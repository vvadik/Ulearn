window.documentReadyFunctions = window.documentReadyFunctions || [];

const katexTransformed = 'katex';

window.documentReadyFunctions.push(function () {
    $(".tex").each(function (index, element) {
        const latex = $(element).text();
        if (latex && element.dataset.transformation !== katexTransformed) {
            element.title = latex;
			element.dataset.transformation = katexTransformed;
            katex.render(latex, element);
        }
    });
});