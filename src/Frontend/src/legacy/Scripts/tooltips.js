window.documentReadyFunctions = window.documentReadyFunctions || [];

require('webpack-jquery-ui/tooltip');

window.documentReadyFunctions.push(function() {
	$('[data-toggle="tooltip"]').tooltip();
});
