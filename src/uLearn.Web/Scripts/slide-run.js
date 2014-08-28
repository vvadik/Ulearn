function setRunVerdict($verdict, ans) {
	if (ans.IsRightAnswer) {
		$verdict.removeClass("label-danger");
		$verdict.addClass("label-success");
		$verdict.text("Успех!");
	} else {
		$verdict.addClass("label-danger");
		$verdict.removeClass("label-success");
		if (ans.CompilationError) {
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
var $difTable = $(".diff-table");
var $questionLog = $(".questions-log");

function updateVerdict(isRight, verdict, details, isCompileError, showExpectedOutput) {
	$runVerdict.show();
	$runVerdict.text(verdict);
	$runVerdict.toggleClass("label-danger", !isRight);
	$runVerdict.toggleClass("label-success", isRight);
	$expectedOutput.toggle(showExpectedOutput);
	$actualOutput.toggleClass("full-size", true);
	if (isCompileError) {
		$actualOutput.find(".output-label").hide();
	}
	$difTable.toggle(showExpectedOutput);
	$actualOutput.toggle(!showExpectedOutput);
	if (!showExpectedOutput) {
		$actualOutputContent.text(details);
	}
	if (isRight)
		slideNavigation.makeShowSolutionsNext();
	$afterRunBlock.show();
}

var $runButton = $(".run-solution-button");

$runButton.click(function () {
	var code = $(".code-exercise")[0].codeMirrorEditor.getValue();
	$runButton.text("...running...").addClass("active");
	$afterRunBlock.hide();
	$runVerdict.hide();

	$.ajax(
		{
			type: "POST",
			url: $runButton.data("url"),
			data: code
		}).success(function (ans) {
			var isCompileError = !!ans.CompilationError;
			var verdict = isCompileError
				? "Compilation error"
				: (ans.IsRightAnswer ? "Успех!" : "Неверный ответ");
			var showExpectedOutput = !isCompileError && !!ans.ExpectedOutput && !ans.IsRightAnswer;
			var details = ans.CompilationError ? ans.CompilationError : ans.ActualOutput;
			$difTable.html("<div></div>");
			if (showExpectedOutput) {
				var solutionsDiff = diffUsingJS(details, ans.ExpectedOutput);
				$difTable.html(solutionsDiff);
			}
			updateVerdict(ans.IsRightAnswer, verdict, details, isCompileError, showExpectedOutput);
		})
		.fail(function (req) {
			updateVerdict(false, "Ошибка сервера :(", req.status + " " + req.statusText, false);
			console.log(req.responseText);
		})
		.always(function () {
			$runButton.text("RUN").removeClass("active");
		});
});

var $prepareButton = $(".prepare-solution-button");
$prepareButton.click(function () {
	var code = $(".code-exercise")[0].codeMirrorEditor.getValue();
	$.ajax(
		{
			type: "POST",
			url: $prepareButton.data("url"),
			data: code
		})
		.fail(function (req) {
			console.log(req.responseText);
		})
		.success(function (ans) {
			console.log(ans);
		});
});


