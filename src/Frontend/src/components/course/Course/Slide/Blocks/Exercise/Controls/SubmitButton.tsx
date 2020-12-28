import React from "react";
import { Button, Tooltip } from "ui";

import styles from './Controls.less';

import texts from "../Exercise.texts";

interface Props {
	valueChanged: boolean,
	submissionLoading: boolean,

	onSendExerciseButtonClicked: () => void,
}

function SubmitButton(props: Props): React.ReactElement {
	const { valueChanged, submissionLoading, onSendExerciseButtonClicked, } = props;

	return (
		<span className={ styles.exerciseControls }>
				<Tooltip
					pos={ "bottom center" }
					trigger={ "hover&focus" }
					render={ renderSubmitCodeHint }>
					<Button
						loading={ submissionLoading }
						use={ "primary" }
						disabled={ !valueChanged }
						onClick={ onSendExerciseButtonClicked }>
						{ texts.controls.submitCode.text }
					</Button>
				</Tooltip>
			</span>
	);

	function renderSubmitCodeHint(): React.ReactNode {
		const { valueChanged } = props;

		return valueChanged ? null : <span>{ texts.controls.submitCode.hint }</span>;
	}
}

export default SubmitButton;
