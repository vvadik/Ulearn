import React from 'react';
import { storiesOf } from '@storybook/react';
import Toast from "@skbkontur/react-ui/components/Toast/Toast";
import Comment from './Comment';
import CommentSendForm from "../CommentSendForm/CommentSendForm";

import '../../../common.less';

const userWithAvatar = {
	"id": "2",
	"visibleName": "Vasiliy Terkin",
	"avatarUrl": "https://staff.skbkontur.ru/content/images/default-user-woman.png",
};

class WrapperForCommentsByRoles extends React.Component {
	state = {
		text: '',
		commentsList: [
			{ id: 1,
			commentText: "Решать эти задачи **можно** прямо в браузере, а специальная проверяющая система тут же проверит ваше решение.",
			renderCommentText: "Решать эти задачи <b>можно</b> прямо в браузере, а специальная проверяющая система тут же проверит ваше решение.",
			author: {
				"id": "11",
				"visibleName": "Louisa",
				"avatarUrl": null,
				"isSystemAdministrator": false,
				"courseRole": "Student",
				"courseAccesses": ["nothing"],
			},
			replies: [
			{ id: 2000,
			author: {
				"id": "55",
				"visibleName": "Мария Парадеева",
				"avatarUrl": null,
				"isSystemAdministrator": false,
				"courseRole": "Student",
				"courseAccesses": ["nothing"],
			},
			commentText: "Я **не согласна**",
			renderCommentText: "Я <b>не согласна</b>",
			publish_time: "2019-02-18T14:12:41.947",
			isApproved: true,
			isCorrectAnswer: false,
			likesCount: 0,
			parentCommentId: 1999
			},
			]},
			{id: 2,
			commentText: "После успешного решения некоторых задач вы сможете посмотреть решения других обучающихся.",
			renderCommentText: "После успешного решения некоторых задач вы сможете посмотреть решения других обучающихся",
			author: {
				"id": "2",
				"visibleName": "Vasiliy Terkin",
				"avatarUrl": 'https://staff.skbkontur.ru/content/images/default-user-woman.png',
				"isSystemAdministrator": false,
				"courseRole": "Student",
				"courseAccesses": ["nothing"],
			}, replies: []},
			{id: 3,
			commentText: "Если вам что-то непонятно или вы нашли ошибку на слайде — пишите об этом в комментариях под слайдом.",
			renderCommentText: "Если вам что-то непонятно или вы нашли ошибку на слайде — пишите об этом в комментариях под слайдом.",
			author: {
				"id": "33",
				"visibleName": "Henry",
				"avatarUrl": null,
				"isSystemAdministrator": false,
				"courseRole": "Instructor",
				"courseAccesses": ["editPinAndRemoveComments"],
			}, replies: []},
			{id: 4,
			commentText: "Вторая часть курса Основы программирования на C#.",
			renderCommentText: "Вторая часть курса Основы программирования на C#.",
			author: {
				"id": "3",
				"visibleName": "Katelin",
				"avatarUrl": "https://staff.skbkontur.ru/content/images/default-user-woman.png",
				"isSystemAdministrator": false,
				"courseRole": "Instructor",
				"courseAccesses": ["viewAllStudentsSubmissions"],
			}, replies: []},
			{id: 5,
			commentText: "Использование этих методов позволят обойтись без циклов, а следовательно делает код понятнее, короче.",
			renderCommentText: "Использование этих методов позволят обойтись без циклов, а следовательно делает код понятнее, короче.",
			author: {
				"id": "3",
				"visibleName": "Robin",
				"avatarUrl": null,
				"isSystemAdministrator": false,
				"courseRole": "Instructor",
				"courseAccesses": ["editPinAndRemoveComments", "viewAllStudentsSubmissions"],
			}, replies: []},
			{id: 6,
			commentText: "Качественные имена, стиль именования, комментарии, разбиение кода на методы.",
			renderCommentText: "Качественные имена, стиль именования, комментарии, разбиение кода на методы.",
			author: {
				"id": "3",
				"visibleName": "Richard",
				"avatarUrl": "https://staff.skbkontur.ru/content/images/default-user-woman.png",
				"isSystemAdministrator": false,
				"courseRole": "CourseAdmin",
				"courseAccesses": ["editPinAndRemoveComments"],
			}, replies: []}
		]};

	render() {
		return (
			<React.Fragment>
				{ this.state.commentsList.sort((a, b) => a.author.visibleName.localeCompare(b.author.visibleName))
					.map(comment => {
					return (
						<div key={comment.id}>
							<h3>{` Автор.id: ${userWithAvatar.id} / Пользователь.id: ${comment.author.id} / Роль: ${comment.author.courseRole} / Права: ${comment.author.courseAccesses}` }</h3>
							<Comment
								url={'https://dev.ulearn.me/Course/BasicProgramming/eb894d4d-5854-4684-898b-5480895685e5?CheckQueueItemId=232301&Group=492'}
								commentId={comment.id}
								author={comment.author}
								user={userWithAvatar}
								userRoles={comment.author}
								text={comment.commentText}
								renderCommentText={comment.renderCommentText}
								publishDate={'2019-01-01T01:37:56'}
								likesCount={10}
								isLiked={true}
								sending={false}
								replies={comment.replies}
								deleteComment={this.deleteComment}
								onSubmit={this.onSubmit}>
								<CommentSendForm
									onSubmit={() => this.addComment(comment.id, this.state.text)}
									handleChange={this.onChange}
									text={this.state.text}
									submitTitle={'Отправить'}
									commentId={'1'}
									sending={false}
									autofocus
									author={userWithAvatar}/>
								{ comment.replies ? comment.replies.map(reply => {
									return (
											<Comment
											url={'https://dev.ulearn.me/Course/BasicProgramming/eb894d4d-5854-4684-898b-5480895685e5?CheckQueueItemId=232301&Group=492'}
											commentId={reply.id}
											author={reply.author}
											user={userWithAvatar}
											userRoles={reply.author}
											text={reply.commentText}
											renderCommentText={reply.renderCommentText}
											publishDate={'2019-01-01T01:37:56'}
											likesCount={10}
											isLiked={true}
											sending={false}
											deleteComment={this.deleteComment}
											onSubmit={this.onSubmit}
											parentCommentId={reply.parentCommentId} />)
									}) : null
								}
							</Comment>
						</div>
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

	onChange = (event) => {
		this.setState({ text: event.target.value });
		console.log(this.state.text);
	};

	addComment = (id, text) => {
		const newReplyComment = [{
		id: 2000,
		author: {
		"id": "55",
			"visibleName": "Мария Парадеева",
			"avatarUrl": null,
			"isSystemAdministrator": false,
			"courseRole": "Student",
			"courseAccesses": ["nothing"],
		},
		commentText: text,
		renderCommentText: text,
		isLiked: false,
		likesCount: 0,
		parentCommentId: id,
		}];

		const changedComment = {...this.state.commentsList.find(comment => comment.id === id),
			replies: this.state.commentsList.find(comment => comment.id === id).replies.concat(newReplyComment)};
		const commentListWithoutChangedComment = this.state.commentsList.filter(comment => comment.id !== id);

		this.setState({
			commentsList: [...commentListWithoutChangedComment, changedComment],
			showReplyForm: false,
		});
	};

	onSubmit = (id, text) => {
		const changedComment = this.state.commentsList.find(comment => comment.id === id);
		const commentListWithoutChangedComment = this.state.commentsList.filter(comment => comment.id !== id);

		this.setState({
			commentsList: [...commentListWithoutChangedComment,
				{...changedComment, commentText: text, renderCommentText: text}],
		})
	};
}

storiesOf('Comments/Comment', module)
	.add('список комментариев с ролями пользователей', () => (
		<WrapperForCommentsByRoles />
	), { viewport: 'desktop' });