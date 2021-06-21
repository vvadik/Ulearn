import React from "react";
import { Link } from "react-router-dom";

import styles from './AntiplagiarismHeader.less';
import texts from './AntiplagiarismHeader.texts';
import { Button, } from "ui";
import cn from "classnames";

export type SuspicionLevel = 'notChecking' | 'accepted' | 'warning' | 'strongWarning' | 'running';

export interface AntiplagiarismInfo {
	suspicionLevel: SuspicionLevel;
	suspicionCount: number;
}

interface State extends AntiplagiarismInfo {
	loadingInfo: boolean;
}

export interface Props {
	shouldCheck: boolean;
	fixed: boolean;

	getAntiPlagiarismStatus: () => Promise<AntiplagiarismInfo>;
	onZeroScoreButtonPressed: () => void;
}

function AntiplagiarismHeader({
	getAntiPlagiarismStatus,
	shouldCheck,
	onZeroScoreButtonPressed,
	fixed,
}: Props): React.ReactElement {
	const [state, setState] = React.useState<State>(
		{
			suspicionLevel: shouldCheck ? 'running' : 'notChecking',
			suspicionCount: 0,
			loadingInfo: false,
		});

	if(shouldCheck && !state.loadingInfo && state.suspicionLevel === 'running') {
		setState({ ...state, loadingInfo: true });
		getAntiPlagiarismStatus().then(c => setState({ ...state, loadingInfo: false, ...c }));
	}

	let text: React.ReactNode;
	let color = '';
	switch (state.suspicionLevel) {
		case "accepted": {
			text = texts.getSuspicionText(0);
			color = styles.noSuspicionColor;
			break;
		}
		case "running": {
			text = texts.runningText;
			color = styles.runningColor;
			break;
		}
		case "strongWarning": {
			text = texts.getSuspicionText(state.suspicionCount, true);
			color = styles.strongSuspicionColor;
			break;
		}
		case "warning": {
			text = texts.getSuspicionText(state.suspicionCount);
			color = styles.suspicionColor;
			break;
		}
		case "notChecking": {
			text = texts.notCheckingText;
			color = styles.notCheckingColor;
			break;
		}
	}

	return (
		<div className={ cn(styles.header, color, { [styles.sticky]: fixed }) }>
			<span className={ styles.text }>{ text }</span>
			{ (shouldShowWarning(state.suspicionLevel)) &&
			<>
				<Link className={ cn(styles.seeDetailsLink, styles.text) } to={ '' }>
					{ texts.buildLinkToAntiplagiarismText }
				</Link>
				<Button
					className={ styles.scoreZeroButton }
					onClick={ onZeroScoreButtonPressed }
					use={ 'danger' }>
					{ texts.scoreZeroText }
				</Button>
			</> }
		</div>
	);

	function shouldShowWarning(suspicionLevel: SuspicionLevel) {
		return suspicionLevel === 'warning' || suspicionLevel === 'strongWarning';
	}
}

export default AntiplagiarismHeader;
