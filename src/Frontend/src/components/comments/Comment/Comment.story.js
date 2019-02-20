import React from 'react';
import { storiesOf } from '@storybook/react';
// import { withViewport } from '@storybook/addon-viewport';
import {action} from "@storybook/addon-actions";
import Toast from "@skbkontur/react-ui/components/Toast/Toast";
import Comment from './Comment';
import CommentSendForm from "../CommentSendForm/CommentSendForm";

import '../../../common.less';

const nameOnlyUser = {
	"visibleName": "Garold",
	"avatarUrl": null,
};

const userWithAvatar = {
	"visibleName": "Vasiliy Terkin",
	"avatarUrl": "https://staff.skbkontur.ru/content/images/default-user-woman.png",
};


class WrapperComment extends React.Component {
	state = {
		commentsList: [
			{id: 1, text: "Решать эти задачи можно прямо в браузере, а специальная проверяющая система тут же проверит ваше решение."},
			{id: 2, text: "После успешного решения некоторых задач вы сможете посмотреть решения других обучающихся. "},
			{id: 3, text: "Если вам что-то непонятно или вы нашли ошибку на слайде — пишите об этом в комментариях под слайдом."}],
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
						commentText={comment.text}
						publishDate={'2019-01-01T01:37:56'}
						likesCount={10}
						isLiked={true}
						sending={false}
						showReplyButton={true}
						deleteComment={this.deleteComment}
						pinnedComment={this.pinnedComment}>
						<CommentSendForm
							onSubmit={action('sendComment')}
							text={comment.text}
							submitTitle={'Отправить'}
							commentId={'1'}
							sending={false}
							// editComment={() => this.editComment(comment.id, comment.text)}
							author={userWithAvatar}/>
					</Comment>
				)}
			)}
			</React.Fragment>
		)
	}

	// editComment = (id, text) => {
	// 	const changedComment = this.state.commentsList.filter(comment => comment.id === id);
	// 	const commentListWithoutChangedComment = this.state.commentsList.filter(comment => comment.id !== id);
	// 	this.setState({
	// 		commentsList: [...commentListWithoutChangedComment, {...changedComment, text: this.state.text}]
	// 	})
	// };

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
	};

	pinnedComment = (id) => {
		const pinnedComment = this.state.commentsList.filter(comment => comment.id === id);
		const commentListWithoutPinnedComment = this.state.commentsList.filter(comment => comment.id !== id);

		this.setState({
			commentsList: pinnedComment.concat(commentListWithoutPinnedComment),
		});
	};
}

storiesOf('Comments/Comment', module)
	.add('comment with actions', () => (
		<WrapperComment />
	),{ viewport: 'desktop' } );