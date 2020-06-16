import React, { useState } from "react";

import PropTypes from "prop-types";
import { BlocksWrapper } from "src/components/course/Course/Slide/Blocks";
import { Button } from "@skbkontur/react-ui";

function Spoiler({ text, blocks }) {
	const [contentVisible, showContent] = useState(0);
	const show = () => showContent(true);

	if(contentVisible) {
		return blocks;
	}

	return (
		<BlocksWrapper withoutTopPaddings>
			<Button use="success" onClick={ show }>{ text }</Button>
		</BlocksWrapper>
	);

}


Spoiler.propTypes = {
	text: PropTypes.string.isRequired,
	blocks: PropTypes.arrayOf(PropTypes.object).isRequired,
};

export default Spoiler;
