function handleMark() {
    $.ajax(
    {
        type: "POST",
        url: $("#" + $("input:radio[name='marks']:checked").val()).data("url"),
        data: ""
    }).success(function (ans) {
        console.log(ans)
    })
		.fail(function (req) {
		    console.log(req.responseText);
		})
		.always(function (ans) {
		});
};

function giveMark() {
    $.ajax(
{
    type: "POST",
    url: $("#finder").data("url"),
    data: ""
}).success(function (ans) {
    var switcher = ans.toLowerCase();
    console.log(switcher);
    $("#" + switcher).prop('checked', true);
        })
    .fail(function (req) {
        console.log(req.responseText);
    })
    .always(function (ans) {
    });
};