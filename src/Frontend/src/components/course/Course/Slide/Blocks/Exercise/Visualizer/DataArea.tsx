import React from 'react';

import { Center, Textarea } from "ui";

import styles from './DataArea.less';
import texts from './Visualizer.texts';

interface DataAreaProps {
	input: string;
	output: string;
	updateInput: (value: string) => void;
}

function DataArea({ input, updateInput, output } : DataAreaProps) : React.ReactElement {
	return (
		<div className={ styles.dataArea }>
			<Center>
				<Textarea
					value={ input }
					className={ styles.inputArea }
					autoResize
					onValueChange={ updateInput }
					placeholder={ texts.dataArea.inputData }
				/>
				<Textarea
					value={ output }
					className={ styles.inputArea }
					autoResize
					readOnly={ true }
					placeholder={ texts.dataArea.outputData }
				/>
			</Center>
		</div>
	);
}

export default DataArea;
