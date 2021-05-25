import React, { ChangeEventHandler, ReactNode } from 'react';

import { Center, Textarea } from "ui";

import styles from './DataArea.less';
import texts from './Visualizer.texts';

export interface Props {
	input: string;
	output: string;
	updateInput: ChangeEventHandler;
}

class DataArea extends React.Component<Props> {
	render() : ReactNode {
		return (
			<div className={ styles["data-area"] }>
				<Center>
					<Textarea
						value={ this.props.input }
						className={ styles["input-area"] }
						autoResize
						onChange={ this.props.updateInput }
						placeholder={ texts.dataArea.inputData }
					/>
					<Textarea
						value={ this.props.output }
						className={ styles["input-area"] }
						autoResize
						readOnly={ true }
						placeholder={ texts.dataArea.outputData }
					/>
				</Center>
			</div>
		);
	}
}

export default DataArea;
