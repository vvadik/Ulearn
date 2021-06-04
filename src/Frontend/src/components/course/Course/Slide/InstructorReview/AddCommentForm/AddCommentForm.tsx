import React, { CSSProperties } from "react";
import { Button, Link, ScrollContainer, Hint, Toast, Gapped, } from "ui";

import { Star2, Star, Edit, Delete, } from "icons";
import { SvgIconProps } from "@skbkontur/react-icons/icons/internal/SvgIcon";

import { childOf, countLines } from "src/utils/domExtensions";

import texts from './AddCommentForm.texts';
import styles from './AddCommentForm.less';
import { MarkdownDescription } from "src/consts/comments";
import MarkdownEditor from "src/components/comments/CommentSendForm/MarkdownEditor/MarkdownEditor";
import scrollToView from "../../../../../../utils/scrollToView";

export interface ReviewComment {
	id: number;
	text: string;
	renderedText: string;
	isFavourite?: boolean | undefined;
	useCount: number;
}

interface ReviewCommentWithStyles extends ReviewComment {
	overextended?: boolean;
	ref: React.RefObject<HTMLLIElement>;
}

export interface Props {
	coordinates: { left: number; top: number; bottom: number };
	comments: ReviewComment[];
	value: string;

	onValueChange: (comment: string) => void;
	addComment: (comment: string) => void;
	addCommentToFavourite: (comment: string) => Promise<ReviewComment>;
	toggleCommentFavourite: (commentId: number) => void;
	onClose: () => void;
}

interface State {
	canBeAddedToFavourite: boolean;
	commentsTextsSet: Set<string>;
	favouriteComments: ReviewCommentWithStyles[];
	otherComments: ReviewCommentWithStyles[];
}

const markupByOperation: MarkdownDescription = {
	bold: {
		markup: "**",
		description: "жирный",
		hotkey: {
			asText: "Ctrl + B",
			ctrl: true,
			key: ["b", "и"],
		},
		icon: <svg xmlns="http://www.w3.org/2000/svg" width="6" height="8" viewBox="0 0 6 8" fill="none">
			<path fillRule="evenodd" clipRule="evenodd"
				  d="M4.51875 3.88C4.97555 3.49714 5.29578 2.86857 5.29578 2.28571C5.29578 0.994286 4.47166 0 3.41206 0H0.46875V8H3.7841C4.76834 8 5.53125 7.02857 5.53125 5.83429C5.53125 4.96571 5.12625 4.22286 4.51875 3.88ZM1.88142 1.42815H3.29421C3.68508 1.42815 4.00061 1.811 4.00061 2.28529C4.00061 2.75958 3.68508 3.14243 3.29421 3.14243H1.88142V1.42815ZM1.88142 6.57168H3.52968C3.92055 6.57168 4.23607 6.18882 4.23607 5.71454C4.23607 5.24025 3.92055 4.85739 3.52968 4.85739H1.88142V6.57168Z"
				  fill="#808080"/>
		</svg>,
	},
	italic: {
		markup: "__",
		description: "курсив",
		hotkey: {
			asText: "Ctrl + I",
			ctrl: true,
			key: ["i", "ш"],
		},
		icon: <svg xmlns="http://www.w3.org/2000/svg" width="6" height="8" viewBox="0 0 6 8" fill="none">
			<path d="M2 0V1.71429H3.105L1.395 6.28571H0V8H4V6.28571H2.895L4.605 1.71429H6V0H2Z" fill="#808080"/>
		</svg>,
	},
	code: {
		markup: "```",
		description: "блок кода",
		hotkey: {
			asText: "Alt + Q",
			alt: true,
			key: ["q", "й"],
		},
		icon: <svg xmlns="http://www.w3.org/2000/svg" width="18" height="8" viewBox="0 0 18 8" fill="none">
			<path opacity="0.6"
				  d="M7.14844 7.56934L0.763672 4.65723V3.5498L7.14844 0.494141V2.12109L2.5957 4.06934V4.11035L7.14844 5.94238V7.56934ZM17.0469 4.56836L10.6621 7.48047V5.86035L15.2285 4.03516V4.00781L10.6621 2.05273V0.425781L17.0469 3.46094V4.56836Z"
				  fill="#808080"/>
		</svg>,
	},
};

class AddCommentForm extends React.Component<Props, State> {
	private maxRowCount = 7;
	private maxCommentLineCount = 2;
	private maxCommentHintWidth = 374;
	private wrapper = React.createRef<HTMLDivElement>();

	constructor(props: Props) {
		super(props);

		const otherComments = props.comments
			.filter(c => !c.isFavourite);
		otherComments.sort((c1, c2) => c2.useCount - c1.useCount);

		const favouriteComments = props.comments
			.filter(c => c.isFavourite);
		favouriteComments.sort((c1, c2) => c2.useCount - c1.useCount);

		this.state = {
			canBeAddedToFavourite: false,
			commentsTextsSet: new Set(props.comments.map(c => c.text)),
			otherComments: otherComments.map(c => ({ ...c, ref: React.createRef() })),
			favouriteComments: favouriteComments.map(c => ({ ...c, ref: React.createRef() })),
		};
	}

	componentDidMount(): void {
		const { otherComments, favouriteComments, } = this.state;

		const overextendedOtherComments = this.markOverextendedComments(otherComments);
		const overextendedFavouriteComments = this.markOverextendedComments(favouriteComments);

		if(overextendedOtherComments) {
			this.setState({
				otherComments: overextendedOtherComments,
			});
		}
		if(overextendedFavouriteComments) {
			this.setState({
				favouriteComments: overextendedFavouriteComments,
			});
		}
		if(this.wrapper.current) {
			const rect = this.wrapper.current.getBoundingClientRect();
			if(
				rect.top <= 0 ||
				rect.bottom >= (window.innerHeight || document.documentElement.clientHeight)
			) {
				this.wrapper.current.scrollIntoView();
			}
		}
	}

	markOverextendedComments = (comments: ReviewCommentWithStyles[]): ReviewCommentWithStyles[] | null => {
		const newOtherComments = [...comments];
		let anyOverextended = false;
		for (let i = 0; i < newOtherComments.length; i++) {
			const comment = newOtherComments[i];
			if(comment.ref.current) {
				const linesCount = countLines(comment.ref.current);
				if(linesCount > this.maxCommentLineCount) {
					newOtherComments[i] = { ...comment, overextended: true };
					anyOverextended = true;
				}
			}
		}

		return anyOverextended
			? newOtherComments
			: null;
	};

	render = (): React.ReactElement => {
		const { coordinates, onClose, value, } = this.props;
		const { otherComments, favouriteComments, canBeAddedToFavourite, } = this.state;

		const style: CSSProperties = {
			top: coordinates.top,
			left: 30,
		};

		return (
			<div ref={ this.wrapper } className={ styles.wrapper } style={ style }>
				<Delete className={ styles.closeFormButton } onClick={ onClose }/>
				{ this.renderTextareaSection(value, canBeAddedToFavourite,) }
				<div className={ styles.divider }/>
				{ this.renderCommentSection(favouriteComments, otherComments) }
			</div>);
	};

	renderTextareaSection = (comment: string, canBeAddedToFavourite: boolean,): React.ReactElement => (
		<div className={ styles.addCommentSection }>
			<span className={ styles.commentsHeader }>
				{ texts.commentSectionHeaderText }
			</span>
			<MarkdownEditor
				className={ styles.addCommentTextArea }
				rows={ this.maxRowCount }
				maxRows={ this.maxRowCount }
				text={ comment }
				hasError={ false }
				isShowFocus={ false }
				handleChange={ this.onValueChange }
				handleSubmit={ this.onAddComment }
				hideDescription
				hidePlaceholder
				markupByOperation={ markupByOperation }>
				{ this.renderControls(this.removeWhiteSpaces(comment).length === 0, canBeAddedToFavourite,) }
			</MarkdownEditor>
		</div>
	);

	renderControls = (sendButtonDisabled: boolean, canBeAddedToFavourite: boolean): React.ReactElement => (
		<div className={ styles.controlsWrapper }>
			<Gapped vertical={ false } gap={ 10 }>
				<Button disabled={ sendButtonDisabled }
						use={ "primary" }
						onClick={ this.onAddComment }>
					{ texts.addCommentButtonText }
				</Button>
				<Hint pos={ "top left" } text={ canBeAddedToFavourite && texts.addToFavouriteButtonText }>
					<Button disabled={ !canBeAddedToFavourite } use={ "primary" } onClick={ this.onAddToFavourite }>
						<Star/>
					</Button>
				</Hint>
			</Gapped>
		</div>

	);

	renderCommentSection = (favouriteComments: ReviewCommentWithStyles[],
		otherComments: ReviewCommentWithStyles[]
	): React.ReactElement => (
		<div className={ styles.favouriteSection }>
			{ this.renderCommentsHeader() }
			<ScrollContainer className={ styles.commentsScrollWrapper }>
				<div className={ styles.commentsWrapper }>
					{ this.renderFavouriteComments(favouriteComments) }
					{ this.renderOtherComments(otherComments) }
				</div>
			</ScrollContainer>
		</div>);

	renderCommentsHeader = (): React.ReactElement => (
		<header className={ styles.header }>
			{ texts.favouriteSectionHeaderText }
			{/*			<Link className={ styles.editFavouritesComments } TODO frozen due to lack of scenarios of using
				  onClick={ () => console.log("Open editing modal") }>
				<Edit/>
			</Link>*/ }
		</header>
	);

	renderFavouriteComments = (favouriteComments: ReviewCommentWithStyles[]): React.ReactElement => (
		favouriteComments.length > 0
			? <ul className={ styles.commentsList }>
				{ favouriteComments.map(this.mapCommentToRendered) }
			</ul>
			: <span className={ styles.noFavouriteCommentsText }>
				{ texts.noFavouriteCommentsText() }
			</span>);

	renderOtherComments = (otherComments: ReviewCommentWithStyles[]): React.ReactElement => (
		<>
			<header className={ styles.header }>
				{ texts.instructorFavouriteSectionHeaderText }
			</header>
			<ul className={ styles.commentsList }>
				{ otherComments.map(this.mapCommentToRendered) }
			</ul>
		</>
	);

	mapCommentToRendered = (c: ReviewCommentWithStyles): React.ReactElement => {
		const Icon = c.isFavourite
			? (props: SvgIconProps) => <Star { ...props }/>
			: (props: SvgIconProps) => <Star2 { ...props }/>;
		const id = c.id.toString();

		if(c.overextended) {
			return (
				<li key={ id } ref={ c.ref }>
					<Icon
						id={ id }
						className={ c.isFavourite ? styles.favouriteIcon : styles.notSelectedFavouriteIcon }
						onClick={ this.onToggleClick }/>
					<Hint pos={ "top left" } maxWidth={ this.maxCommentHintWidth }
						  text={ <span dangerouslySetInnerHTML={ { __html: c.renderedText } }/> }>
						<span
							className={ styles.commentTextElapsed }
							id={ id }
							onClick={ this.onCommentClick }
							dangerouslySetInnerHTML={ { __html: c.renderedText } }
						/>
					</Hint>
				</li>);
		}

		return (
			<li key={ id } ref={ c.ref }>
				<Icon
					id={ id }
					className={ c.isFavourite ? styles.favouriteIcon : styles.notSelectedFavouriteIcon }
					onClick={ this.onToggleClick }/>
				<span
					id={ id }
					onClick={ this.onCommentClick }
					dangerouslySetInnerHTML={ { __html: c.renderedText } }
				/>
			</li>);
	};

	onValueChange = (value: string): void => {
		const trimmed = this.removeWhiteSpaces(value);
		this.props.onValueChange(value);
		this.setState({
			canBeAddedToFavourite: trimmed.length > 0 && !this.isInFavourite(trimmed),
		});
	};

	onAddComment = (): void => {
		const { addComment, value, } = this.props;

		addComment(value);
	};

	onAddToFavourite = (): void => {
		const { favouriteComments, commentsTextsSet, } = this.state;
		const { addCommentToFavourite, value, } = this.props;
		const trimmedText = this.removeWhiteSpaces(value);

		addCommentToFavourite(trimmedText)
			.then(c => {
				Toast.push('Комментарий добавлен в избранное');
				const newFavouriteComments = [
					...favouriteComments, {
						...c,
						ref: React.createRef(),
					} as ReviewCommentWithStyles
				];

				this.setState({
					favouriteComments: newFavouriteComments,
					canBeAddedToFavourite: false,
					commentsTextsSet: new Set([...commentsTextsSet, c.text]),
				}, () => {
					const overextendedFavouriteComments = this.markOverextendedComments(newFavouriteComments);
					if(overextendedFavouriteComments) {
						this.setState({
							favouriteComments: overextendedFavouriteComments,
						});
					}
				});
			});
	};

	onToggleClick = (event: React.MouseEvent): void => {
		const { toggleCommentFavourite, } = this.props;
		const { otherComments, favouriteComments, } = this.state;

		const id = Number.parseInt(event.currentTarget.id);
		toggleCommentFavourite(id);

		const indexInFavourite = favouriteComments.findIndex(c => c.id === id);
		const indexInOthers = otherComments.findIndex(c => c.id === id);

		if(indexInFavourite > -1) {
			const comments = [...favouriteComments];
			const comment = comments[indexInFavourite];
			comments[indexInFavourite] = { ...comment, isFavourite: !comment.isFavourite };
			this.setState({
				favouriteComments: comments,
			});
		} else {
			const comments = [...otherComments];
			const comment = comments[indexInOthers];
			comments[indexInOthers] = { ...comment, isFavourite: !comment.isFavourite };
			this.setState({
				otherComments: comments,
			});
		}
	};

	onCommentClick = (event: React.MouseEvent): void => {
		const { comments, onValueChange, } = this.props;
		const id = Number.parseInt(event.currentTarget.id);
		const commentText = comments.find(c => c.id === id)?.text;

		onValueChange(commentText || '');
		this.setState({
			canBeAddedToFavourite: false,
		});
	};

	isInFavourite = (trimmedText: string): boolean => {
		const { commentsTextsSet, } = this.state;
		return commentsTextsSet.has(trimmedText);
	};

	removeWhiteSpaces = (commentText: string): string => {
		//do not replace spaces in text to avoid scenario with multi line code //
		// .replace(/\s+/g, ' ');
		return commentText.trim();
	};
}

export default AddCommentForm;
