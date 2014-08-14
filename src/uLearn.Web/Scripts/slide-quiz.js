function submitQuiz(courseId, slideIndex, needingCount) {
    if (areAllAnswered(needingCount)) {
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
        markSubmit(courseId, slideIndex);
        $.ajax(
               {
                   type: "POST",
                   url: $("#SubmitQuiz").data("url"),
                   data: {
                       courseId: courseId,
                       slideIndex: slideIndex,
                       answer: answer
                   }
               }).success(function (ans) {
                   var wrongIndexes = ans.split('*');
                   $(".quiz-block-input").each(function () {
                       $(this).find("input").addClass("right-quiz");
                   });
                   for (var i in wrongIndexes) {
                       $("#" + wrongIndexes[i] + "quizBlock").removeClass("right-quiz").addClass("wrong-quiz");
                       $("#" + wrongIndexes[i] + "_example_quizItem").show();
                   }
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

function markSubmit(courseId, slideIndex) {
  $.ajax(
     {
         type: "POST",
         url: $("#GetRightAnswersToQuiz").data("url"),
         data: {
             courseId: courseId,
             slideIndex: slideIndex,
         }
     }).success(function (ans) {
          markAns(ans);
      })
     .fail(function (req) {
         console.log(req.responseText);
     })
     .always(function (ans) {
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


function QuizAnswer(type, quizId, itemId, text) {
    this.QuizType = type;
    this.QuizId = quizId;
    this.ItemId = itemId;
    this.Text = text;
}

function markAns(s) {
    if (s != "none") {
        var toMarkAsRight = s.split('||');
        var rightAnswersId = [];
        for (var q in toMarkAsRight) {
            if (toMarkAsRight[q] != "") {
                var content = toMarkAsRight[q].split('=');
                var blockId = content[0];
                if (content[1] == "True" || content[1] == "False") {
                    $("#" + blockId + content[1]).parent().addClass("right-quiz");
                    $("#" + blockId + content[1]).parent().parent().children('i').addClass('right-quiz glyphicon glyphicon-ok');
                    rightAnswersId.push(blockId + content[1]);
                } else {
                    var items = content[1].split("*");
                    for (var itemIndex in items) {
                        $("#" + blockId + items[itemIndex]).parent().addClass("right-quiz");
                        $("#" + blockId + items[itemIndex]).parent().parent().children('i').addClass('right-quiz glyphicon glyphicon-ok');
                        rightAnswersId.push(blockId + items[itemIndex]);
                    }
                }
            }
        }


        $(".quiz").each(function() {
            var $e = (($(this).children('label').children('input')));
            if (!$e.parent().parent().children('i').hasClass('right-quiz'))
                $e.parent().parent().children('i').addClass('glyphicon glyphicon-remove wrong-quiz');
            if ($e.is(':checked')) {
                if (($.inArray(($e.attr('id')), rightAnswersId)) == -1)
                ($e.parent().addClass("wrong-quiz"));
            }
        });

        for (i in rightAnswersId) {
            if ((!$("#" + rightAnswersId[i]).is(':checked')))
            ($("#" + rightAnswersId[i]).parent().removeClass("right-quiz").addClass("wrong-quiz"));
        }
    }
}