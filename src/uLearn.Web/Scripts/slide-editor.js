CodeMirror.commands.autocomplete = function (cm) {
	cm.showHint({ hint: CodeMirror.hint.csharp });
}

function getMode(langId) {
	// see http://codemirror.net/mode/
	langId = langId || "csharp";
	return "text/x-" + langId;
}

function codeMirrorClass(c, editable) {
	var codes = document.getElementsByClassName(c);
	for (var i = 0; i < codes.length; i++) {
		var element = codes[i];
		var $el = $(element);
		var langId = $el.data("lang");
		var editor = CodeMirror.fromTextArea(element,
		{
			mode: getMode(langId),
			lineNumbers: true,
			theme: editable ? "cobalt" : "default",
			indentWithTabs: true,
			tabSize: 4,
			indentUnit: 4,
			extraKeys: {
				"Ctrl-Space": "autocomplete",
				".": function(cm) {
					setTimeout(function() { cm.execCommand("autocomplete"); }, 100);
					return CodeMirror.Pass;
				}
			},
			readOnly: !editable,
			//autoCloseBrackets: true, // bug: autoCloseBracket breakes indentation after for|while|...
			styleActiveLine: editable,
			matchBrackets: true,
	});
		element.codeMirrorEditor = editor;
		if (editable)
			editor.focus();

	}
}

codeMirrorClass("code-exercise", true);
codeMirrorClass("code-sample", false);

function refreshPreviousDraft(ac, id) {
    window.onbeforeunload = function () {
        if (ac == 'False')
            localStorage[id] = $('.code-exercise')[0].codeMirrorEditor.getValue();
    }
    if (localStorage[id] != undefined && ac == 'False') {
        $('.code-exercise')[0].codeMirrorEditor.setValue(localStorage[id]);
    }
}