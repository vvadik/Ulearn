import React from "react";
import { Button } from "ui";

import cn from 'classnames';

import styles from './Controls.less';

export interface Props {
	onClick: React.MouseEventHandler,
	text: string,
}

function RunButton(props: Props): React.ReactElement {
	const { onClick, text, } = props;

	return (
		<span className={ cn(styles.exerciseControls, styles.sendButton) }>
			<Button
				use={ "primary" }
				onClick={ onClick }>
				{ text }
			</Button>
		</span>
);
}

export default RunButton;
