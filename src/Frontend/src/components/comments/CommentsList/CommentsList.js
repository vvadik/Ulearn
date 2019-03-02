import React, {Component, createContext} from 'react';
import PropTypes from "prop-types";
import Thread from "../Thread/Thread";

export const CommentContext = createContext({
	dispatch: () => {},
});

const handlersName = {
	togglePinnedMark: 'togglePinnedMark',
	toggleCorrectAnswerMark: 'toggleCorrectAnswerMark',
	toggleVisibleMark: 'toggleVisibleMark',
	toggleLikeChange: 'toggleLikeChange',
	editComment: 'editComment',
	deleteComment: 'deleteComment',
	addReplyComment: 'addReplyComment',
};

class CommentsList extends Component {
	constructor(props) {
		super(props);

		this.state = {
			comments: [],
		};
	}

	render() {
		const { dispatch } = this;

		return (
			<CommentContext.Provider value={{ dispatch }}>
				{this.state.comments.map((comment) =>
					<Thread
						key={comment.id}
						comment={comment}
						handleLikeChange={this.handleLikeChange}
						handleCorrectAnswerMark={this.handleCorrectAnswerMark}
						handleVisibleMark={this.handleVisibleMark}
						handlePinnedToTopMark={this.handlePinnedToTopMark}
						handleEditComment={this.handleEditComment}
						handleDeleteComment={this.handleDeleteComment}
						handleAddReplyComment={this.handleAddReplyComment}
						getUserSolution={this.getUserSolution}
					/>)}
			</CommentContext.Provider>
		)
	}

	handleLikeChange = (id, isLiked) => {

		// if (isLiked) {
			// POST: api.comment.likeComment(id);
		// 	} else {
			//	DELETE: api.comment.dislike(id);
		// }
	};

	handleCorrectAnswerMark = (id, isCorrectAnswer) => {
		//PATCH: api.comment.changeComment(id, `isCorrectAnswer: ${isCorrectAnswer}`)
	};

	handleVisibleMark = (id, isApproved) => {
		//PATCH: api.comment.changeComment(id, `isApproved: ${isApproved}`)
	};

	handlePinnedToTopMark = (id, isPinnedToTop) => {
		//PATCH: api.comment.changeComment(id, `isPinnedToTop: ${isPinnedToTop}`)
	};

	handleEditComment = (text) => {
		//PATCH: api.comment.changeComment(id, `text: ${text}`)
	};

	handleAddReplyComment = (text) => {
		console.log(text);
	};

	handleDeleteComment = (id) => {
		//DELETE: api.comment.deleteComment(id);
	};

	getUserSolution(userId) {
		return `https://dev.ulearn.me/Analytics/UserSolutions?courseId=BasicProgramming&slideId=90bcb61e-57f0-4baa-8bc9-10c9cfd27f58&userId=${userId}`;
	};

	dispatch = (action) => {
		switch (action) {
			case handlersName.togglePinnedMark:
				return this.handlePinnedToTopMark();
			case handlersName.toggleVisibleMark:
				return this.handleVisibleMark();
			case handlersName.toggleCorrectAnswerMark:
				return this.handleCorrectAnswerMark();
			case handlersName.editComment:
				return this.handleEditComment();
			case handlersName.deleteComment:
				return this.handleDeleteComment();
			case handlersName.addReplyComment:
				return this.handleAddReplyComment();
			case handlersName.toggleLikeChange:
				return this.handleLikeChange();
			default:
				return console.warn(`No case for this handlers name ${action}`)
		}
	};
}

CommentsList.propTypes = {

};

export default CommentsList;