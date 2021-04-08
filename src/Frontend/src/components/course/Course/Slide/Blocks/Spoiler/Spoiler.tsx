import React from "react";

import { BlocksWrapper, Text } from "src/components/course/Course/Slide/Blocks";
import { ArrowChevronDown, ArrowChevronUp } from "icons";

import styles from './Spoiler.less';


interface Props {
	text: string,
	renderedBlocks: React.ReactNode[],
	blocksId: string,
	isPreviousBlockHidden: boolean,
	hide: boolean,
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

	toggleContent = (): void => {
		const { contentVisible } = this.state;
		if(contentVisible) {
			this.hideContent();
		} else {
			this.showContent();
		}
	};

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
		const { text, renderedBlocks, hide, isPreviousBlockHidden, } = this.props;
		const { contentVisible, } = this.state;
		const titleClassName = contentVisible ? styles.opened : undefined;

		return (
			<>
				<BlocksWrapper
					withoutEyeHint={ hide && isPreviousBlockHidden }
					withoutTopPaddings={ hide === isPreviousBlockHidden }
					isBlock={ isPreviousBlockHidden !== undefined }
					hide={ hide }
					className={ titleClassName }
				>
					<Text disableAnchorsScrollHandlers disableTranslatingTex>
						<span onClick={ this.toggleContent } className={ styles.title }>
							{ text }
							<span className={ styles.arrow }>
								{ contentVisible
									? <ArrowChevronUp/>
									: <ArrowChevronDown/> }
							</span>
						</span>
					</Text>
				</BlocksWrapper>
				{ contentVisible && this.getBlocksWithStyles(renderedBlocks) }
			</>
		);
	};

	getBlocksWithStyles = (blocks: React.ReactNode[]): React.ReactNode => {
		const { isPreviousBlockHidden, hide, } = this.props;
		let prevBlockHidden = isPreviousBlockHidden;

		return blocks.map((block) => {
			const element = block as React.ReactElement;

			if(typeof element.type === typeof BlocksWrapper) {
				const withoutTopPaddings = element.props.hide === prevBlockHidden;
				prevBlockHidden = element.props.hide;

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
