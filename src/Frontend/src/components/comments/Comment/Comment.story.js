import React from 'react';
import { storiesOf } from '@storybook/react';
// import { withViewport } from '@storybook/addon-viewport';
import {action} from "@storybook/addon-actions";
import Comment from './Comment';
import CommentSendForm from "../CommentSendForm/CommentSendForm";
import Toast from "@skbkontur/react-ui/components/Toast/Toast";

import '../../../common.less';

const nameOnlyUser = {
	"visibleName": "Garold",
	"avatarUrl": null,
};

const userWithAvatar = {
	"visibleName": "Vasiliy Terkin",
	"avatarUrl": "https://staff.skbkontur.ru/content/images/default-user-woman.png",
};


class DeleteComment extends React.Component {
	state = {
		commentsList: [{id: 1, text: "hello, i'm a first comment"}, {id: 2, text: "hello, i'm a second comment"}],
	};

	render() {

		return (
			<React.Fragment>
				{ this.state.commentsList.map(comment => {
					return (
					<Comment
						key={comment.id}
						commentId={comment.id}
						author={nameOnlyUser}
						commentHtml={comment.text}
						publishDate={'2019-01-01T01:37:56'}
						likesCount={10}
						isLiked={true}
						sending={false}
						showReplyButton={true}
						deleteComment={this.deleteComment}>
						<CommentSendForm author={userWithAvatar}/>
					</Comment>
				)}
			)}
			</React.Fragment>
		)
	}

	deleteComment = (commentId) => {
		const newCommentsList = this.state.commentsList
			.filter(comment => comment.id !== commentId);

		this.setState({
			commentsList: newCommentsList,
		});

		Toast.push("Комментарий удалён",  {
			label: "Восстановить",
			handler: () => {
				this.setState({
					commentsList: [...newCommentsList, {id: commentId}],
				});
				Toast.push("Комментарий восстановлен")
			}
		});
	}
}

storiesOf('Comments/Comment', module)
	.add('only comment', () => (
		<Comment
			author={nameOnlyUser}
			commentHtml={'Сама природа <code>JS</code> и его способы <b>использования</b> готовят нас к тому, что никогда не настанет светлых времен с <a href="https://kontur.ru">современными рантаймами</a>.<br><br>Аминь!'}
			dateAdded={'2019-01-01T01:37:56'}
			likesCount={10}
			isLiked={true}
			sending={false}
			showReplyButton={true}>
			<CommentSendForm author={userWithAvatar} />
		</Comment>
	), { viewport: 'desktop' })
	.add('comment with delete action', () => (
		<DeleteComment />
	),{ viewport: 'desktop' } );