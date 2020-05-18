import React from "react";

import PropTypes from 'prop-types';
import classNames from "classnames";
import translateCode from "src/codeTranslator/translateCode";

import styles from "./Text.less";

class Text extends React.Component {
	constructor(props) {
		super(props);
		this.textContainer = null;
	}

	componentDidMount() {
		this.translateTex();
	}

	componentDidUpdate(prevProps, prevState, snapshot) {
		if(prevProps.content !== this.props.content) {
			this.translateTex();
		}
	}

	translateTex = () => {
		translateCode(this.textContainer, {}, { codeMirror: true });
	}

	render() {
		const { content, className, children, } = this.props;
		if(content) {
			return (
				<div
					ref={ ref => this.textContainer = ref }
					className={ classNames(styles.text, className) }
					dangerouslySetInnerHTML={ { __html: content } }
				/>
			);
		}
		return (
			<div
				ref={ ref => this.textContainer = ref }
				className={ classNames(styles.text, className) }>
				{ children }
			</div>
		);
	}
}

Text.propTypes = {
	className: PropTypes.string,
	content: PropTypes.string,
}


export default Text;
