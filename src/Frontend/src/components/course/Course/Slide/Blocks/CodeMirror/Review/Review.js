import React from "react";
import PropTypes from "prop-types";
import classNames from "classnames";

import Avatar from "src/components/common/Avatar/Avatar";
import { Textarea, ThemeContext, } from "ui";
import { Send3 } from "icons";
import texts from "./Review.texts";
import { textareaHidden } from "src/uiTheme";

import styles from "./Review.less";
import api from "src/api";


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
		const { reviews } = this.props;

		let sameValues = reviews.length === prevProps.reviews.length;

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
				{ comments.map(this.renderComment) }
			</ol>
		);
	}

	renderComment = ({ review: { id, addingTime, author, comment, finishLine, finishPosition, startLine, startPosition, }, margin, ref, }, i,) => {
		const { selectedReviewId, onSelectComment, } = this.props;
		const { commentsReplies, } = this.state;
		const className = classNames(styles.comment, { [styles.selectedReviewCommentWrapper]: selectedReviewId === id });

		if(!author) {
			author = { visibleName: 'Ulearn bot', id: 'bot', isBot: true, };
		}

		const selectComment = (e) => onSelectComment(e, id);

		return (
			<li key={ i } className={ className } ref={ ref }
				onClick={ selectComment }
				style={ { marginTop: `${ margin }px` } }
			>
				<div className={ styles.authorWrapper }>
					<Avatar user={ author } size="big" className={ styles.commentAvatar }/>
					<div className={ styles.commentInfoWrapper }>
						<span className={ styles.commentInfo }>
							<span className={ styles.authorName }>
								{ author.visibleName }
							</span>
							<span className={ styles.commentLineNumber }>
								{ texts.getLineCapture(startLine, finishLine) }
							</span>
						</span>
						{ addingTime &&
						<p className={ styles.commentAddingTime }>{ texts.getAddingTime(addingTime) }</p>
						}
					</div>
				</div>
				<p className={ styles.commentText }>{ comment }</p>
				{ !author.isBot &&
				<ThemeContext.Provider value={ textareaHidden }>
					<div className={ styles.commentReplyTextArea }>
						<Textarea
							width={ 200 }
							rows={ 1 }
							autoResize
							placeholder={ texts.sendButton }
							onValueChange={ this.onTextareaValueChange }
							onFocus={ selectComment }
							value={ commentsReplies[id] }
						/>
					</div>
					<button
						disabled={ commentsReplies[id] === '' }
						className={ commentsReplies[id] ? styles.commentReplyButtonActive : styles.commentReplyButton }
						onClick={ this.sendComment }
						onFocus={ selectComment }>
						<Send3/>
					</button>
				</ThemeContext.Provider>
				}
			</li>
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
		const { selectedReviewId, } = this.props;
		const { commentsReplies, } = this.state;

		api.exercise.addCodeReviewComment(selectedReviewId, commentsReplies[selectedReviewId]);
	}
}

Review.propTypes = {
	reviews: PropTypes.array,
	onSelectComment: PropTypes.func,
	selectedReviewId: PropTypes.number,
}

export default Review;
