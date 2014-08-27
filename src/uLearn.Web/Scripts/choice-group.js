function choiceGroup() {
	var $field = $("#group-field");
	var choiceText = $("#group-select").val();
	var fieldText = $field.val();
	var text;
	if (choiceText == 'Выберите группу') {
		text = fieldText;
	} else {
		text = choiceText;
	}
	$.ajax(
	{
		type: "POST",
		url: $field.data("url"),
		data: { groupName: text }
	});
}