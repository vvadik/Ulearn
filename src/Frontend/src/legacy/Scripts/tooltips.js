require('webpack-jquery-ui/tooltip');

export default function() {
	$('[data-toggle="tooltip"]').tooltip();
}
