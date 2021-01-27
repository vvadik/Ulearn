import React from "react";

import { Toggle } from "ui";

import { studentModeToggleAction, } from "src/actions/instructor";
import getSlideInfo from "src/utils/getSlideInfo";

import { Dispatch } from "redux";

import { connect } from "react-redux";
import { RouteComponentProps, withRouter } from "react-router-dom";

import { RootState } from "src/models/reduxState";

import styles from "./StudentMode.less";

interface Props extends RouteComponentProps {
	containerClass?: string,
	isStudentMode: boolean,
	setStudentMode: (value: boolean) => void,
}

function StudentMode({ isStudentMode, setStudentMode, containerClass, location, }: Props) {
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
	);

	function showForStudentToggleChanged(value: boolean) {
		setStudentMode(value);
	}
}

const mapStateToProps = (state: RootState,) => {
	return {
		isStudentMode: state.instructor.isStudentMode,
	};
};
const mapDispatchToProps = (dispatch: Dispatch) => ({
	setStudentMode: (isStudentMode: boolean) => dispatch(studentModeToggleAction(isStudentMode)),
});

export default connect(mapStateToProps, mapDispatchToProps)(withRouter(StudentMode));
