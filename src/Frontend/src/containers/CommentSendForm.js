import connect from "react-redux/es/connect/connect";
import CommentSendForm from "../components/comments/CommentSendForm/CommentSendForm";
import { COURSES__COURSE_ENTERED } from '../consts/actions';

function mapStateToProps(state) {
	return {
		account: state.account,
	}
}

function mapDispatchToProps(dispatch, state) {
	return {
		enterToCourse: (text) => dispatch({
			type: COURSES__COURSE_ENTERED,
			courseId: state.courseId,
			slideId: state.slideId,
		}),
	}
}

export default connect(mapStateToProps, mapDispatchToProps)(CommentSendForm);
