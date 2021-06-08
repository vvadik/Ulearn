import React from "react";
import { Sticky } from "ui";

interface StickyWrapper {
	stickerClass: string;

	sticker: (fixed: boolean) => React.ReactElement<StickerProps>;
	content: React.ReactNode;
}

export interface StickerProps {
	fixed: boolean;
}

export default function StickyWrapper({ sticker, stickerClass, content, }: StickyWrapper): React.ReactElement {
	const ref = React.useRef<HTMLDivElement>(null);

	return (
		<>
			<Sticky getStop={ getStopper } side={ "top" }>
				{ fixed => sticker(fixed) }
			</Sticky>
			{ content }
			<div ref={ ref } className={ stickerClass }/>
		</>
	);

	function getStopper() {
		return ref.current;
	}
}
