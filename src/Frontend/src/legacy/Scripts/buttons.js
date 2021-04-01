import 'bootstrap-fileinput/js/fileinput.min';
import 'bootstrap-fileinput/js/locales/ru';
import 'bootstrap-fileinput/css/fileinput.min.css';

export default function () {
	$('.btn').button();
	$('.btn-hover').hover(
		function () {
			$(this).addClass($(this).data("hover-class"));
		},
		function () {
			$(this).removeClass($(this).data("hover-class"));
		}
	);
	const input = $('input[type=file]');
	input.textContent = input.title;
	input.fileinput({
		showPreview: false,
		showCaption: true,
		showRemove: false,
		showUpload: false,
		language: 'ru',
		browseOnZoneClick: true,
	});
	$('.file-inputs').fileinput({
		showPreview: false,
		showCaption: true,
		showRemove: false,
		showUpload: false,
		language: 'ru',
		browseOnZoneClick: true,
	});
}
