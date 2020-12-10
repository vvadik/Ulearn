import React, { useEffect, useState } from "react";

import styles from './showAfterDelay.less';

interface Props {
	timeoutInMs: number,
	children: React.ReactNode,
}

//This component allow show kontur react modal with delay
const ShowAfterDelay: React.FC<Props> | React.ReactNode = ({ timeoutInMs = 300, children }) => {
	const [timeoutPassed, callTimeout] = useState(false);
	const wrapper: React.Ref<HTMLDivElement> = React.createRef<HTMLDivElement>();

	useEffect(() => {
		if(wrapper?.current?.children[0]) {
			getContainer(wrapper.current)?.classList.add(styles.invisible);
		}
		setTimeout(() => {
			callTimeout(true);
			if(wrapper?.current?.children[0]) {
				getContainer(wrapper.current)?.classList.remove(styles.invisible);
			}
		}, timeoutInMs);
	});

	return (
		<div className={ timeoutPassed ? undefined : styles.invisible } ref={ wrapper }>
			{ children }
		</div>
	);

	function getContainer(wrapper: HTMLDivElement): Element | null {
		if(wrapper.children[0] instanceof HTMLElement) {
			const { renderContainerId } = wrapper.children[0].dataset;
			return document.querySelector(`[data-rendered-container-id="${ renderContainerId }"]`);
		}

		return null;
	}
};

export default ShowAfterDelay;
