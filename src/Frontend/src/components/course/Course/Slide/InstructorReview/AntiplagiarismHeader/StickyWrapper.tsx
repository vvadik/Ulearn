import React from "react";
import { Sticky } from "ui";

interface StickyWrapper {
	stickerClass: string;

	renderSticker: (fixed: boolean) => React.ReactElement<StickerProps>;
	renderContent: () => React.ReactNode;
}

export interface StickerProps {
	fixed: boolean;
}

export default function StickyWrapper({ renderSticker, stickerClass, renderContent, }: StickyWrapper): React.ReactElement {
	const ref = React.useRef<HTMLDivElement>(null);

	return (
		<>
			<Sticky getStop={ getStopper } side={ "top" }>
				{ fixed => renderSticker(fixed) }
			</Sticky>
			{ renderContent() }
			<div ref={ ref } className={ stickerClass }/>
		</>
	);

	function getStopper() {
		return ref.current;
	}
}
