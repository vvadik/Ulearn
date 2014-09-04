function changeGroup(userId) {
	var $textarea = $("#" + userId);
	var group = $textarea.val();
	$.ajax({
		type: "POST",
		url: $textarea.data("url"),
		data: {
			groupName: group,
			userId: userId
		}
	});
}