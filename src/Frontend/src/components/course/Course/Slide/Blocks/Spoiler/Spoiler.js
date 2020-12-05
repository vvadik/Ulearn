import React from "react";

import PropTypes from "prop-types";
import { BlocksWrapper } from "src/components/course/Course/Slide/Blocks";
import { Button } from "@skbkontur/react-ui";

const closeButtonText = "Свернуть спойлер";

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

	hideContent = () => {
		this.setState({
			contentVisible: false,
		});
	}

	render = () => {
		const { text, blocks, isHidden, isPreviousBlockHidden, closable, } = this.props;
		const { contentVisible, } = this.state;

		if(contentVisible) {
			return (
				<React.Fragment>
					{ this.getBlocksWithStyles(blocks) }
					{ closable &&
					<BlocksWrapper
						withoutEyeHint={ isHidden === blocks[blocks.length - 1].props.isHidden }
						withoutTopPaddings
						isBlock={ isPreviousBlockHidden !== undefined }
						isHidden={ isHidden }
					>
						<Button use="success" onClick={ this.hideContent }>{ closeButtonText }</Button>
					</BlocksWrapper>
					}
				</React.Fragment>
			)
		}

		return (
			<BlocksWrapper
				withoutEyeHint={ isHidden && isPreviousBlockHidden }
				withoutTopPaddings={ isHidden === isPreviousBlockHidden }
				isBlock={ isPreviousBlockHidden !== undefined }
				isHidden={ isHidden }
			>
				<Button use="success" onClick={ this.showContent }>{ text }</Button>
			</BlocksWrapper>
		);
	}

	getBlocksWithStyles = (blocks) => {
		const { isPreviousBlockHidden, isHidden, } = this.props;

		return blocks.map((block, i) => {
			if(i === 0 && block.type === BlocksWrapper) {
				const withoutTopPaddings = isHidden === isPreviousBlockHidden;
				const blockProps = block.props;

				return <BlocksWrapper
					{ ...block.props }
					withoutEyeHint={ isHidden && isPreviousBlockHidden }
					isHidden={ isHidden || blockProps.isHidden }
					isBlock={ blocks.length !== 0 || isPreviousBlockHidden !== undefined }
					withoutTopPaddings={ withoutTopPaddings }
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
	closable: PropTypes.bool,
};

Spoiler.defaultProps = {
	isHidden: false,
	isPreviousBlockHidden: false,
}

export default Spoiler;
