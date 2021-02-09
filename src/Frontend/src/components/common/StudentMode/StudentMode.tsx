import React, { RefObject } from "react";
import { Dispatch } from "redux";
import { connect } from "react-redux";
import { RouteComponentProps, } from "react-router-dom";
import cn from "classnames";

import { Toggle } from "ui";

import { studentModeToggleAction, } from "src/actions/instructor";
import getSlideInfo from "src/utils/getSlideInfo";

import { RootState } from "src/models/reduxState";
import { DeviceType } from "src/consts/deviceType";

import styles from './StudentMode.less';

interface Props extends RouteComponentProps {
	isStudentMode: boolean;
	deviceType: DeviceType;
	containerClass?: string;
	setStudentMode: (value: boolean) => void;
}

function StudentMode({ isStudentMode, setStudentMode, location, deviceType, containerClass, }: Props) {
	const slideInfo = getSlideInfo(location);
	const refButton: RefObject<HTMLButtonElement> = React.createRef();
	const refSpan: RefObject<HTMLSpanElement> = React.createRef();

	if(!slideInfo || slideInfo.isReview || slideInfo.isLti) {
		return null;
	}

	return (
		<button className={ cn(styles.toggleWrapper, containerClass,) } onClick={ onClick } ref={ refButton }>
			<Toggle
				checked={ isStudentMode }
				onValueChange={ showForStudentToggleChanged }/>
			{ deviceType !== DeviceType.mobile && <span ref={ refSpan }> Режим студента </span> }
		</button>
	);

	function showForStudentToggleChanged(value: boolean) {
		setStudentMode(value);
	}

	function onClick(e: React.MouseEvent) {
		if(e.target === refButton.current || e.target === refSpan.current) {
			setStudentMode(!isStudentMode);
		}
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

export default connect(mapStateToProps, mapDispatchToProps)(StudentMode);
