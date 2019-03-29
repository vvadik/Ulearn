import React, { Component } from "react";
import PropTypes from "prop-types";
import Hint from "@skbkontur/react-ui/components/Hint/Hint";

import styles from "./MarkdownButtons.less";

class MarkdownButtons extends Component {

	render() {
		const {markupByOperation} = this.props;

		return (
			<div className={styles.markdownButtons}>
				<span className={styles.markdownText}>Поддерживаем markdown</span>
				{Object.entries(markupByOperation)
				.map(([name, operation]) => this.renderMarkdownButton(name, operation))}
			</div>
		);
	}

	renderMarkdownButton(name, operation) {
		return (
			<div key={name}>
				<Hint pos="top" text={this.renderHint(operation)} disableAnimations={false} useWrapper={true}>
					<button className={styles.button} onClick={this.onClick(operation)} type="button">
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
			</div>
		)
	}

	renderHint({description, markup, hotkey}) {
		return <span className={styles._yellow}>{markup}<span className={styles._white}>{description}</span>{markup}
			<br /><span className={styles._yellow}>{hotkey.asText}</span>
		</span>;
	};

	onClick = (operation) => () => {
		this.props.onClick(operation);
	};
}

MarkdownButtons.propTypes = {
	markupByOperation: PropTypes.object,
	onClick: PropTypes.func,
};

export default MarkdownButtons;