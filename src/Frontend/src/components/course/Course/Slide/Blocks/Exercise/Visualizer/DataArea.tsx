import React from 'react';

import { Textarea } from "ui";

import styles from './DataArea.less';
import texts from './Visualizer.texts';

interface DataAreaProps {
	input: string;
	output: string;
	updateInput: (value: string) => void;
}

export const DataArea =
	({ input, updateInput, output } : DataAreaProps) : React.ReactElement =>
		(
		<div className={ styles.dataArea }>
			<div className={ styles.textArea }>
				<p>{ texts.dataArea.inputData }</p>
				<Textarea
					value={ input }
					maxRows={ 10 }
					onValueChange={ updateInput }
				/>
			</div>
			<div className={ styles.textArea }>
				<p>{ texts.dataArea.outputData }</p>
				<Textarea
					value={ output }
					maxRows={ 10 }
					readOnly={ true }
				/>
			</div>
		</div>
	);
