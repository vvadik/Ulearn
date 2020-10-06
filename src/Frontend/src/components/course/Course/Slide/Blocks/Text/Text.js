import React from "react";

import PropTypes from 'prop-types';
import classNames from "classnames";
import translateCode from "src/codeTranslator/translateCode";
import scrollToView from "src/utils/scrollToView";

import styles from "./Text.less";

class Text extends React.Component {
	constructor(props) {
		super(props);
		this.textContainer = null;
	}

	componentDidMount() {
		this.translateTex();
		const anchors = Array.from(this.textContainer.getElementsByTagName('a'));
		const hashAnchorsLinks = anchors.filter(a => a.hash);

		const hashInUrl = window.location.hash;
		if(hashInUrl) {
			const hashToScroll = hashInUrl.replace('#', '');
			if(anchors.some(a => a.name === hashToScroll)) {
				this.scrollToHashAnchor(hashInUrl);
			}
		}

		for (const hashAnchor of hashAnchorsLinks) {
			const { hash } = hashAnchor;
			hashAnchor.addEventListener('click', (e) => {
				e.stopPropagation();
				e.preventDefault();
				this.scrollToHashAnchor(hash);
			});
		}
	}

	scrollToHashAnchor = (hash) => {
		window.history.pushState(null, null, hash);

		const anchors = document.querySelectorAll(`a[name=${ hash.replace('#', '') }]`);
		if(anchors.length > 0) {
			scrollToView({ current: anchors[0] });
		}
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
