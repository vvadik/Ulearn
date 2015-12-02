function submitQuiz(courseId, slideIndex, expectedCount, isLti) {
	if (areAllAnswered(expectedCount)) {
		var answers = [];
		$(".quiz").each(function () {
			var id = $(this).children('label').children('input').attr('id'); //id of quiz
			var content = id.split('quizBlock');
			if ($(this).hasClass('quiz-block-input')) {
				var val = $(this).children('label').children('input').val();
				answers.push(new QuizAnswer("Text", content[0], "", val));
			}
			if ($('#' + id).is(':checked')) {
				var type = content[1] == "True" || content[1] == "False" ? "TrueFalse" : "Checkbox";
				answers.push(new QuizAnswer(type, content[0], content[1], ""));
			}
		});
		var answer = JSON.stringify(answers);
		$.ajax(
			{
				type: "POST",
				url: $("#SubmitQuiz").data("url"),
				data: {
					courseId: courseId,
					slideIndex: slideIndex,
					answer: answer,
					isLti: isLti
				}
			}).success(function (ans) {
				$("#quiz-status").text("Проверяется...");
				window.scrollTo(0, 0);
				window.location.reload();
			})
			.fail(function (req) {
				console.log(req.responseText);
			})
			.always(function (ans) {
			});
		console.log(answers);
		return answer;
	} else
		alert("Fill this quiz!");
};


function areAllAnswered(needingCount) {
	var realCount = 0;
	$(".quiz-block-mark").each(function () {
		if ($(this).children().children().children().is(':checked')) {
			realCount++;
			return;

		}
	});
	$(".quiz-block-input").each(function () {
		if ($(this).children('label').children('input').val() != "") {
			realCount++;
			return;
		}
	});
	return realCount >= needingCount;
}


function QuizAnswer(type, quizId, itemId, text) {
	this.QuizType = type;
	this.QuizId = quizId;
	this.ItemId = itemId;
	this.Text = text;
}

function ShowQuizStatistic(e) {
	e.preventDefault();
	var button = $(e.target);
	button.siblings().removeClass('btn-primary');
	button.addClass('btn-primary');
	var divId = button.data('div-id');
	var div = $('#' + divId);
	div.siblings('div.statistic-container').css('display', 'none');
	if (div.text() == '')
		div.load(div.data('url'));
	div.css('display', 'block');
}