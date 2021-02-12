import React, { createRef, RefObject } from "react";

import classNames from "classnames";
import translateCode from "src/codeTranslator/translateCode";
import scrollToView from "src/utils/scrollToView";

import styles from "./Text.less";

interface Props {
	className?: string,
	content?: string,
}

class Text extends React.Component<Props> {
	private textContainer: RefObject<HTMLDivElement> = createRef();

	componentDidMount(): void {
		this.translateTex();

		if(!this.textContainer.current) {
			return;
		}

		const anchors = Array.from(this.textContainer.current.getElementsByTagName('a'));
		//if href equal to origin + pathname + hash => <a href="#hash"> that means its navigation on slide via headers, we need to handle this via our modificated scrolling
		const hashAnchorsLinks = anchors.filter(
			a => window.location.origin + window.location.pathname + a.hash === a.href);

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

	scrollToHashAnchor = (hash: string): void => {
		window.history.pushState(null, '', hash);

		const anchors = document.querySelectorAll(`a[name=${ hash.replace('#', '') }]`);
		if(anchors.length > 0) {
			scrollToView({ current: anchors[0] });
		}
	};

	componentDidUpdate(prevProps: Props): void {
		if(prevProps.content !== this.props.content) {
			this.translateTex();
		}
	}

	translateTex = (): void => {
		if(this.textContainer.current) {
			translateCode(this.textContainer.current);
		}
	};

	render(): React.ReactNode {
		const { content, className, children, } = this.props;
		if(content) {
			return (
				<div
					ref={ this.textContainer }
					className={ classNames(styles.text, className) }
					dangerouslySetInnerHTML={ { __html: content } }
				/>
			);
		}
		return (
			<div
				ref={ this.textContainer }
				className={ classNames(styles.text, className) }>
				{ children }
			</div>
		);
	}
}


export default Text;
