function choiceGroup() {
	var $field = $("#group-field");
	var text = $field.val();
	$.ajax(
	{
		type: "POST",
		url: $field.data("url"),
		data: { groupName: text }
	});
}