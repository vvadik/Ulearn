function submitQuiz(courseId, slideIndex) {
    var answers = [];
    $(".quiz").each(function () {
        var id = $(this).attr('id');
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
//    $.ajax(
//{
//    type: "POST",
//    url: $("#quizSub").data("url"),
//    data: {
//        courseId: courseId, slideIndex: slideIndex, answer: toSend
//    }
//}).success(function (ans) {
//    var wrongIndexes = ans.split('*');
//            for (var i in wrongIndexes)
//                $("#" + wrongIndexes[i] + "_quizBlock").addClass("wrong-quiz");
//        })
//    .fail(function (req) {
//        console.log(req.responseText);
//    })
//    .always(function (ans) {
//    });
}
