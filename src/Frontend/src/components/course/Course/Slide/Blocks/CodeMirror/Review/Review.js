import React from "react";
import PropTypes from "prop-types";

import Avatar from "src/components/common/Avatar/Avatar";

import classNames from "classnames";
import { getDateDDMMYY } from "src/utils/getMoment";

import styles from "./Review.less";


class Review extends React.Component {
	constructor(props) {
		super(props);

		this.state = {
			comments: props.reviews
				.map(r => ({
					margin: 0,
					review: r,
					ref: React.createRef(),
				})),
		};
	}

	componentDidMount() {
		this.addMarginsToComments();
	}

	componentDidUpdate(prevProps, prevState, snapshot) {
		if(this.props.reviews !== prevProps.reviews) {
			this.setState({
				comments: this.props.reviews
					.map(r => ({
						margin: 0,
						review: r,
						ref: React.createRef(),
					})),
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
			<div className={ styles.reviewsContainer }>
				{ comments.map(this.renderComment) }
			</div>
		);
	}

	renderComment = ({ review: { addingTime, author, comment, finishLine, finishPosition, startLine, startPosition, }, margin, ref, }, i,) => {
		const { selectedReviewIndex, onSelectComment, } = this.props;
		const className = classNames(styles.comment, { [styles.selectedReviewCommentWrapper]: selectedReviewIndex === i });

		if(!author) {
			author = { visibleName: 'Ulearn bot', id: 'bot', };
		}

		return (
			<div key={ i } className={ className } ref={ ref }
				 onClick={ (e) => onSelectComment(e, i) }
				 style={ { marginTop: `${ margin }px` } }
			>
				<div className={ styles.authorWrapper }>
					<Avatar user={ author } size="big" className={ styles.commentAvatar }/>
					<div className={ styles.authorCredentialsWrapper }>
						{ author.visibleName }
						<span className={ styles.commentLine }>{ `строка ${ startLine + 1 }` }</span>
						{ addingTime && <p className={ styles.addingTime }>{ getDateDDMMYY(addingTime) }</p> }
					</div>
				</div>
				<p className={ styles.commentText }>{ comment }</p>
			</div>
		);
	}
}

Review.propTypes = {
	reviews: PropTypes.array,
	onSelectComment: PropTypes.func,
	selectedReviewIndex: PropTypes.number,
}

export default Review;
