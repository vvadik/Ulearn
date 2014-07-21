function handleMark() {
    $.ajax(
    {
        type: "POST",
        url: $("#" + $("input:radio[name='marks']:checked").val()).data("url"),
        data: ""
    }).success(function (ans) {

    })
		.fail(function (req) {
		    console.log(req.responseText);
		})
		.always(function (ans) {
		});
};