import { setExerciseVersion } from "src/legacy/Scripts/slide-versions";

let $runResults, $serviceError, $compileError, $styleError, $waError, $waErrorNoDiff, $success;

function initErrorsBlocks() {
	$runResults = $(".run-result");
	$serviceError = $runResults.filter(".run-service-error");
	$compileError = $runResults.filter(".run-compile-error");
	$styleError = $runResults.filter(".run-style-error");
	$waError = $runResults.filter(".run-wa");
	$waErrorNoDiff = $runResults.filter(".run-wa-no-diff");
	$success = $runResults.filter(".run-success");
}

function setSimpleResult($block, details) {
	$block.find(".run-details").text(details);
	$block.show();
}

function setWA(expected, actual) {
	const $difTable = $waError.find(".diff-table");
	const solutionsDiff = diffHtml(actual, expected);
	$difTable.html(solutionsDiff);
	$waError.show();
}

function RunSolutionResult() {
	this.Ignored = false;
	this.IsCompillerFailure = false;
	this.IsCompileError = false;
	this.IsStyleViolation = false;
	this.IsRightAnswer = false;
	this.ExpectedOutput = "";
	this.ActualOutput = "";
	this.ErrorMessage = "";
	this.StyleMessage = "";
	this.SentToReview = false;
}

function setResults(ans) {
	///<param name="ans" type="RunSolutionResult"></param>
	if (ans.Ignored) $('.run-solution-button').notify(ans.ErrorMessage);
	else if (ans.IsCompillerFailure) setSimpleResult($serviceError, ans.ErrorMessage);
	else if (ans.IsCompileError) setSimpleResult($compileError, ans.ErrorMessage);
	else if (ans.IsRightAnswer) {
		if(window.reloadUserProgress){
			window.reloadUserProgress();
		}
		if (ans.SubmissionId > 0)
			setExerciseVersion(ans.SubmissionId, true);
		else /* for course monitor tool */
		{
			if (ans.IsStyleViolation)
				setSimpleResult($styleError, ans.StyleMessage);
			else
				setSimpleResult($success, ans.ActualOutput);
		}
	} else if (ans.ExpectedOutput === null || ans.ExpectedOutput === "")
		setSimpleResult($waErrorNoDiff, ans.ActualOutput);
	else
		setWA(ans.ExpectedOutput, ans.ActualOutput);
}

window.documentReadyFunctions = window.documentReadyFunctions || [];

window.documentReadyFunctions.push(function () {
    $('.exercise__submission').on('click', '.run-solution-button', function () {
        const $runButton = $(this);
        initErrorsBlocks();
        let code = $(".code-exercise")[0].codeMirrorEditor.getValue();
        if (code.length === 0)
            code = " ";
        const href = window.location.href;
		$runButton.text("Выполняется...").addClass("active");
        $runResults.hide();

        $.ajax(
            {
                type: "POST",
                contentType: "text/plain",
                url: $runButton.data("url"),
                data: code
            }).done(function (res) {
				if(href === window.location.href) {
					setResults(res);
				}
				return res;
            })
            .fail(function (req) {
				if(href === window.location.href) {
					setSimpleResult($serviceError, req.status + " " + req.statusText);
					console.log(req.responseText);
				}
            })
            .always(function () {
                $runButton.text("Отправить").removeClass("active");
            });
    });

    initErrorsBlocks();    
});
