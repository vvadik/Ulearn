import React from 'react';
import { storiesOf } from '@storybook/react';
import Toast from "@skbkontur/react-ui/components/Toast/Toast";
import Comment from './Comment';

import '../../../common.less';

const nameOnlyUser = {
	"id": "1",
	"visibleName": "Garold",
	"avatarUrl": null,
};

class Reply extends React.Component {
	state = {
		text: '',
		commentsList: [
			{id: 1,
				commentText: "Решать эти задачи **можно** прямо в браузере, а специальная проверяющая система тут же проверит ваше решение.",
				renderCommentText: "Решать эти задачи <b>можно</b> прямо в браузере, а специальная проверяющая система тут же проверит ваше решение.",
				author: {
					"id": "2",
					"visibleName": "Henry",
					"avatarUrl": null,
					"isSystemAdministrator": false,
					"courseRole": "Instructor",
					"courseAccesses": ["editPinAndRemoveComments", "viewAllStudentsSubmissions"],
				}},
		]
	};

	render() {
		return (
			<React.Fragment>
				{ this.state.commentsList.map(comment => {
					return (
						<Comment
							key={comment.id}
							url={'https://dev.ulearn.me/Course/BasicProgramming/eb894d4d-5854-4684-898b-5480895685e5?CheckQueueItemId=232301&Group=492'}
							parentCommentId={2000}
							commentId={comment.id}
							author={comment.author}
							user={nameOnlyUser}
							userRoles={comment.author}
							text={comment.commentText}
							renderCommentText={comment.renderCommentText}
							publishDate={'2019-01-01T01:37:56'}
							likesCount={10}
							isLiked={true}
							sending={false}
							onSubmit={this.onSubmit}
							deleteComment={this.deleteComment}>
						</Comment>
					)}
				)}
			</React.Fragment>
		)
	}

	deleteComment = (commentId) => {
		const newCommentsList = this.state.commentsList
			.filter(comment => comment.id !== commentId);

		const deleteComment = this.state.commentsList
			.find(comment => comment.id === commentId);

		this.setState({
			commentsList: newCommentsList,
		});

		Toast.push("Комментарий удалён", {
			label: "Восстановить",
			handler: () => {
				this.setState({
					commentsList: [...newCommentsList, deleteComment],
				});
				Toast.push("Комментарий восстановлен")
			}
		});
	};

	onSubmit = (id, text) => {
		const changedComment = this.state.commentsList.find(comment => comment.id === id);
		const commentListWithoutChangedComment = this.state.commentsList.filter(comment => comment.id !== id);
		console.log(text);

		this.setState({
			commentsList: [...commentListWithoutChangedComment,
				{...changedComment, commentText: text, renderCommentText: text}],
		})
	};
}

storiesOf('Comments/Reply', module)
	.add('ответ на комментарий', () => (
		<Reply />
	),{ viewport: 'desktop' } );