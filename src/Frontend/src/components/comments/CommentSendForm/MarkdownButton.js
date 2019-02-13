import React from 'react';
import PropTypes from "prop-types";
import Hint from "@skbkontur/react-ui/components/Hint/Hint";

function MarkdownButton(props) {
	const { hint, icon, width, height, onClick } = props;
	return (
		<Hint pos="top" text={hint}>
			<button onClick={onClick} type="button">
				<svg
					width={width}
					height={height}
					xmlns="http://www.w3.org/2000/svg"
					fill="none"
				>
					{icon}
				</svg>
			</button>
		</Hint>
	)
}

MarkdownButton.propTypes = {
	hint: PropTypes.element,
	icon: PropTypes.element,
	width: PropTypes.number,
	height: PropTypes.number
};

export default MarkdownButton;