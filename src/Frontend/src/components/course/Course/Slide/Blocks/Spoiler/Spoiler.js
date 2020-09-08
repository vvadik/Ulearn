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
		const { text, blocks, isHidden, isPreviousBlockHidden, closable, isHeaderOfHiddenSlide, } = this.props;
		const { contentVisible, } = this.state;

		if(contentVisible) {
			return (
				<React.Fragment>
					{ this.getBlocksWithStyles(blocks) }
					{ closable &&
					<BlocksWrapper
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
				withoutTopPaddings={ !isHeaderOfHiddenSlide && (isHidden ? isPreviousBlockHidden : !isPreviousBlockHidden) }
				isBlock={ isPreviousBlockHidden !== undefined }
				isHidden={ isHidden }
				showEyeHint={ isHidden && !isPreviousBlockHidden }
				isHeaderOfHiddenSlide={ isHeaderOfHiddenSlide }
			>
				<Button use="success" onClick={ this.showContent }>{ text }</Button>
			</BlocksWrapper>
		);
	}

	getBlocksWithStyles = (blocks) => {
		const { isPreviousBlockHidden, isHidden, isHeaderOfHiddenSlide, } = this.props;

		return blocks.map((block, i) => {
			if(i === 0 && block.type === BlocksWrapper) {
				const isFirstElementOnSlide = isHeaderOfHiddenSlide || isPreviousBlockHidden === undefined;
				const withoutTopPaddings = isFirstElementOnSlide ? isHeaderOfHiddenSlide : isPreviousBlockHidden;
				const blockProps = block.props;

				return <BlocksWrapper
					{ ...block.props }
					isHidden={ isHidden || blockProps.isHidden }
					showEyeHint={ (isHidden || blockProps.isHidden) && !isPreviousBlockHidden }
					isBlock={ blocks.length !== 0 || isPreviousBlockHidden !== undefined }
					isHeaderOfHiddenSlide={ isHeaderOfHiddenSlide }
					withoutTopPaddings={ !isHeaderOfHiddenSlide && withoutTopPaddings }
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
	isHeaderOfHiddenSlide: PropTypes.bool,
};

export default Spoiler;
