import React from "react";
import classnames from 'classnames';
import styles from "./ProgressBar.less";

interface Props {
	value: number;
	small?: boolean;
	color?: 'green' | 'blue';
	active?: boolean;
}

function ProgressBar({ value, small, color = 'green', active, }: Props): React.ReactElement {
	return (
		<div className={ classnames(styles.wrapper, { [styles.small]: small }, { [styles.active]: active }) }>
			<div
				className={ classnames(styles.value, { [styles.blue]: color === 'blue' }) }
				style={ { width: `${ value * 100 }%` } }/>
		</div>
	);
}

export default ProgressBar;
