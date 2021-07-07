import React from "react";
import { Link } from "react-router-dom";
import { Button, } from "ui";

import cn from "classnames";

import { AntiplagiarismStatusResponse, SuspicionLevel } from "src/models/instructor";

import styles from './AntiplagiarismHeader.less';
import texts from './AntiplagiarismHeader.texts';


interface State extends AntiplagiarismStatusResponse {
	loadingInfo: boolean;
}

export interface Props {
	shouldCheck: boolean;
	fixed: boolean;

	getAntiplagiarismStatus: () => Promise<AntiplagiarismStatusResponse | string>;
	onZeroScoreButtonPressed: () => void;
}

function AntiplagiarismHeader({
	getAntiplagiarismStatus,
	shouldCheck,
	onZeroScoreButtonPressed,
	fixed,
}: Props): React.ReactElement {
	const [state, setState] = React.useState<State>(
		{
			suspicionLevel: shouldCheck ? 'running' : 'notChecking',
			suspiciousAuthorsCount: 0,
			loadingInfo: false,
			status: 'notChecked',
		});

	if(shouldCheck && !state.loadingInfo && state.suspicionLevel === 'running') {
		setState({ ...state, loadingInfo: true });
		getAntiplagiarismStatus().then(c => {
			const repsonse = c as AntiplagiarismStatusResponse;
			if(repsonse) {
				setState({ ...state, loadingInfo: false, ...repsonse });
			}
		});
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
			text = texts.getSuspicionText(state.suspiciousAuthorsCount, true);
			color = styles.strongSuspicionColor;
			break;
		}
		case "warning": {
			text = texts.getSuspicionText(state.suspiciousAuthorsCount);
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
