import React from "react";

import { BlocksWrapper } from "src/components/course/Course/Slide/Blocks";
import { Button } from "@skbkontur/react-ui";

const closeButtonText = "Свернуть спойлер";

interface Props {
	text: string,
	renderedBlocks: React.ReactNode[],
	blocksId: string,
	isPreviousBlockHidden: boolean,
	hide: boolean,
	closable?: boolean,
}

interface State {
	contentVisible: boolean,
}

class Spoiler extends React.Component<Props, State> {
	static defaultProps: Partial<Props> = {
		hide: false,
		isPreviousBlockHidden: false,
	};

	constructor(props: Props) {
		super(props);

		this.state = {
			contentVisible: false,
		};
	}

	componentDidUpdate(prevProps: Props): void {
		if(this.props.blocksId !== prevProps.blocksId) {
			this.setState({
				contentVisible: false,
			});
		}
	}

	showContent = (): void => {
		this.setState({
			contentVisible: true,
		});
	};

	hideContent = (): void => {
		this.setState({
			contentVisible: false,
		});
	};

	render = (): React.ReactNode => {
		const { text, renderedBlocks, hide, isPreviousBlockHidden, closable, } = this.props;
		const { contentVisible, } = this.state;

		if(contentVisible) {
			return (
				<React.Fragment>
					{ this.getBlocksWithStyles(renderedBlocks) }
					{ closable &&
					<BlocksWrapper
						withoutEyeHint={ hide && !isPreviousBlockHidden }
						withoutTopPaddings
						isBlock={ isPreviousBlockHidden !== undefined }
						hide={ hide }
					>
						<Button use="success" onClick={ this.hideContent }>{ closeButtonText }</Button>
					</BlocksWrapper>
					}
				</React.Fragment>
			);
		}

		return (
			<BlocksWrapper
				withoutEyeHint={ hide && isPreviousBlockHidden }
				withoutTopPaddings={ hide === isPreviousBlockHidden }
				isBlock={ isPreviousBlockHidden !== undefined }
				hide={ hide }
			>
				<Button use="success" onClick={ this.showContent }>{ text }</Button>
			</BlocksWrapper>
		);
	};

	getBlocksWithStyles = (blocks: React.ReactNode[]): React.ReactNode => {
		const { isPreviousBlockHidden, hide, } = this.props;
		return blocks.map((block) => {
			const element = block as React.ReactElement;

			if(typeof element.type === typeof BlocksWrapper) {
				const withoutTopPaddings = hide === isPreviousBlockHidden;

				return <BlocksWrapper
					{ ...element.props }
					withoutEyeHint={ element.props.hide && isPreviousBlockHidden }
					hide={ element.props.hide || hide }
					isBlock={ blocks.length !== 0 || isPreviousBlockHidden !== undefined }
					withoutTopPaddings={ withoutTopPaddings }
					key={ element.key }
				/>;
			}
			return block;
		});
	};
}

export default Spoiler;
