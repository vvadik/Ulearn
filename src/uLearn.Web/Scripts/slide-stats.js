function handleRate(rate) {
    if (rate == "NotUnderstand")
        $("#ask_question_window").click();
    $("#notwatched").removeClass("not-watched");
    $("#ratings").removeClass("bounce");
    $.ajax(
    {
        type: "POST",
        url: $("#ratesBar").data("url"),
        data: {rate:rate}
    }).success(function (ans) {
            FillRate(rate);
        })
		.fail(function (req) {
		    console.log(req.responseText);
		})
		.always(function (ans) {
		});
};

function FillRate(rate) {
    var switcher = rate.toLowerCase();
    var colors = {};
    colors["good"] = "btn-success";
    colors["notunderstand"] = "btn-danger";
    colors["trivial"] = "btn-info";
    var rated = false;
    if (colors[switcher] != undefined)
        rated = true;
    for (var i in colors) {
            $("#" + i).parent().removeClass(colors[i]);
        }
    if (rated)
        $("#next_slide_button").removeClass("block-next");
    else
        $("#notwatched").addClass("not-watched");
    $("#" + switcher).parent().button('toggle');
    $("#" + switcher).parent().addClass(colors[switcher]);
};