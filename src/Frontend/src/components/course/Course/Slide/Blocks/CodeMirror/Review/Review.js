import React from "react";
import PropTypes from "prop-types";
import classNames from "classnames";

import Avatar from "src/components/common/Avatar/Avatar";
import { Textarea, ThemeContext, } from "ui";
import { Send3 } from "icons";
import texts from "./Review.texts";
import { textareaHidden } from "src/uiTheme";

import styles from "./Review.less";


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
		this.addMarginsToComments();
	}

	componentDidUpdate(prevProps, prevState, snapshot) {
		const { reviews, allCommentsLength, } = this.props;

		let sameValues = allCommentsLength === prevProps.allCommentsLength;

		if(sameValues) {
			const reviewsIds = reviews.map(r => r.id);
			const oldReviewsIds = prevProps.reviews.map(r => r.id);

			reviewsIds.sort();
			oldReviewsIds.sort();


			for (const [i, id] of reviewsIds.entries()) {
				if(oldReviewsIds[i] !== id) {
					sameValues = false;
				}
			}
		}

		if(!sameValues) {
			this.setState({
				comments: this.getCommentsOrderByStart(reviews)
					.map(r => ({
						margin: 0,
						review: r,
						ref: React.createRef(),
					})),
				commentsReplies: this.buildCommentsReplies(reviews),
			});
		} else if(this.state.comments[0].margin === 0) {
			this.addMarginsToComments();
		}
	}

	addMarginsToComments = () => {
		const { comments } = this.state;
		const commentsWithMargin = [...comments];

		let lastReviewBottomHeight = 0;
		for (const comment of commentsWithMargin) {
			const { anchorTop } = comment.review;
			const height = comment.ref.current.offsetHeight;
			const offset = Math.max(5, anchorTop - lastReviewBottomHeight);

			comment.margin = offset;
			lastReviewBottomHeight += offset + height;
		}

		this.setState({
			comments: commentsWithMargin,
		})
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
		const { commentsReplies, } = this.state;
		const className = classNames(styles.comment, { [styles.selectedReviewCommentWrapper]: selectedReviewId === id });

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
				style={ { marginTop: `${ margin }px` } }
			>
				{ this.renderComment(review) }
				{
					!author.isBot &&
					<ol className={ styles.commentRepliesList }>
						{ comments.map((c, i) =>
							<li className={ styles.commentReply } key={ i }>
								{ this.renderComment(c) }
							</li>)
						}
					</ol>
				}
				{ !author.isBot && this.renderAddReviewComment(selectComment, commentsReplies[id]) }
			</li>
		);
	}

	renderComment = ({ author, startLine, finishLine, addingTime, publishTime, text, comment, }) => {
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
						</span>
						{ time &&
						<p className={ styles.commentAddingTime }>{ texts.getAddingTime(time) }</p>
						}
					</div>
				</div>
				<p className={ styles.commentText }>{ comment || text }</p>
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
	onSelectComment: PropTypes.func,
	selectedReviewId: PropTypes.number,
	addReviewComment: PropTypes.func,
	allCommentsLength: PropTypes.number,
}

export default Review;
