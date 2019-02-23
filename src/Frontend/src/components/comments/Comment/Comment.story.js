import React from 'react';
import { storiesOf } from '@storybook/react';
// import { withViewport } from '@storybook/addon-viewport';
import {action} from "@storybook/addon-actions";
import Toast from "@skbkontur/react-ui/components/Toast/Toast";
import Comment from './Comment';
import CommentSendForm from "../CommentSendForm/CommentSendForm";

import '../../../common.less';

const nameOnlyUser = {
	"id": "1",
	"visibleName": "Garold",
	"avatarUrl": null,
};

const userWithAvatar = {
	"id": "2",
	"visibleName": "Vasiliy Terkin",
	"avatarUrl": "https://staff.skbkontur.ru/content/images/default-user-woman.png",
};

class WrapperForCommentsByRoles extends React.Component {
	state = {
		commentsList: [
			{id: 1, text: "Решать эти задачи можно прямо в браузере, а специальная проверяющая система тут же проверит ваше решение.",
			author: {
				"id": "11",
				"visibleName": "Louisa",
				"avatarUrl": null,
				"isSystemAdministrator": false,
				"courseRole": "Student",
				"courseAccesses": ["nothing"],
			}},
			{id: 2, text: "После успешного решения некоторых задач вы сможете посмотреть решения других обучающихся.",
			author: {
				"id": "2",
				"visibleName": "Maria",
				"avatarUrl": 'https://staff.skbkontur.ru/content/images/default-user-woman.png',
				"isSystemAdministrator": false,
				"courseRole": "Student",
				"courseAccesses": ["nothing"],
			}},
			{id: 3, text: "Если вам что-то непонятно или вы нашли ошибку на слайде — пишите об этом в комментариях под слайдом.",
			author: {
				"id": "33",
				"visibleName": "Henry",
				"avatarUrl": null,
				"isSystemAdministrator": false,
				"courseRole": "Instructor",
				"courseAccesses": ["editPinAndRemoveComments"],
			}},
			{id: 4, text: "Вторая часть курса Основы программирования на C#.",
			author: {
				"id": "3",
				"visibleName": "Katelin",
				"avatarUrl": "https://staff.skbkontur.ru/content/images/default-user-woman.png",
				"isSystemAdministrator": false,
				"courseRole": "Instructor",
				"courseAccesses": ["viewAllStudentsSubmissions"],
			}},
			{id: 5, text: "Использование этих методов позволят обойтись без циклов, а следовательно делает код понятнее, короче.",
			author: {
				"id": "3",
				"visibleName": "Robin",
				"avatarUrl": null,
				"isSystemAdministrator": false,
				"courseRole": "Instructor",
				"courseAccesses": ["editPinAndRemoveComments", "viewAllStudentsSubmissions"],
			}},
			{id: 6, text: "Качественные имена, стиль именования, комментарии, разбиение кода на методы.",
			author: {
				"id": "3",
				"visibleName": "Richard",
				"avatarUrl": "https://staff.skbkontur.ru/content/images/default-user-woman.png",
				"isSystemAdministrator": false,
				"courseRole": "CourseAdmin",
				"courseAccesses": ["editPinAndRemoveComments"],
			}},
		]};

	render() {
		return (
			<React.Fragment>
				{ this.state.commentsList.map(comment => {
					return (
						<div>
							<h3>{` Автор.id: ${userWithAvatar.id} / Пользователь.id: ${comment.author.id} / Роль: ${comment.author.courseRole} / Права: ${comment.author.courseAccesses}` }</h3>
							<Comment
								url={'https://dev.ulearn.me/Course/BasicProgramming/eb894d4d-5854-4684-898b-5480895685e5?CheckQueueItemId=232301&Group=492'}
								key={comment.id}
								commentId={comment.id}
								author={comment.author}
								user={userWithAvatar}
								userRoles={comment.author}
								commentText={comment.text}
								publishDate={'2019-01-01T01:37:56'}
								likesCount={10}
								isLiked={true}
								sending={false}
								showReplyButton={true}
								deleteComment={this.deleteComment}
								pinComment={this.pinComment}>
								<CommentSendForm
									onSubmit={action('sendComment')}
									text={comment.text}
									submitTitle={'Отправить'}
									commentId={'1'}
									sending={false}
									autofocus
									// editComment={() => this.editComment(comment.id, comment.text)}
									author={userWithAvatar}/>
							</Comment>
						</div>
					)}
				)}
			</React.Fragment>
		)
	}
}

class ReplyToComment extends React.Component {
	state = {
		commentsList: [
			{id: 1, text: "Решать эти задачи можно прямо в браузере, а специальная проверяющая система тут же проверит ваше решение.",
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
							url={'https://dev.ulearn.me/Course/BasicProgramming/eb894d4d-5854-4684-898b-5480895685e5?CheckQueueItemId=232301&Group=492'}
							key={comment.id}
							parentCommentId={2000}
							commentId={comment.id}
							author={comment.author}
							user={nameOnlyUser}
							userRoles={comment.author}
							commentText={comment.text}
							publishDate={'2019-01-01T01:37:56'}
							likesCount={10}
							isLiked={true}
							sending={false}
							showReplyButton={true}
							deleteComment={this.deleteComment}>
							<CommentSendForm
								// editComment={() => this.editComment(comment.id, comment.text)}
								onSubmit={action('sendComment')}
								autofocus
								text={comment.text}
								submitTitle={'Отправить'}
								commentId={'1'}
								sending={false}
								author={userWithAvatar}/>
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
	};

	// editComment = (id, text) => {
	// 	const changedComment = this.state.commentsList.filter(comment => comment.id === id);
	// 	const commentListWithoutChangedComment = this.state.commentsList.filter(comment => comment.id !== id);
	// 	this.setState({
	// 		commentsList: [...commentListWithoutChangedComment, {...changedComment, text: this.state.text}]
	// 	})
	// };
}

storiesOf('Comments/Comment', module)
	.add('список комментариев с ролями пользователей', () => (
		<WrapperForCommentsByRoles />
	), { viewport: 'desktop' })
	.add('ответ на комментарий', () => (
		<ReplyToComment />
	),{ viewport: 'desktop' } );