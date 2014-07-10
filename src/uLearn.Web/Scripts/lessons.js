CodeMirror.commands.autocomplete = function (cm) {
	cm.showHint({ hint: CodeMirror.hint.csharp });
}

function codeMirrorClass(c, editable) {
	var codes = document.getElementsByClassName(c);
	for (var i = 0; i < codes.length; i++) {
		var element = codes[i];
		var editor = CodeMirror.fromTextArea(element,
		{
			mode: "text/x-csharp",
			lineNumbers: true,
			theme: editable ? "cobalt" : "default",
			indentWithTabs: true,
			tabSize: 4,
			indentUnit: 4,
			extraKeys: { "Ctrl-Space": "autocomplete" },
			readOnly: editable ? false : "nocursor",
		});
		element.codeMirrorEditor = editor;

	}
}

codeMirrorClass("code-exercise", true);
codeMirrorClass("code-sample", false);


$("#run").click(function () {
	var code = $(".code-exercise")[0].codeMirrorEditor.getValue();
	console.log(code);
	$.ajax(
		{
			type: "POST",
			url: runSolutionUrl,
			data: code,
			success : function(ans) {
				console.log(ans);
			}
		}
		)
})