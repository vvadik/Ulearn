import React from "react";

import { Hint, } from "ui";
import { EyeClosed } from "icons";

import classNames from "classnames";

import PropTypes from "prop-types";

import styles from "./BlocksWrapper.less";

const hiddenHintText = "Студенты не видят этот блок";

function BlocksWrapper({ children, className, isBlock, isHidden, withoutBottomPaddings, withoutTopPaddings, withoutEyeHint, }) {
	const isHiddenBlock = isBlock && isHidden;
	const isHiddenSlide = !isBlock && isHidden;

	const wrapperClassNames = classNames(
		styles.wrapper,
		styles.withPaddings,
		{ [styles.withoutTopPaddings]: withoutTopPaddings },
		{ [styles.withoutBottomPaddigns]: withoutBottomPaddings },
		{ [styles.hiddenBackgroundColor]: isHiddenBlock },
		{ [styles.hiddenSlide]: isHiddenSlide },
		className
	);

	return (
		<div className={ wrapperClassNames }>
			{ !withoutEyeHint && isHiddenBlock && renderEyeHint() }
			{ children }
		</div>
	);


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
	withoutBottomPaddings: PropTypes.bool,
	withoutTopPaddings: PropTypes.bool,
	withoutEyeHint: PropTypes.bool,
	isHidden: PropTypes.bool,
}

export default BlocksWrapper;
