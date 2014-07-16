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


function setRunVerdict($verdict, ans) {
	if (ans.IsRightAnswer) {
		$verdict.removeClass("label-danger");
		$verdict.addClass("label-success");
		$verdict.text("Успех!");
	} else {
		$verdict.addClass("label-danger");
		$verdict.removeClass("label-success");
		if (ans.ExecutionResult.CompilationError) {
			$verdict.text("Ошибка компиляции");
		} else {
			$verdict.text("Ошибка в программе");
		}
	}
};

var $actualOutput = $(".actual-output");
var $expectedOutput = $(".expected-output");
var $actualOutputContent = $(".actual-output-content");
var $afterRunBlock = $(".after-run-block");
var $runVerdict = $(".run-verdict");

function updateVerdict(isRight, verdict, details, isCompileError) {
	$runVerdict.show();
	$runVerdict.text(verdict);
	$runVerdict.toggleClass("label-danger", !isRight);
	$runVerdict.toggleClass("label-success", isRight);

	var showExpectedOutput = !isCompileError && !isRight;
	$expectedOutput.toggle(showExpectedOutput);
	$actualOutput.toggleClass("half-size", showExpectedOutput);
	$actualOutput.toggleClass("full-size", !showExpectedOutput);
	$actualOutput.find(".output-label").toggle(!isCompileError);
	$actualOutputContent.text(details);

	$afterRunBlock.show();
    if (isRight) {
        $showAnotherAnswer.show();
        $nextLink.hide();
        $endHere.hide();
    }
}

var $runButton = $("#run");
var $nextLink = $("#NextLink");
var $prevLink = $("#PrevLink");
var $endHere = $("#EndHere");
var $selectorHere = $("#SelectorHere");
var $showAnotherAnswer = $("#ShowAnotherAnswer");

$runButton.click(function () {
	var code = $(".code-exercise")[0].codeMirrorEditor.getValue();
	$runButton.text("...running...").addClass("active");
	$afterRunBlock.hide();
	$runVerdict.hide();

	$.ajax(
	{
		type: "POST",
		url: runSolutionUrl,
		data: code
	}).success(function (ans) {
		var isCompileError = ans.ExecutionResult.CompilationError;
		var verdict = isCompileError
			? "Compilation error" 
			: (ans.IsRightAnswer ? "Успех!" : "Неверный ответ");
		var details = ans.ExecutionResult.CompilationError ? ans.ExecutionResult.CompilationError : ans.ExecutionResult.Output;
		updateVerdict(ans.IsRightAnswer, verdict, details, isCompileError);
		console.log(ans);
	})
	.fail(function (req) {
		updateVerdict(false, "Ошибка сервера :(", req.status + " " + req.statusText);
		console.log(req.responseText);
		})
	.always(function (ans) {
		$runButton.text("RUN").removeClass("active");
	});
})


function LikeSolution(solutionId) {
    $.ajax(
	{
	    type: "POST",
	    url: likeSolutionUrl,
        data: String(solutionId)
	}).success(function (ans) {
	})
	.fail(function (req) {

	})
	.always(function (ans) {
	});
}