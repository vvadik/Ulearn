import React from "react";
import { Hint } from "ui";

import { isMobile } from "src/utils/getDeviceType";

import { MarkdownDescription, MarkdownOperation } from "src/consts/comments";

import styles from "./MarkdownButtons.less";

interface Props {
	markupByOperation: MarkdownDescription;

	onClick: (operation: MarkdownOperation) => void;
}

function MarkdownButtons(props: Props): React.ReactElement {
	const { markupByOperation, onClick } = props;

	return (
		<div className={ styles.markdownButtons }>
			<span className={ styles.markdownText }>Поддерживаем Markdown</span>
			{ Object.entries(markupByOperation)
				.map(([name, operation]) =>
					<div key={ name } className={ styles.buttonBlock }>
						{ !isMobile() ?
							<Hint
								pos="bottom"
								text={ renderHint(operation) }>
								{ renderMarkdownButton(name, operation) }
							</Hint> : renderMarkdownButton(name, operation) }
					</div>) }
		</div>
	);

	function renderMarkdownButton(name: string, operation: MarkdownOperation) {
		return (
			<button
				className={ styles.button }
				onClick={ () => handleClick(operation) }
				type="button">
				<svg
					width={ 20 }
					height={ 18 }
					xmlns="http://www.w3.org/2000/svg"
					fill="none">
					{ operation.icon }
				</svg>
			</button>
		);
	}

	function renderHint({ description, markup, hotkey }: MarkdownOperation) {
		return (
			<span className={ styles.lightYellow }>
				{ markup }
				<span className={ styles.white }>{ description }</span>
				{ markup }<br/>
				{ !isMobile() &&
				<span className={ styles.lightYellow }>{ hotkey.asText }</span> }
			</span>);
	}

	function handleClick(operation: MarkdownOperation) {
		onClick(operation);
	}
}

export default MarkdownButtons;
