import React from "react";
import PropTypes from "prop-types";
import classNames from "classnames";

import Avatar from "src/components/common/Avatar/Avatar";
import { Loader, Textarea, ThemeContext, } from "ui";
import { Send3, Trash, Delete, } from "icons";

import { textareaHidden } from "src/uiTheme";

import styles from "./Review.less";
import texts from "./Review.texts";

class Review extends React.Component {
	constructor(props) {
		super(props);

		this.state = {
			comments: this.getCommentsOrderByStart(props.reviews)
				.map(r => ({
					margin: 0,
					review: r,
					ref: React.createRef(),
				})),
			commentsReplies: this.buildCommentsReplies(props.reviews),
			marginsAdded: false,
		};
	}

	getCommentsOrderByStart = (reviews) => {
		return reviews.sort((r1, r2) => {
			if(r1.startLine < r2.startLine || (r1.startLine === r2.startLine && r1.startPosition < r2.startPosition))
				return -1;
			if(r2.startLine < r1.startLine || (r2.startLine === r1.startLine && r2.startPosition < r1.startPosition))
				return 1;
			return 0;
		});
	}

	buildCommentsReplies = (reviews) => {
		const commentsReplies = {};

		for (const { id } of reviews) {
			commentsReplies[id] = '';
		}

		return commentsReplies;
	}

	componentDidMount() {
		//add margins in a moment after mounting, so code could resize correctly to calculate correct topAnchor
		this.setState({}, () => this.addMarginsToComments());
	}

	componentDidUpdate(prevProps, prevState, snapshot) {
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

			for (const [i, { id, innerCommentsIds }] of newReviews.entries()) {
				const oldReview = oldReviews[i];
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
						ref: React.createRef(),
					})),
				commentsReplies: this.buildCommentsReplies(reviews),
				marginsAdded: false,
			});
		} else if(!this.state.marginsAdded
			|| (selectedReviewId !== prevProps.selectedReviewId && selectedReviewId >= 0)) {
			this.addMarginsToComments();
		}
	}

	addMarginsToComments = () => {
		const { comments, } = this.state;
		const { selectedReviewId, getReviewAnchorTop, } = this.props;

		const commentsWithMargin = [...comments];
		const selectedReviewIndex = commentsWithMargin.findIndex(c => c.review.id === selectedReviewId);
		let lastReviewBottomHeight = 0;

		if(selectedReviewIndex >= 0) {
			const selectedComment = commentsWithMargin[selectedReviewIndex];
			const anchorTop = getReviewAnchorTop(selectedComment.review);
			const height = selectedComment.ref.current.offsetHeight;
			const offset = Math.max(5, anchorTop);

			let spaceToSelectedReview = offset;
			selectedComment.margin = offset;
			lastReviewBottomHeight = offset + height;

			if(selectedReviewIndex > 0) {
				let totalCommentsHeight = 5;
				for (let i = 0; i < selectedReviewIndex; i++) {
					const comment = commentsWithMargin[i];
					const height = comment.ref.current.offsetHeight;
					totalCommentsHeight += height + 5;
				}

				commentsWithMargin[0].margin = spaceToSelectedReview - totalCommentsHeight;
				for (let i = 1; i <= selectedReviewIndex; i++) {
					const comment = commentsWithMargin[i];
					const anchorTop = getReviewAnchorTop(comment.review);
					const height = comment.ref.current.offsetHeight;
					comment.margin = Math.max(5, Math.min(anchorTop, spaceToSelectedReview - totalCommentsHeight));
					spaceToSelectedReview -= (height + 5);
					totalCommentsHeight -= (height);
				}
			}
		}

		for (let i = selectedReviewIndex + 1; i < commentsWithMargin.length; i++) {
			const comment = commentsWithMargin[i];
			const anchorTop = getReviewAnchorTop(comment.review);
			const height = comment.ref.current.offsetHeight;
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
	}

	render = () => {
		const { comments, } = this.state;

		return (
			<ol className={ styles.reviewsContainer }>
				{ comments.map(this.renderTopLevelComment) }
			</ol>
		);
	}

	renderTopLevelComment = ({ review, margin, ref, }, i,) => {
		const { id, comments } = review;
		const { selectedReviewId, onSelectComment, } = this.props;
		const { commentsReplies, marginsAdded, } = this.state;
		const className = classNames(styles.comment, { [styles.commentMounted]: marginsAdded }, { [styles.selectedReviewCommentWrapper]: selectedReviewId === id },);

		let author = review.author;
		if(!author) {
			author = { visibleName: 'Ulearn bot', id: 'bot', isBot: true, };
		}

		const selectComment = (e) => onSelectComment(e, id);
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
					!author.isBot &&
					<ol className={ styles.commentRepliesList }>
						{ comments.filter(r => !r.isDeleted).map((c, i) =>
							<li className={ styles.commentReply } key={ i }>
								{ this.renderComment(c, id) }
							</li>)
						}
					</ol>
				}
				{ !author.isBot && this.renderAddReviewComment(selectComment, commentsReplies[id]) }
			</li>
		);
	}

	renderComment = ({ id, author, startLine, finishLine, addingTime, publishTime, renderedText, renderedComment, isLoading, }, reviewId = null) => {
		const { userId, deleteReviewComment } = this.props;

		if(isLoading) {
			return <Loader type={ "mini" } active={ true }/>;
		}

		if(!author) {
			author = { visibleName: 'Ulearn bot', id: 'bot', isBot: true, };
		}
		const time = addingTime || publishTime;

		return (
			<React.Fragment>
				<div className={ styles.authorWrapper }>
					<Avatar user={ author } size="big" className={ styles.commentAvatar }/>
					<div className={ styles.commentInfoWrapper }>
						<span className={ styles.commentInfo }>
							<span className={ styles.authorName }>
								{ author.visibleName }
							</span>
							{ startLine &&
							<span className={ styles.commentLineNumber }>
								{ texts.getLineCapture(startLine, finishLine) }
							</span>
							}
							{
								author.id === userId && <Trash
									className={ styles.innerCommentDeleteButton }
									onClick={ () => deleteReviewComment(reviewId, id) }
									size={ 12 }
								/>

								/* TODO Not included in release
																(reviewId
																		? <Trash
																			className={ styles.innerCommentDeleteButton }
																			onClick={ () => deleteReviewComment(reviewId, id) }
																			size={ 12 }
																		/>
																		: <Delete
																			className={ styles.commentDeleteButton }
																			onClick={ () => deleteReviewComment(reviewId, id) }
																			size={ 14 }
																		/>
																)*/
							}
						</span>
						{ time &&
						<p className={ styles.commentAddingTime }>{ texts.getAddingTime(time) }</p>
						}
					</div>
				</div>
				<p className={ styles.commentText }
				   dangerouslySetInnerHTML={ { __html: renderedComment || renderedText } }/>
			</React.Fragment>
		);
	}

	renderAddReviewComment = (selectComment, commentReplie) => {
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
						value={ commentReplie }
					/>
				</div>
				<button
					disabled={ commentReplie === '' }
					className={ commentReplie ? styles.commentReplyButtonActive : styles.commentReplyButton }
					onClick={ this.sendComment }
					onFocus={ selectComment }>
					<Send3/>
				</button>
			</ThemeContext.Provider>
		);
	}

	onTextareaValueChange = (value) => {
		const { commentsReplies, } = this.state;
		const { selectedReviewId, } = this.props;

		const newCommentsReplies = { ...commentsReplies };

		newCommentsReplies[selectedReviewId] = value;

		this.setState({
			commentsReplies: newCommentsReplies,
		})
	}

	sendComment = () => {
		const { selectedReviewId, addReviewComment, } = this.props;
		const { commentsReplies, } = this.state;

		addReviewComment(selectedReviewId, commentsReplies[selectedReviewId]);
	}
}

Review.propTypes = {
	reviews: PropTypes.array,
	selectedReviewId: PropTypes.number,
	userId: PropTypes.string,
	onSelectComment: PropTypes.func,
	addReviewComment: PropTypes.func,
	deleteReviewComment: PropTypes.func,
	getReviewAnchorTop: PropTypes.func,
}

export default Review;
