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
var $questionLog= $(".questions-log");

function updateVerdict(isRight, verdict, details, isCompileError) {
	$runVerdict.show();
	$runVerdict.text(verdict);
	$runVerdict.toggleClass("label-danger", !isRight);
	$runVerdict.toggleClass("label-success", isRight);
	var showExpectedOutput = !isCompileError && !isRight;
	$expectedOutput.toggle(showExpectedOutput);
	$actualOutput.toggleClass("full-size", true);
	if (isCompileError) {
		$actualOutput.find(".output-label").hide();
		$actualOutputContent.text(details);
	}
	$difTable.toggle(false);
	$actualOutput.toggle(true);
	if (isCompileError) {
		$difTable.toggle(false);
		$actualOutput.toggle(true);
	} else if (!isRight) {
	    $difTable.toggle(true);
	    $actualOutput.toggle(false);
    }
    $afterRunBlock.show();
	if (isRight) {
	    $actualOutputContent.text(details);
		slideNavigation.makeShowSolutionsNext();
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
			var isCompileError = !!ans.CompilationError;
			var verdict = isCompileError
				? "Compilation error"
				: (ans.IsRightAnswer ? "Успех!" : "Неверный ответ");
			var details = ans.CompilationError ? ans.CompilationError : ans.ActualOutput;
			$difTable.html("<div></div>");
			if (!isCompileError) {
				var solutionsDiff = diffUsingJS(details, ans.ExpectedOutput);
				$difTable.html(solutionsDiff);
			}
			updateVerdict(ans.IsRightAnswer, verdict, details, isCompileError);
		})
		.fail(function (req) {
			updateVerdict(false, "Ошибка сервера :(", req.status + " " + req.statusText);
			console.log(req.responseText);
		})
		.always(function () {
			$runButton.text("RUN").removeClass("active");
		});
});

var likeSolutionUrl = $("#LikeSolutionUrl").data("url");
var questUrl = $("#Ask").data("url");

function likeSolution(solutionId) {
	$.ajax({
		type: "POST",
		url: likeSolutionUrl,
		data: String(solutionId)
	}).success(function (ans) {
		var likerCounterId = "#counter" + solutionId;
		var likeCounter = $(likerCounterId);;
		if (ans == "success") {
			likeCounter.text(String(parseInt(likeCounter.val()) + 1));
		} else {
			likeCounter.text("already like from you");
		}
	});
}

function sendQuestion() {
    var quest = $("#questField").val();
    $.ajax(
		{
		    type: "POST",
		    url: questUrl,
		    data: quest
		}).success(function (ans) {
		    $("#sendButton").text("Отправлено");
        })
		.fail(function (req) {
		    console.log(req.responseText);
		})
		.always(function (ans) {
		});
};

var getAllQuestionsUrl = $("#WatchQuestions").data("url");
function printAllQuestions() {
    $.ajax(
		{
		    type: "POST",
		    url: getAllQuestionsUrl,
		    data: " "
		}).success(function (ans) {
            $questionLog.html(makeTableForQuestions(ans));
        })
		.fail(function (req) {
		    console.log(req.responseText);
		})
		.always(function (ans) {
		});
};

function makeTableForQuestions(input) {
    var content = input.split("\n***");
    var table = "<table class=\"answer-table\" border=1px>";
    for (var i = 0; i < content.length-1; i += 5) {
        table += "<tr>";
        for (var j = 0; j < 4; j++)
            table += ("<td class=\"answer-table-headers\">" + content[i + j] + "</td>");
        table += "</tr>";
        table += "<tr><td colspan=\"4\">" + content[i + 4] + "</tr></td>";
    }
    table += "</table";
    return table;
}
