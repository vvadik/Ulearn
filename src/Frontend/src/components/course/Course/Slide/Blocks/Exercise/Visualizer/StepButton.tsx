import React, { MouseEventHandler } from "react";
import { Button } from "ui";

import cn from 'classnames';

import styles from './Controls.less';

export interface Props {
	onClick: MouseEventHandler,
	text: string,
	disabled: boolean,
}

function StepButton(props: Props): React.ReactElement {
	const { onClick, text, disabled, } = props;

	return (
		<span className={ cn(styles.exerciseControls, styles.sendButton) }>
			<Button
				use={ "default" }
				onClick={ onClick }
				disabled={ disabled }>
				{ text }
			</Button>
		</span>
	);
}

export default StepButton;
