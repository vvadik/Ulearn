CodeMirror.commands.autocomplete = function (cm) {
	cm.showHint({ hint: CodeMirror.hint.csharp });
}

function getMode(lang) {
	// see http://codemirror.net/mode/

	var langIds = {
		"cs": "x-csharp",
		"py": "x-python"
	};

	lang = lang || "cs";
	return "text/" + langIds[lang];
}

var editorLastRange = null;

function codeMirrorClass(c, editable, guest, review) {
	var codes = document.getElementsByClassName(c);
	for (var i = 0; i < codes.length; i++) {
		var element = codes[i];
		var $el = $(element);
		var langId = $el.data("lang");
		var editor = CodeMirror.fromTextArea(element,
		{
			mode: getMode(langId),
			lineNumbers: true,
			theme: (editable || guest) ? "cobalt" : "default",
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
			//autoCloseBrackets: true, // workaround: autoCloseBracket breakes indentation after for|while|...
			styleActiveLine: editable,
			matchBrackets: true,
		});

		if (review)
			editor.on("beforeSelectionChange",
				function (cm, params) {
					if (params.ranges < 1)
						return;
					var range = params.ranges[0];
					editorLastRange = range;
					var maxLine = Math.max(range.anchor.line, range.head.line);
					var coords = cm.cursorCoords({ line: maxLine + 1, ch: 1 }, 'page');

					var $addReviewPopup = $('.exercise__add-review').first();
					$addReviewPopup.offset({ top: coords.top, left: coords.left });
					$addReviewPopup.show();

					$addReviewPopup.find('.exercise__add-review__comment').trigger('input');
				});

		element.codeMirrorEditor = editor;
		if (editable)
			editor.focus();
		if (guest)
			editor.on("mousedown", function(cm) { loginForContinue(); });
	}
}

codeMirrorClass("code-exercise", true, false, false);
codeMirrorClass("code-sample", false, false, false);
codeMirrorClass("code-guest", false, true, false);
codeMirrorClass("code-review", false, false, true);

$('.exercise__add-review')
	.each(function() {
		var $self = $(this);
		var addReviewUrl = $self.data('url');

		$self.find('.exercise__add-review__comment')
			.on('input',
				function () {
					if ($(this).val() === '')
						$self.find('.exercise__add-review__button').attr('disabled', 'disabled');
					else
						$self.find('.exercise__add-review__button').removeAttr('disabled');
				});

		$self.find('.exercise__add-review__button')
			.click(function() {
				var comment = $self.find('.exercise__add-review__comment').val();
				var params = {
					StartLine: editorLastRange.anchor.line,
					StartPosition: editorLastRange.anchor.ch,
					FinishLine: editorLastRange.head.line,
					FinishPosition: editorLastRange.head.ch,
					Comment: comment,
				}
				$.post(addReviewUrl,
					params,
					function(data) {
						if (data.status !== 'ok') {
							console.log(data);
							alert('Can\'t save comment: error');
						} else {
							$self.find('.exercise__add-review__comment').val('');
							$self.hide();
						}
					},
					'json');
			});
	});

function refreshPreviousDraft(ac, id) {
    window.onbeforeunload = function () {
        if (ac == 'False')
            localStorage[id] = $('.code-exercise')[0].codeMirrorEditor.getValue();
    }
    if (localStorage[id] != undefined && ac == 'False') {
        $('.code-exercise')[0].codeMirrorEditor.setValue(localStorage[id]);
    }
}