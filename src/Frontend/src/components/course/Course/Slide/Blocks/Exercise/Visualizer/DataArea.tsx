import React, { ChangeEventHandler, ReactNode } from 'react';

import { Center, Textarea } from "ui";

import styles from './DataArea.less';

export interface Props {
	input: string;
	output: string;
	updateInput: ChangeEventHandler;
}

class DataArea extends React.Component<Props> {
	render() : ReactNode {
		return (
			<div className={styles["data-area"]}>
				<Center>
					<Textarea
						value={ this.props.input }
						className={styles["input-area"]}
						autoResize
						onChange={this.props.updateInput}
						placeholder="Входные данные"
					/>
					<Textarea
						value={ this.props.output }
						className={styles["input-area"]}
						autoResize
						readOnly={true}
						placeholder="Выходные данные"
					/>
				</Center>
			</div>
		);
	}
}

export default DataArea;
