import React from "react";

import { Hint, } from "ui";
import { EyeClosed } from "icons";

import classNames from "classnames";

import PropTypes from "prop-types";

import styles from "./BlocksWrapper.less";

const hiddenHintText = "Студенты не видят этот блок";

function BlocksWrapper({ children, className, isBlock, isHidden, isContainer, withoutBottomPaddigns, withoutTopPaddings, }) {
	const isHiddenBlock = isBlock && isHidden;
	const isHiddenSlide = !isBlock && isHidden;

	const wrapperClassNames = classNames(
		styles.wrapper,
		styles.withPaddings,
		{ [styles.withoutTopPaddings]: withoutTopPaddings },
		{ [styles.withoutBottomPaddigns]: withoutBottomPaddigns },
		{ [styles.hiddenBackgroundColor]: isHiddenBlock },
		{ [styles.hiddenSlide]: isHiddenSlide },
		className
	);

	return (
		<React.Fragment>
			{ isContainer
				? children
				: <div className={ wrapperClassNames }>
					{ !isContainer && isHiddenBlock && renderEyeHint() }
					{ children }
				</div>
			}
		</React.Fragment>);


	function renderEyeHint() {
		const wrapperClass = classNames(styles.eyeClosedWrapper);

		return (
			<div className={ wrapperClass }>
				<Hint pos={ "top right" } text={ hiddenHintText }>
					<EyeClosed/>
				</Hint>
			</div>
		);
	}
}


BlocksWrapper.propTypes = {
	className: PropTypes.string,
	isBlock: PropTypes.bool,
	withoutBottomPaddigns: PropTypes.bool,
	withoutTopPaddings: PropTypes.bool,
	isHidden: PropTypes.bool,
	isContainer: PropTypes.bool,
}

export default BlocksWrapper;
