import 'webpack-jquery-ui';
import 'bootstrap';

import('bootstrap/dist/css/bootstrap.min.css');
import('src/legacy/Content/font-awesome.css');
import('src/legacy/Content/ulearn.less');

const scripts =
	"activate-clipboard\n" +
	"analytics\n" +
	"analytics-highcharts\n" +
	//"antiplagiarism\n" + should be after slide-editor
	"bootstrap-select-override\n" +
	"buttons\n" +
	"certificates-editor\n" +
	"connect-checkboxes\n" +
	"course-statistics\n" +
	"diagnostics\n" +
	"forms.replace-action\n" +
	"grader\n" +
	"groups-editor\n" +
	"likely\n" +
	"modals\n" +
	"notifications\n" +
	"notifications-settings\n" +
	"profile\n" +
	"react-renderer\n" +
	//"respond\n" + //deprecated?
	"slide-checking\n" +
	"slide-comments\n" + //deprecated
	"slide-diff\n" +
	"slide-editor\n" +
	"slide-guest\n" + //deprecated
	"slide-hints\n" + //deprecated
	"slide-modals\n" + //deprecated?
	//"slide-popovers\n" + //deprecated
	"slide-quiz\n" +
	"slide-run\n" + //deprecated
	"slide-solutions-like\n" +
	"slide-spoilers\n" +
	"slide-tex\n" + //deprecated by using react render
	"slide-utils\n" +
	"slide-versions\n" + //deprecated?
	"stepik\n" +
	"style-errors-settings\n" +
	"suggest-mail-notifications\n" +
	"tooltips\n" +
	"users-list\n" +
	"antiplagiarism\n" +
	"exercise/student-submissions\n";
//deprecated->deleted slide-localvisits


for (const module of scripts.split('\n')) {
	if(module.length === 0) {
		continue;
	}
	import('src/legacy/Scripts/' + module + '.js');
}
