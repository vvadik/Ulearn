function ShowPanel(e) {
	e.preventDefault();
	var button = $(e.target);
	button.siblings().removeClass('btn-primary');
	button.addClass('btn-primary');
	var divId = button.data('div-id');
	var div = $('#' + divId);
	div.siblings('div.statistic-container').css('display', 'none');
	if (div.text() == '')
		div.load(div.data('url'));
	div.css('display', 'block');
}

$(document).ready(function() {
	$('.js__onchange-send-form').change(function () {
		$(this).closest('form').submit();
	});
})