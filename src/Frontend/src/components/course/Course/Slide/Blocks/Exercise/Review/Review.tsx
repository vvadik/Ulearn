import React from "react";
import cn from "classnames";

import Avatar from "src/components/common/Avatar/Avatar";
import { DropdownMenu, MenuItem, MenuSeparator, Textarea, ThemeContext, Button, Gapped, } from "ui";
import { Send3, Trash, MenuKebab, Star2, Edit, Star, } from "icons";
import { getDataFromReviewToCompareChanges } from "../../../InstructorReview/utils";

import { textareaHidden } from "src/uiTheme";

import { isInstructor, UserInfo, } from "src/utils/courseRoles";
import { clone } from "src/utils/jsonExtensions";

import { ReviewCommentResponse, ReviewInfo } from "src/models/exercise";
import { Editor } from "codemirror";

import styles from "./Review.less";
import texts from "./Review.texts";


interface CommentReplies {
	[id: number]: string;
}

interface RenderedReview {
	margin: number;
	review: ReviewInfo;
	ref: React.RefObject<HTMLLIElement>;
}

interface ReviewState {
	renderedReviews: RenderedReview[];
	replies: CommentReplies;
	marginsAdded: boolean;

	editingParentReviewId?: number;
	editingReviewId?: number;
	editingCommentValue?: string;
}

interface ReviewProps {
	editor: Editor | null;
	reviews: ReviewInfo[];
	selectedReviewId: number;
	user?: UserInfo;

	isReviewCanBeAdded: (reviewText: string) => boolean;
	onReviewClick: (e: React.MouseEvent | React.FocusEvent, id: number,) => void;
	addReviewComment: (reviewId: number, comment: string) => void;
	deleteReviewOrComment: (id: number, reviewId?: number) => void;
	editReviewOrComment: (text: string, id: number, reviewId?: number) => void;

	backgroundColor?: 'orange' | 'gray';

	toggleReviewFavourite?: (id: number,) => void;
	isReviewFavourite?: (reviewText: string) => boolean;
	isReviewOutdated?: (reviewId: number) => boolean;
}

class Review extends React.Component<ReviewProps, ReviewState> {
	private botUser = { visibleName: 'Ulearn bot', id: 'bot', };

	constructor(props: ReviewProps) {
		super(props);

		this.state = {
			renderedReviews: this.getCommentsOrderByStart(clone(props.reviews))
				.map(r => ({
					margin: 0,
					review: r,
					ref: React.createRef<HTMLLIElement>(),
				})),
			replies: this.buildCommentsReplies(props.reviews),
			marginsAdded: false,
		};
	}

	getCommentsOrderByStart = (reviews: ReviewInfo[]): ReviewInfo[] => {
		return reviews.sort((r1, r2) => {
			if(r1.startLine < r2.startLine || (r1.startLine === r2.startLine && r1.startPosition < r2.startPosition)) {
				return -1;
			}
			if(r2.startLine < r1.startLine || (r2.startLine === r1.startLine && r2.startPosition < r1.startPosition)) {
				return 1;
			}
			return 0;
		});
	};

	buildCommentsReplies = (reviews: ReviewInfo[], oldReplies?: CommentReplies): CommentReplies => {
		const newCommentReplies: CommentReplies = {};

		for (const { id } of reviews) {
			newCommentReplies[id] = oldReplies?.[id] || '';
		}

		return newCommentReplies;
	};

	componentDidMount(): void {
		//add margins in a moment after mounting, so code could resize correctly to calculate correct topAnchor
		this.setState({}, this.addMarginsToComments);
	}

	componentDidUpdate(prevProps: ReviewProps): void {
		const { reviews, selectedReviewId, } = this.props;
		const { replies, marginsAdded, } = this.state;

		const sameReviews = JSON.stringify(this.getCommentsOrderByStart(reviews).map(getDataFromReviewToCompareChanges))
			=== JSON.stringify( this.getCommentsOrderByStart(prevProps.reviews).map(getDataFromReviewToCompareChanges));

		if(!sameReviews) {
			this.setState({
				renderedReviews: this.getCommentsOrderByStart(reviews)
					.map(r => ({
						margin: 0,
						review: r,
						ref: React.createRef<HTMLLIElement>(),
					})),
				replies: this.buildCommentsReplies(reviews, replies),
				marginsAdded: false,
			}, this.addMarginsToComments);
		} else if(!marginsAdded
			|| (selectedReviewId !== prevProps.selectedReviewId && selectedReviewId >= 0)) {
			this.addMarginsToComments();
		}
	}

	addMarginsToComments = (): void => {
		const { renderedReviews, } = this.state;
		const { selectedReviewId, editor, } = this.props;

		const commentsWithMargin = [...renderedReviews];
		const selectedReviewIndex = commentsWithMargin.findIndex(c => c.review.id === selectedReviewId);
		let lastReviewBottomHeight = 0;

		if(selectedReviewIndex >= 0) {
			const selectedComment = commentsWithMargin[selectedReviewIndex];
			const anchorTop = this.getReviewAnchorTop(selectedComment.review, editor);
			const height = selectedComment.ref.current?.offsetHeight || 0;
			const offset = Math.max(5, anchorTop);

			let spaceToSelectedReview = offset;
			selectedComment.margin = offset;
			lastReviewBottomHeight = offset + height;

			if(selectedReviewIndex > 0) {
				let totalCommentsHeight = 5;
				for (let i = 0; i < selectedReviewIndex; i++) {
					const comment = commentsWithMargin[i];
					const height = comment.ref.current?.offsetHeight || 0;
					totalCommentsHeight += height + 5;
				}

				for (let i = 0; i <= selectedReviewIndex; i++) {
					const comment = commentsWithMargin[i];
					const anchorTop = this.getReviewAnchorTop(comment.review, editor);
					const height = comment.ref.current?.offsetHeight || 0;
					comment.margin = Math.min(anchorTop, spaceToSelectedReview - totalCommentsHeight);
					if(i > 0) {
						comment.margin = Math.max(5, comment.margin);
					}
					spaceToSelectedReview -= (height + comment.margin);
					totalCommentsHeight -= (height);
				}
			}
		}

		for (let i = selectedReviewIndex + 1; i < commentsWithMargin.length; i++) {
			const comment = commentsWithMargin[i];
			const anchorTop = this.getReviewAnchorTop(comment.review, editor);
			const height = comment.ref.current?.offsetHeight || 0;
			const offset = Math.max(5, anchorTop - lastReviewBottomHeight);

			comment.margin = offset;
			lastReviewBottomHeight += offset + height;
		}

		this.setState({
			renderedReviews: commentsWithMargin,
		}, () => {
			this.setState({
				marginsAdded: true,
			});
		});
	};

	render = (): React.ReactNode => {
		const { renderedReviews, } = this.state;
		const { backgroundColor = 'orange', } = this.props;

		return (
			<ol className={ cn(styles.reviewsContainer,
				backgroundColor === 'orange'
					? styles.reviewOrange
					: styles.reviewGray) }
			>
				{ renderedReviews.map(this.renderTopLevelComment) }
			</ol>
		);
	};

	renderTopLevelComment = ({ review, margin, ref, }: RenderedReview, i: number,): React.ReactNode => {
		const { id, comments, } = review;
		const { selectedReviewId, isReviewOutdated, } = this.props;
		const { replies, marginsAdded, editingReviewId, } = this.state;

		const outdated = isReviewOutdated && isReviewOutdated(id) || false;

		const className = cn(
			styles.comment,
			{ [styles.outdatedComment]: outdated },
			{ [styles.commentMounted]: marginsAdded },
			{ [styles.selectedReviewCommentWrapper]: selectedReviewId === id },
		);

		const authorToRender = review.author ?? this.botUser;

		return (
			<li key={ i }
				className={ className }
				ref={ ref }
				data-tid={ id }
				onClick={ this.selectComment }
				style={ {
					marginTop: `${ margin }px`,
				} }
			>
				{ this.renderComment(review) }
				{
					comments.length > 0 &&
					<ol className={ styles.commentRepliesList }>
						{ comments.map((c, i) =>
							<li className={ styles.commentReply } key={ i }>
								{ this.renderComment(c, id, outdated) }
							</li>)
						}
					</ol>
				}
				{ editingReviewId !== id
				&& comments.every(c => c.id !== editingReviewId)
				&& selectedReviewId === id
				&& !outdated
				&& (authorToRender.id !== this.botUser.id || comments.length > 0)
				&& this.renderAddReviewComment(replies[id], id) }
			</li>
		);
	};

	renderComment(review: ReviewInfo): React.ReactNode;
	renderComment(reviewComment: ReviewCommentResponse, reviewId: number, reviewOutdated?: boolean): React.ReactNode;
	renderComment(
		review: ReviewInfo & ReviewCommentResponse,
		reviewId: number | null = null,
		reviewOutdated = undefined,
	): React.ReactNode {
		const {
			author,
			addingTime,
			publishTime,
			renderedText,
			text,
			comment,
			renderedComment,
			id,
		} = review;
		const {
			editingReviewId,
			editingParentReviewId,
			editingCommentValue,
		} = this.state;
		const {
			isReviewOutdated,
		} = this.props;
		const authorToRender = author ?? this.botUser;

		const time = addingTime || publishTime;
		const content = text || comment;
		const outdated = reviewOutdated ?? (isReviewOutdated && isReviewOutdated(id) || false);

		return (
			<React.Fragment>
				<div className={ styles.authorWrapper }>
					<Avatar user={ authorToRender } size={ "big" } className={ styles.commentAvatar }/>
					<div className={ styles.commentInfoWrapper }>
						<span className={ styles.commentInfo }>
							<span className={ styles.authorName }>
								{ authorToRender.visibleName }
							</span>
							{ this.renderCommentControls(review, reviewId, outdated) }
						</span>
						{ time &&
						<p className={ styles.commentAddingTime }>{ texts.getAddingTime(time) }</p>
						}
					</div>
				</div>
				{ editingReviewId !== id && editingParentReviewId !== reviewId
					? <p
						className={ styles.commentText }
						dangerouslySetInnerHTML={ { __html: renderedText ?? renderedComment } }
					/>
					: <Gapped gap={ 12 } vertical className={ styles.commentEditTextArea }>
						<Textarea
							autoFocus
							width={ 210 } //if this number < 250, then this number should be equal to .commentEditTextArea min-width
							rows={ 2 }
							autoResize
							placeholder={ texts.sendButton }
							onValueChange={ this.onEditingTextareaValueChange }
							value={ editingCommentValue }
						/>
						<Gapped gap={ 14 }>
							<Button
								use={ 'primary' }
								disabled={ content === editingCommentValue }
								onClick={ this.saveReviewOrComment }>
								{ texts.editing.save }
							</Button>
							<Button
								onClick={ this.stopEditingComment }>
								{ texts.editing.cancel }
							</Button>
						</Gapped>
					</Gapped> }
			</React.Fragment>
		);
	}

	renderCommentControls = ({
			id,
			author,
			comment,
			text,
		}: ReviewInfo & ReviewCommentResponse,
		reviewId: number | null = null,
		reviewOutdated = false,
	): React.ReactNode => {
		const {
			user,
			selectedReviewId,
			toggleReviewFavourite,
			isReviewFavourite,
		} = this.props;
		const authorToRender = author ?? this.botUser;
		const shouldRenderControls = authorToRender.id === user?.id
			&& !reviewOutdated
			&& (selectedReviewId === id || selectedReviewId === reviewId);
		if(!shouldRenderControls) {
			return null;
		}

		const actionsMarkup: React.ReactElement[] = [];
		if(toggleReviewFavourite && isReviewFavourite && selectedReviewId === id && !reviewId) {
			actionsMarkup.push(
				<MenuItem
					data-id={ id }
					onClick={ this.toggleReviewToFavourite }
					key={ 'favourite' }>
					{ texts.getToggleFavouriteMarkup(isReviewFavourite(text || comment)) }
				</MenuItem>,
				<MenuSeparator key={ 'separator' }/>
			);
		}
		if(authorToRender.id === user?.id) {
			actionsMarkup.push(
				<MenuItem
					key={ 'edit' }
					onClick={ this.startEditingComment }
					data-id={ id }
					data-reviewid={ reviewId }
					data-text={ text ?? comment }
				>
					{ texts.editButton }
				</MenuItem>);
		}
		if(authorToRender.id === user?.id || user && isInstructor(user)) {
			actionsMarkup.push(
				<MenuItem
					key={ 'delete' }
					data-id={ id }
					data-reviewid={ reviewId }
					onClick={ this.deleteReviewOrComment }>
					{ texts.deleteButton }
				</MenuItem>
			);
		}

		return <DropdownMenu
			className={ styles.kebabMenu }
			caption={ <MenuKebab
				className={ styles.kebabMenuIcon }
				size={ 18 }
			/> }
			positions={ ['left top'] }
			menuWidth={ 216 }
		>
			{ actionsMarkup }
		</DropdownMenu>;
	};

	startEditingComment = (event: React.MouseEvent | React.SyntheticEvent): void => {
		const { id, reviewId, } = this.parseCommentData(event);
		const text = (event.currentTarget as HTMLElement).dataset.text;

		this.setState({
			editingReviewId: id,
			editingParentReviewId: reviewId,
			editingCommentValue: text,
		});
	};

	parseCommentData = (event: React.MouseEvent | React.SyntheticEvent): { id: number, reviewId?: number, } => {
		const { id, reviewid, } = (event.currentTarget as HTMLElement).dataset;

		return {
			id: parseInt(id || '-1'),
			reviewId: reviewid ? parseInt(reviewid) : undefined,
		};
	};

	toggleReviewToFavourite = (event: React.MouseEvent | React.SyntheticEvent): void => {
		const { toggleReviewFavourite, } = this.props;
		const { id, } = this.parseCommentData(event);

		if(!toggleReviewFavourite) {
			return;
		}

		toggleReviewFavourite(id);
		event.preventDefault();
		event.stopPropagation();
	};

	deleteReviewOrComment = (event: React.MouseEvent | React.SyntheticEvent): void => {
		const { deleteReviewOrComment, } = this.props;
		const { id, reviewId } = this.parseCommentData(event);

		if(!deleteReviewOrComment) {
			return;
		}

		if(reviewId) {
			deleteReviewOrComment(reviewId, id);
		} else {
			deleteReviewOrComment(id);
		}
	};

	renderAddReviewComment = (
		commentReply: string,
		id: number,
	): React.ReactNode => {
		const isReviewCanBeAdded = this.props.isReviewCanBeAdded(commentReply);
		return (
			<ThemeContext.Provider value={ textareaHidden }>
				<div className={ styles.commentReplyTextArea }>
					<Textarea
						width={ 230 }//fix of overlapping send button
						rows={ 1 }
						autoResize
						placeholder={ texts.sendButton }
						onValueChange={ this.onTextareaValueChange }
						data-tid={ id.toString() }
						onFocus={ this.selectComment }
						value={ commentReply }
					/>
				</div>
				<button
					disabled={ !isReviewCanBeAdded }
					className={ isReviewCanBeAdded ? styles.commentReplyButtonActive : styles.commentReplyButton }
					onClick={ this.sendComment }
					data-tid={ id }
					onFocus={ this.selectComment }>
					<Send3/>
				</button>
			</ThemeContext.Provider>
		);
	};

	selectComment = (e: React.MouseEvent | React.FocusEvent): void => {
		const { selectedReviewId, } = this.props;
		const element = e.currentTarget as HTMLElement;
		const id = parseInt(element.dataset.tid
			|| element.parentElement?.parentElement?.parentElement?.dataset.tid
			|| '-1');
		if(selectedReviewId !== id) {
			this.props.onReviewClick(e, id);
		}
	};

	onTextareaValueChange = (value: string): void => {
		const { replies, } = this.state;
		const { selectedReviewId, } = this.props;

		const newCommentsReplies = { ...replies };

		newCommentsReplies[selectedReviewId] = value;

		this.setState({
			replies: newCommentsReplies,
		});
	};

	onEditingTextareaValueChange = (value: string): void => {
		this.setState({
			editingCommentValue: value,
		});
	};

	saveReviewOrComment = (): void => {
		const {
			editReviewOrComment,
		} = this.props;
		const {
			editingReviewId,
			editingCommentValue,
			editingParentReviewId,
		} = this.state;


		if(!editingCommentValue || editingReviewId === undefined) {
			return;
		}

		editReviewOrComment(editingCommentValue, editingReviewId, editingParentReviewId,);

		this.stopEditingComment();
	};

	stopEditingComment = (): void => {
		this.setState({
			editingCommentValue: undefined,
			editingReviewId: undefined,
			editingParentReviewId: undefined,
		});
	};

	sendComment = (): void => {
		const { selectedReviewId, addReviewComment, } = this.props;
		const { replies, } = this.state;

		addReviewComment(selectedReviewId, replies[selectedReviewId]);
		this.setState({
			replies: { ...replies, [selectedReviewId]: '' },
		});
	};

	getReviewAnchorTop = (review: ReviewInfo, editor: Editor | null,): number => {
		if(!editor) {
			return -1;
		}

		return editor.charCoords({
			line: review.startLine,
			ch: review.startPosition,
		}, 'local').top;
	};
}

export { Review, ReviewProps };
