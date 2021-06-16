import React from "react";
import classNames from "classnames";

import Avatar from "src/components/common/Avatar/Avatar";
import { Textarea, ThemeContext, } from "ui";
import { Send3, Trash, MenuKebab, } from "icons";

import { textareaHidden } from "src/uiTheme";

import { ReviewCommentResponse, ReviewInfo } from "src/models/exercise";

import styles from "./Review.less";
import texts from "./Review.texts";
import { Editor } from "codemirror";
import cn from "classnames";


interface CommentReplies {
	[id: number]: string;
}

export interface InstructorReviewInfo extends ReviewInfo {
	outdated?: boolean;
}

interface StateComment {
	margin: number;
	review: InstructorReviewInfo;
	ref: React.RefObject<HTMLLIElement>;
}

interface ReviewState {
	comments: StateComment[];
	commentsReplies: CommentReplies;
	marginsAdded: boolean;
}

interface ReviewProps {
	editor: Editor | null;
	reviews: InstructorReviewInfo[];
	selectedReviewId: number;
	userId?: string | null;

	reviewBackgroundColor?: 'orange' | 'gray';
	disableLineNumbers?: boolean;

	onSelectComment: (e: React.MouseEvent | React.FocusEvent, id: number,) => void;
	addReviewComment: (reviewId: number, comment: string) => void;
	deleteReviewOrComment: (reviewId: number, commentId?: number) => void;
}

const botUser = { visibleName: 'Ulearn bot', id: 'bot', };

class Review extends React.Component<ReviewProps, ReviewState> {
	constructor(props: ReviewProps) {
		super(props);

		this.state = {
			comments: this.getCommentsOrderByStart([...props.reviews])
				.map(r => ({
					margin: 0,
					review: r,
					ref: React.createRef<HTMLLIElement>(),
				})),
			commentsReplies: this.buildCommentsReplies(props.reviews),
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

		let sameReviews = reviews.length === prevProps.reviews.length;
		if(sameReviews) {
			const newReviews = reviews.map(r => ({
				id: r.id,
				innerCommentsIds: r.comments.map(c => c.id),
			}));
			const oldReviews = prevProps.reviews.map(r => ({
				id: r.id,
				innerCommentsIds: r.comments.map(c => c.id),
			}));

			for (const { id, innerCommentsIds } of newReviews.values()) {
				const oldReview = oldReviews.find(or => or.id === id);
				if(!oldReview) {
					sameReviews = false;
					break;
				}

				if(oldReview.id !== id || innerCommentsIds.length !== oldReview.innerCommentsIds.length) {
					sameReviews = false;
					break;
				}
				if(innerCommentsIds[innerCommentsIds.length - 1] !== oldReview.innerCommentsIds[oldReview.innerCommentsIds.length - 1]) {
					sameReviews = false;
					break;
				}
			}
		}
		if(!sameReviews) {
			this.setState({
				comments: this.getCommentsOrderByStart(reviews)
					.map(r => ({
						margin: 0,
						review: r,
						ref: React.createRef<HTMLLIElement>(),
					})),
				commentsReplies: this.buildCommentsReplies(reviews, this.state.commentsReplies),
				marginsAdded: false,
			});
		} else if(!this.state.marginsAdded
			|| (selectedReviewId !== prevProps.selectedReviewId && selectedReviewId >= 0)) {
			this.addMarginsToComments();
		}
	}

	addMarginsToComments = (): void => {
		const { comments, } = this.state;
		const { selectedReviewId, editor, } = this.props;

		const commentsWithMargin = [...comments];
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
			comments: commentsWithMargin,
		}, () => {
			this.setState({
				marginsAdded: true,
			});
		});
	};

	render = (): React.ReactNode => {
		const { comments, } = this.state;
		const { reviewBackgroundColor = 'orange', } = this.props;

		return (
			<ol className={ cn(styles.reviewsContainer,
				reviewBackgroundColor === 'orange' ? styles.reviewOrange : styles.reviewGray) }>
				{ comments.map(this.renderTopLevelComment) }
			</ol>
		);
	};

	renderTopLevelComment = ({ review, margin, ref, }: StateComment, i: number,): React.ReactNode => {
		const { id, comments, } = review;
		const { selectedReviewId, onSelectComment, } = this.props;
		const { commentsReplies, marginsAdded, } = this.state;
		const className = classNames(
			styles.comment,
			{ [styles.outdatedComment]: review.outdated },
			{ [styles.commentMounted]: marginsAdded },
			{ [styles.selectedReviewCommentWrapper]: selectedReviewId === id },
		);

		const authorToRender = review.author ?? botUser;

		const selectComment = (e: React.MouseEvent | React.FocusEvent) => onSelectComment(e, id);
		return (
			<li key={ i }
				className={ className }
				ref={ ref }
				onClick={ selectComment }
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
								{ this.renderComment(c, id, review.outdated) }
							</li>)
						}
					</ol>
				}
				{ selectedReviewId === id && !review.outdated && (authorToRender.id !== botUser.id || comments.length > 0)
				&& this.renderAddReviewComment(selectComment, commentsReplies[id]) }
			</li>
		);
	};

	renderComment(review: InstructorReviewInfo): React.ReactNode;
	renderComment(reviewComment: ReviewCommentResponse, reviewId: number, reviewOutdated?: boolean): React.ReactNode;
	renderComment({
			id,
			author,
			startLine,
			finishLine,
			addingTime,
			publishTime,
			renderedText,
			renderedComment,
			outdated,
		}: InstructorReviewInfo & ReviewCommentResponse,
		reviewId: number | null = null,
		reviewOutdated = false,
	): React.ReactNode {
		const { userId, disableLineNumbers, selectedReviewId, } = this.props;
		const authorToRender = author ?? botUser;

		const time = addingTime || publishTime;

		return (
			<React.Fragment>
				<div className={ styles.authorWrapper }>
					<Avatar user={ authorToRender } size="big" className={ styles.commentAvatar }/>
					<div className={ styles.commentInfoWrapper }>
						<span className={ styles.commentInfo }>
							<span className={ styles.authorName }>
								{ authorToRender.visibleName }
							</span>
							{ !disableLineNumbers && startLine >= 0 &&
							<span className={ styles.commentLineNumber }>
								{ texts.getLineCapture(startLine, finishLine) }
							</span>
							}
							{

								authorToRender.id === userId && !outdated && !reviewOutdated && (
									!reviewId
										? <MenuKebab
											data-id={ id }
											className={ styles.kebabMenu }
											//onClick={ this.deleteReviewOrComment }
											size={ 16 }
										/>
										: <Trash
											data-id={ id }
											data-reviewid={ reviewId }
											className={ styles.innerCommentDeleteButton }
											onClick={ this.deleteReviewOrComment }
											size={ 12 }
										/>
								)
							}
						</span>
						{ time &&
						<p className={ styles.commentAddingTime }>{ texts.getAddingTime(time) }</p>
						}
					</div>
				</div>
				<p
					className={ styles.commentText }
					dangerouslySetInnerHTML={ { __html: renderedText ?? renderedComment } }
				/>
			</React.Fragment>
		);
	}

	deleteReviewOrComment = (event: React.MouseEvent): void => {
		const { deleteReviewOrComment, } = this.props;

		const data = (event.currentTarget as HTMLElement).dataset;
		if(data.reviewid) {
			deleteReviewOrComment(parseInt(data.reviewid), data.id ? parseInt(data.id) : undefined);
		} else {
			deleteReviewOrComment(parseInt(data.id!));
		}
	};

	renderAddReviewComment = (
		selectComment: (e: React.MouseEvent | React.FocusEvent) => void,
		commentReply: string
	): React.ReactNode => {
		return (
			<ThemeContext.Provider value={ textareaHidden }>
				<div className={ styles.commentReplyTextArea }>
					<Textarea
						width={ 200 }
						rows={ 1 }
						autoResize
						placeholder={ texts.sendButton }
						onValueChange={ this.onTextareaValueChange }
						onFocus={ selectComment }
						value={ commentReply }
					/>
				</div>
				<button
					disabled={ commentReply === '' }
					className={ commentReply ? styles.commentReplyButtonActive : styles.commentReplyButton }
					onClick={ this.sendComment }
					onFocus={ selectComment }>
					<Send3/>
				</button>
			</ThemeContext.Provider>
		);
	};

	onTextareaValueChange = (value: string): void => {
		const { commentsReplies, } = this.state;
		const { selectedReviewId, } = this.props;

		const newCommentsReplies = { ...commentsReplies };

		newCommentsReplies[selectedReviewId] = value;

		this.setState({
			commentsReplies: newCommentsReplies,
		});
	};

	sendComment = (): void => {
		const { selectedReviewId, addReviewComment, } = this.props;
		const { commentsReplies, } = this.state;

		addReviewComment(selectedReviewId, commentsReplies[selectedReviewId]);
		this.setState({
			commentsReplies: { ...commentsReplies, [selectedReviewId]: '' },
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
