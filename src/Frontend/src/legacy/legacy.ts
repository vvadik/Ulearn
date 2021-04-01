import 'webpack-jquery-ui';
import 'bootstrap';

import 'bootstrap/dist/css/bootstrap.min.css';
require('src/legacy/styles/font-awesome.global.css');
require('src/legacy/styles/ulearn.global.css');

//legacy scripts
import tooltips from './scripts/tooltips.js';
import activateClipboard from './scripts/activate-clipboard.js';
import buttons from './scripts/buttons.js';
import slideChecking from './scripts/slide-checking.js';
import slideEditor from './scripts/slide-editor.js';
import slideModals from './scripts/slide-modals.js'; //deprecated?
import slideQuiz from './scripts/slide-quiz.js';
import slideSpoilers from './scripts/slide-spoilers.js'; //reuse spoiler react
import slideTex from './scripts/slide-tex.js'; //reuse tex react
import slideUtils from './scripts/slide-utils.js';
import slideVersions from './scripts/slide-versions.js';
import groupsEditor from './scripts/groups-editor.js';
import certificatesEditor from './scripts/certificates-editor.js';
import analyticsHighcharts from './scripts/analytics-highcharts.js';
import analytics from './scripts/analytics.js';
import courseStatistics from './scripts/course-statistics.js';
import suggestMailNotifications from './scripts/suggest-mail-notifications.js';
import diagnostics from './scripts/diagnostics.js';
import formsReplaceAction from './scripts/forms.replace-action.js';
import grader from './scripts/grader.js';
import notificationsSettings from './scripts/notifications-settings.js';
import profile from './scripts/profile.js';
import notifications from './scripts/notifications.js';
import stepik from './scripts/stepik.js';
import modals from './scripts/modals.js';
import connectCheckboxes from './scripts/connect-checkboxes.js'; //?check
import { antiplagiarism, antiplagiarismChart } from './scripts/antiplagiarism.js';
import styleErrorsSettings from './scripts/style-errors-settings.js';
import studentSubmissions from './scripts/student-submissions.js';
import reactRenderer from './scripts/react-renderer.js';

//scripts used in cshtml by backend renderer
import diffHtml from './scripts/slide-diff.js';
import likeSolution from './scripts/slide-solutions-like.js';
import loginForContinue from './scripts/slide-guest.js'; //reuse login for continue react
import {
	closePopup,
	openPopup,
	ToggleButtonClass,
	ToggleDropDownClass,
	ToggleSystemRoleOrAccess,
} from './scripts/users-list.js';

/*deprecated section
respond
likely
slide-comments
slide-hints
slide-popovers
slide-run
 */

window.documentReadyFunctions = [
	tooltips,
	activateClipboard,
	buttons,
	slideChecking,
	slideEditor,
	slideModals,
	slideQuiz,
	slideSpoilers,
	slideTex,
	slideUtils,
	slideVersions,
	groupsEditor,
	certificatesEditor,
	analyticsHighcharts,
	analytics,
	courseStatistics,
	suggestMailNotifications,
	diagnostics,
	formsReplaceAction,
	grader,
	notificationsSettings,
	profile,
	notifications,
	stepik,
	modals,
	connectCheckboxes,
	antiplagiarism,
	antiplagiarismChart,
	styleErrorsSettings,
	studentSubmissions,
	reactRenderer,
];

window.loginForContinue = loginForContinue;
window.diffHtml = diffHtml;
window.likeSolution = likeSolution;
window.ToggleSystemRoleOrAccess = ToggleSystemRoleOrAccess;
window.ToggleButtonClass = ToggleButtonClass;
window.ToggleDropDownClass = ToggleDropDownClass;
window.openPopup = openPopup;
window.closePopup = closePopup;
