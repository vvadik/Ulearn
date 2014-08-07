function submitQuiz(courseId, slideIndex, needingCount) {
    if (areAllAnswered(needingCount)) {
        var answers = [];
        markAnswers();
        $(".quiz").each(function() {
            var id = $(this).children('label').children('input').attr('id');
            if (id.indexOf('placetoinsert') == -1) {
                if ($('#' + id).is(':checked')) {
                    answers.push(id);
                }
            } else {
                var input = $('#' + id).val();
                if (input != "") {
                    var ans = id.replace('placetoinsert', input);
                    answers.push(ans);
                }
            }
        });
        console.log(answers.join('*'));
        var toSend = (answers.join('*'));
        $.ajax(
            {
                type: "POST",
                url: $("#quizSub").data("url"),
                data: {
                    courseId: courseId,
                    slideIndex: slideIndex,
                    answer: toSend
                }
            }).success(function(ans) {
                var wrongIndexes = ans.split('*');
                for (var i in wrongIndexes) {
                    $("#" + wrongIndexes[i] + "_placetoinsert_quizItem").addClass("wrong-quiz");
                    $("#" + wrongIndexes[i] + "_example_quizItem").show();
                }
                console.log(ans);
            })
            .fail(function(req) {
                console.log(req.responseText);
            })
            .always(function(ans) {
            });
    } else
        alert("Fill this quiz!");
};

function markAnswers() {
    $(".false").each(function () {
        $(this).addClass("glyphicon glyphicon-remove wrong-quiz");
    });
    $(".quiz").each(function () {
        if ($(this).children('label').children('input').is(':checked')) {
            if ($(this).children('i').hasClass("false"))
                $(this).children('label').addClass("wrong-quiz");
        } else {
            if ($(this).children('i').hasClass("true") && $(this).parent().hasClass("checkbox"))
                $(this).children('label').addClass("wrong-quiz");
        }
    });
    $(".true").each(function () {
        $(this).addClass('glyphicon glyphicon-ok right-quiz');
    });
}

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