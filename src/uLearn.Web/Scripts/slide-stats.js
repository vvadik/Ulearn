function handleRate(rate) {
    $.ajax(
    {
        type: "POST",
        url: $("#ratesBar").data("url"),
        data: {rate:rate}
    }).success(function (ans) {
            console.log(ans);
        })
		.fail(function (req) {
		    console.log(req.responseText);
		})
		.always(function (ans) {
		});
};

function FillRate(rate) {
    var switcher = rate.toLowerCase();
    console.log(switcher);
    $("#" + switcher).parent().button('toggle');
};