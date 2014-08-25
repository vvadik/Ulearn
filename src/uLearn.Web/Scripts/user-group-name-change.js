function changeGroup(key) {
	console.log(key);
	var $textarea = $("#" + key);
	var group = $textarea.val();
	$.ajax({
		type: "POST",
		url: $textarea.data("url"),
		data: {
			groupName: group,
			userName : key
		}
	}).success(function (ans) {
	}).fail(function (req) {
	}).always(function (ans) {
	});
}