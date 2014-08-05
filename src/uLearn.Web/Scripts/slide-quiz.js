function submitQuiz(courseId, slideId) {
    $.ajax(
{
    type: "POST",
    url: $("#quizSub").data("url"),
    data: {
        courseId: courseId, slideId: slideId, hintId: index
    }
}).success(function (ans) {
            alert("Submitted");
        })
    .fail(function (req) {
        console.log(req.responseText);
    })
    .always(function (ans) {
    });
}