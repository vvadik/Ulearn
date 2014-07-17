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
var $difTable = $(".diff-table");


function updateVerdict(isRight, verdict, details, isCompileError) {
    $runVerdict.show();
    $runVerdict.text(verdict);
    $runVerdict.toggleClass("label-danger", !isRight);
    $runVerdict.toggleClass("label-success", isRight);
    var showExpectedOutput = !isCompileError && !isRight;
    $expectedOutput.toggle(showExpectedOutput);
    $difTable.toggle(false);
    $actualOutput.toggle(true);
    $actualOutput.toggleClass("full-size", true);
    if (isCompileError) {
        $actualOutput.find(".output-label").toggle(!isCompileError);
        $actualOutputContent.text(details); //тут я возьму текущий вывод
        $difTable.toggle(false);
    }
    if (!isRight && !isCompileError) {
        $actualOutput.toggle(false);
        $difTable.toggle(true);
    }
    $afterRunBlock.show();
    if (isRight) {
        slideNavigation.makeShowSolutionsNext();
        $actualOutput.find(".output-label").toggle(!isCompileError);
        $actualOutputContent.text(details);
    }
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
		    var isCompileError = ans.ExecutionResult.CompilationError;
		    var verdict = isCompileError
				? "Compilation error"
				: (ans.IsRightAnswer ? "Успех!" : "Неверный ответ");
		    var details = ans.ExecutionResult.CompilationError ? ans.ExecutionResult.CompilationError : ans.ExecutionResult.Output;
		    $difTable.html("<div></div>");
		    if (verdict != "Compilation error") {
		        var solutionsDiff = diffUsingJS(details, ans.ExpectedOutput);
		        $difTable.html(solutionsDiff);
		    }
		    updateVerdict(ans.IsRightAnswer, verdict, details, isCompileError);
		})
		.fail(function (req) {
		    updateVerdict(false, "Ошибка сервера :(", req.status + " " + req.statusText);
		    console.log(req.responseText);
		})
		.always(function (ans) {
		    $runButton.text("RUN").removeClass("active");
		});
});

var likeSolutionUrl = $("#LikeSolutionUrl").data("url");

function likeSolution(solutionId) {
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

