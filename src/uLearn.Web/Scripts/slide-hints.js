function addHints(index) {
    $.ajax(
{
    type: "POST",
    url: $("#hintPanel"+index).data("url"),
    data: ""
}).success(function (ans) {
    $('#hintPanel' + index).addClass("visited-hint").removeClass("panel-heading");
        })
    .fail(function (req) {
        console.log(req.responseText);
    })
    .always(function (ans) {
    });
}

function getHints() {
    $.ajax(
{
    type: "POST",
    url: $("#hintsStore").data("url"),
    data: ""
}).success(function (ans) {
    var hints = ans.split(' ');
            for (var hint in hints)
                $('#hintPanel' + hints[hint]).addClass("visited-hint").removeClass("panel-heading");
    console.log(ans);
})
    .fail(function (req) {
        console.log(req.responseText);
    })
    .always(function (ans) {
    });
}