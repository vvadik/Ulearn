import React from "react";

import { Toggle } from "ui";

import { setStudentMode, } from "src/actions/instructor";
import getSlideInfo from "src/utils/getSlideInfo";

import { connect } from "react-redux";
import { withRouter } from "react-router-dom";
import PropTypes from "prop-types";

import styles from "./StudentMode.less";

function StudentMode({ isStudentMode, setStudentMode, containerClass, location, }) {
	const showForStudentToggleChanged = (value) => {
		setStudentMode(value);
	}
	const slideInfo = getSlideInfo(location);

	if(!slideInfo || slideInfo.isReview || slideInfo.isLti) {
		return null;
	}

	return (
		<div className={ containerClass }>
			<Toggle
				checked={ isStudentMode }
				onValueChange={ showForStudentToggleChanged }
			/>
			<span className={ styles.toggleText }> Режим студента </span>
		</div>
	)
}

StudentMode.propTypes = {
	containerClass: PropTypes.string,
	location: PropTypes.object,
	isStudentMode: PropTypes.bool.isRequired,
	setStudentMode: PropTypes.func.isRequired,
}

const mapStateToProps = (state,) => {
	return {
		isStudentMode: state.instructor.isStudentMode,
	}
};
const mapDispatchToProps = (dispatch) => ({
	setStudentMode: (isStudentMode) => dispatch(setStudentMode(isStudentMode)),
});

export default connect(mapStateToProps, mapDispatchToProps)(withRouter(StudentMode));