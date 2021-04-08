import React, { createRef, RefObject, useEffect, useRef } from "react";

import classNames from "classnames";
import translateCode from "src/codeTranslator/translateCode";
import scrollToView from "src/utils/scrollToView";

import styles from "./Text.less";

interface Props {
	className?: string;
	content?: string;
	disableAnchorsScrollHandlers?: boolean;
	disableTranslatingTex?: boolean;
	children?: React.ReactElement;
}

function Text(props: Props): React.ReactElement {
	const textContainer: RefObject<HTMLDivElement> = createRef();
	const prevProps = usePreviousProps(props);

	useEffect(() => {
		const { disableTranslatingTex = true, disableAnchorsScrollHandlers = true, content, children, } = props;
		if(!prevProps || prevProps.content !== content || prevProps.children?.key !== children?.key) {
			if(disableTranslatingTex) {
				translateTex();
			}

			if(disableAnchorsScrollHandlers) {
				addScrollHandlersToAnchors();
			}
		}
	});

	function addScrollHandlersToAnchors(): void {
		if(!textContainer.current) {
			return;
		}

		const anchors = Array.from(textContainer.current.getElementsByTagName('a'));
		//if href equal to origin + pathname + hash => <a href="#hash"> that means its navigation on slide via headers, we need to handle this via our modificated scrolling
		const hashAnchorsLinks = anchors.filter(
			a => window.location.origin + window.location.pathname + a.hash === a.href);

		const hashInUrl = window.location.hash;
		if(hashInUrl) {
			const hashToScroll = hashInUrl.replace('#', '');
			if(anchors.some(a => a.name === hashToScroll)) {
				scrollToHashAnchor(hashInUrl);
			}
		}

		for (const hashAnchor of hashAnchorsLinks) {
			console.log(hashAnchor);
			const { hash } = hashAnchor;
			hashAnchor.addEventListener('click', (e) => {
				e.stopPropagation();
				e.preventDefault();
				scrollToHashAnchor(hash);
			});
		}
	}

	function scrollToHashAnchor(hash: string): void {
		window.history.pushState(null, '', hash);

		const anchors = document.querySelectorAll(`a[name=${ hash.replace('#', '') }]`);
		if(anchors.length > 0) {
			scrollToView({ current: anchors[0] }, {
				animationDuration: 500,
				allowScrollToTop: false,
				behavior: 'smooth',
				additionalTopOffset: 50,
			});
		}
	}

	function translateTex(): void {
		if(textContainer.current) {
			translateCode(textContainer.current);
		}
	}

	function usePreviousProps(props: Props) {
		const ref = useRef<Props>();
		useEffect(() => {
			ref.current = props;
		});

		return ref.current;
	}

	const { content, className, children, } = props;
	if(content) {
		return (
			<div
				ref={ textContainer }
				className={ classNames(styles.text, className) }
				dangerouslySetInnerHTML={ { __html: content } }
			/>
		);
	}
	return (
		<div
			ref={ textContainer }
			className={ classNames(styles.text, className) }>
			{ children }
		</div>
	);
}


export default Text;
