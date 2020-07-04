import React from "react";

import PropTypes from "prop-types";
import { BlocksWrapper } from "src/components/course/Course/Slide/Blocks";
import { Button } from "@skbkontur/react-ui";

class Spoiler extends React.Component {
	constructor(props) {
		super(props);

		this.state = {
			contentVisible: false,
		}
	}

	componentDidUpdate(prevProps, prevState, snapshot) {
		if(this.props.blocksId !== prevProps.blocksId) {
			this.setState({
				contentVisible: false,
			});
		}
	}

	showContent = () => {
		this.setState({
			contentVisible: true,
		});
	}

	render = () => {
		const { text, blocks, isHidden, isPreviousBlockHidden, } = this.props;
		const { contentVisible, } = this.state;

		if(contentVisible) {
			return this.getBlocksWithStyles(blocks);
		}

		return (
			<BlocksWrapper
				withoutTopPaddings
				isBlock={ isPreviousBlockHidden !== undefined }
				isHidden={ isHidden }
				showEyeHint={ isHidden && !isPreviousBlockHidden }
			>
				<Button use="success" onClick={ this.showContent }>{ text }</Button>
			</BlocksWrapper>
		);
	}

	getBlocksWithStyles = (blocks) => {
		const { isPreviousBlockHidden, isHidden, } = this.props;

		return blocks.map((block, i) => {
			if(i === 0 && block.type === BlocksWrapper) {
				return <BlocksWrapper
					{ ...block.props }
					showEyeHint={ isHidden && !isPreviousBlockHidden }
					isBlock={ isPreviousBlockHidden !== undefined }
					withoutTopPaddings
					key={ block.key }
				/>;
			}
			return block;
		});
	}
}


Spoiler.propTypes = {
	text: PropTypes.string.isRequired,
	blocks: PropTypes.arrayOf(PropTypes.object).isRequired,
	blocksId: PropTypes.string.isRequired,
	isPreviousBlockHidden: PropTypes.bool,
	isHidden: PropTypes.bool,
};

export default Spoiler;
