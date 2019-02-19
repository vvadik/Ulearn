import React, {Component} from 'react';
import PropTypes from "prop-types";
import Hint from "@skbkontur/react-ui/components/Hint/Hint";

import styles from "./commentSendForm.less";

class MarkdownButton extends Component {

	render() {
		const { markupByOperation } = this.props;
		return (
			<div className={styles.markdownButtons}>
				<span className={styles.markdownText}>Поддерживаем markdown</span>
				{Object.values(markupByOperation)
					.map(operation => this.renderMarkdownButton(operation))}
			</div>
		);
	}

	renderMarkdownButton(operation) {
		return (
			<Hint pos="top" text={this.renderHint(operation)}>
				<button onClick={this.onClick(operation)} type="button">
					<svg
						width={20}
						height={18}
						xmlns="http://www.w3.org/2000/svg"
						fill="none"
					>
						{operation.icon}
					</svg>
				</button>
			</Hint>
		)
	}

	renderHint({description, markup, hotkey}) {
		return <span className={styles._yellow}>{markup}<span className={styles._white}>{description}</span>{markup}
			<br/><span className={styles._yellow}>{hotkey.asText}</span>
		</span>;
	};

	onClick = (operation) => () => {
		this.props.onClick(operation);
	};
}

MarkdownButton.propTypes = {
	markupByOperation: PropTypes.object,
	onClick: PropTypes.func,
};

export default MarkdownButton;