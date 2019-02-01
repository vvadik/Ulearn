import React from 'react';
import PropTypes from "prop-types";
import Hint from "@skbkontur/react-ui/components/Hint/Hint";
import SVGIcon from "../SVGIcons/SVGIcon";

function MarkdownButton(props) {
	const { text, name, width, onClick } = props;
	return (
		<Hint pos="top" text={text}>
			<button onClick={onClick}>
				<SVGIcon name={name} width={width}/>
			</button>
		</Hint>
	)
}

MarkdownButton.propTypes = {
	text: PropTypes.string,
	name: PropTypes.string,
	width: PropTypes.number,
};

export default MarkdownButton;