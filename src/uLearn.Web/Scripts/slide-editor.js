$(document).ready(function() {
	var editorLastRange = null;
	var currentReviewTextMarker = null;
	var reviewsTextMarkers = {};
	var exerciseCodeDoc = null;

	var $exerciseCodeBlock;
	if ($('.code-review').length > 0)
		$exerciseCodeBlock = $('.code-review')[0];
	else
		$exerciseCodeBlock = $('.code-reviewed')[0];

	CodeMirror.commands.autocomplete = function (cm) {
		cm.showHint({ hint: CodeMirror.hint.csharp });
	}

	codeMirrorClass("code-exercise", true, false, false);
	codeMirrorClass("code-sample", false, false, false);
	codeMirrorClass("code-guest", false, true, false);
	codeMirrorClass("code-review", false, false, true);
	codeMirrorClass("code-reviewed", false, false, false);

	if ($exerciseCodeBlock) {
		var exerciseCodeEditor = $exerciseCodeBlock.codeMirrorEditor;
		exerciseCodeDoc = exerciseCodeEditor.getDoc();
	}

	String.prototype.br2nl = function () {
		return this.replace(/<br\s*\/?>/gi, "\n");
	}

	String.prototype.nl2br = function () {
		return this.replace(/\n/g, "<br>");
	}

	String.prototype.encodeMultiLineText = function () {
		return $.encodeHtmlEntities(this).nl2br();
	}

	String.prototype.decodeMultiLineText = function () {
		return $.decodeHtmlEntities(this.br2nl());
	}

	jQuery.encodeHtmlEntities = function (text) {
		return $('<span>').text(text).html();
	}

	jQuery.decodeHtmlEntities = function (html) {
		return $('<span>').html(html).text();
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

	function unselectAllReviews() {
		$('.exercise__review.selected').removeClass('selected');
		if (currentReviewTextMarker) {
			currentReviewTextMarker.clear();
			currentReviewTextMarker = null;
		}
	}

	function selectReview($review) {
		var startLine = $review.data('start-line');
		var startPosition = $review.data('start-position');
		var finishLine = $review.data('finish-line');
		var finishPosition = $review.data('finish-position');

		unselectAllReviews();

		$review.addClass('selected');
		currentReviewTextMarker = exerciseCodeDoc.markText({ line: startLine, ch: startPosition }, { line: finishLine, ch: finishPosition }, {
			className: 'exercise__code__reviewed-fragment__selected'
		});
	}

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
				editor.on("beforeSelectionChange", function (cm, params) {
					unselectAllReviews();

					if (params.ranges < 1)
						return;

					var range = params.ranges[0];
					editorLastRange = range;
					var maxLine = Math.max(range.anchor.line, range.head.line);
					var coords = cm.cursorCoords({ line: maxLine + 1, ch: 1 }, 'page');

					var $addReviewPopup = $('.exercise__add-review').first();
					if (range.anchor == range.head) {
						$addReviewPopup.hide();
						return;
					}
					$addReviewPopup.show();
					$addReviewPopup.offset({ top: coords.top, left: coords.left });

					$addReviewPopup.find('.exercise__add-review__comment').trigger('input');

					// Focusing on something from an event handler that, itself, grants focus, is problematic
					setTimeout(function() {
						$addReviewPopup.find('.exercise__add-review__comment').focus();
					}, 100);
				});

			editor.on('cursorActivity', function(cm) {
				var cursor = cm.getDoc().getCursor();
				var $foundReview = false;
				$('.exercise__reviews .exercise__review').each(function () {
					if ($foundReview !== false)
						return;
					var $review = $(this);
					var startLine = $review.data('start-line');
					var startPosition = $review.data('start-position');
					var finishLine = $review.data('finish-line');
					var finishPosition = $review.data('finish-position');
					if (startLine > cursor.line || (startLine == cursor.line && startPosition > cursor.ch))
						return;
					if (finishLine < cursor.line || (finishLine == cursor.line && finishPosition < cursor.ch))
						return;
					$foundReview = $review;
				});
				if ($foundReview !== false)
					selectReview($foundReview);
			});

			element.codeMirrorEditor = editor;
			if (editable)
				editor.focus();
			if (guest)
				editor.on("mousedown", function(cm) { loginForContinue(); });
		}
	}

	$('.exercise__add-review').each(function() {
		var $self = $(this);
		var addReviewUrl = $self.data('url');

		$self.find('.exercise__add-review__comment').on('input', function () {
			if ($(this).val() === '')
				$self.find('.exercise__add-review__button').attr('disabled', 'disabled');
			else
				$self.find('.exercise__add-review__button').removeAttr('disabled');
		});

		$self.find('.exercise__add-review__button').click(function() {
			var comment = $self.find('.exercise__add-review__comment').val();
			var params = {
				StartLine: editorLastRange.anchor.line,
				StartPosition: editorLastRange.anchor.ch,
				FinishLine: editorLastRange.head.line,
				FinishPosition: editorLastRange.head.ch,
				Comment: comment,
			}
			$.post(addReviewUrl, params, function(data) {
				if (data.status !== 'ok') {
					console.log(data);
					alert('Can\'t save comment: error');
				} else {
					addExerciseCodeReview(data.review);
					$self.find('.exercise__add-review__comment').val('');
					$self.hide();
				}
			},
			'json');
		});
	});

	$('.exercise__reviews').on('click', '.exercise__review', function () {
		if (!$exerciseCodeBlock)
			return;

		var $review = $(this).closest('.exercise__review');
		selectReview($review);
	});

	$('.exercise__reviews').on('click', '.exercise__delete-review', function () {
		var $self = $(this);
		var $review = $self.closest('.exercise__review');
		var token = $('input[name="__RequestVerificationToken"]').val();
		var reviewId = $self.data('id');
		var url = $self.data('url');
		$.post(url, {
			__RequestVerificationToken: token
		}, function(data) {
			if (data.status !== 'ok') {
				console.log(data);
				alert('Can\'t delete comment: error');
			} else {
				$review.slideUp('fast', function() { $(this).remove(); });
				reviewsTextMarkers[reviewId].clear();
				unselectAllReviews();
			}
		}, 'json');
	});
	
	var createMarkTextForReview = function($review) {
		var reviewId = $review.data('id');
		var startLine = $review.data('start-line');
		var startPosition = $review.data('start-position');
		var finishLine = $review.data('finish-line');
		var finishPosition = $review.data('finish-position');

		reviewsTextMarkers[reviewId] = exerciseCodeDoc.markText({ line: startLine, ch: startPosition }, { line: finishLine, ch: finishPosition }, {
			className: 'exercise__code__reviewed-fragment'
		});
	}

	$('.exercise__reviews .exercise__review').each(function () {
		var $review = $(this);
		createMarkTextForReview($review);
	});

	function getReviewLines(review) {
		if (review.StartLine !== review.FinishLine)
			return "строки " + (review.StartLine + 1) + "–" + (review.FinishLine + 1);
		return "строка " + (review.StartLine + 1);
	}

	function addExerciseCodeReview(review) {
		var $reviews = $('.exercise__reviews');
		var $newReview =
			$('<div class="exercise__review"><div class="exercise__review__header"><span class="author"></span>, <span class="lines"></span></div><div class="exercise__review__comment"></div></div>');
		$newReview.find('.exercise__review__header .author').text(review.AuthorName);
		$newReview.find('.exercise__review__header .lines').text(getReviewLines(review));
		$newReview.find('.exercise__review__comment').html(review.Comment.encodeMultiLineText());
		$newReview.data('id', review.Id)
			.data('start-line', review.StartLine)
			.data('start-position', review.StartPosition)
			.data('finish-line', review.FinishLine)
			.data('finish-position', review.FinishPosition);

		createMarkTextForReview($newReview);
		$reviews.append($newReview);
	}
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