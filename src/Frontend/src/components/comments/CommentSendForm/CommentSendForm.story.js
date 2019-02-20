import React from 'react';
import { storiesOf } from '@storybook/react';
import {action} from '@storybook/addon-actions';
import { withViewport } from '@storybook/addon-viewport';
import CommentSendForm from "./CommentSendForm";

import '../../../common.less';

const nameOnlyUser = {
	"visibleName": "lol",
	"avatarUrl": null,
};

const userWithAvatar = {
	"visibleName": "Vasiliy Terkin",
	"avatarUrl": "https://staff.skbkontur.ru/content/images/default-user-woman.png",
};

class SendingCommentStory extends React.Component {
	state = {
		id: "first",
		sending: false,
	};

	render() {
		return (
			<CommentSendForm onSubmit={this.onSubmit} commentId={this.state.id} author={nameOnlyUser} sending={this.state.sending}/>
		)
	}

	onSubmit = () => {
		this.setState({
			sending: true,
		});
		setTimeout(() => {
			let newState = {
				sending: false,
			};
			if (this.props.success)
				newState.id = "second";
			this.setState(newState)
		}, 500);
	};
}

storiesOf('Comments/CommentSendForm', module)
	.addDecorator(withViewport())
	.add('desktop', () => (
		<div>
			<CommentSendForm
				onSubmit={action('sendComment')}
				commentId={'1'}
				author={nameOnlyUser}
				sending={false}
			/>
			<CommentSendForm
				onSubmit={action('sendComment')}
				submitTitle={'Отправить'}
				commentId={'1'}
				author={nameOnlyUser}
				sending={false}
			/>
			<CommentSendForm
				onSubmit={action('sendComment')}
				submitTitle={'Сохранить'}
				onCancel={action('cancelComment')}
				commentId={'1'}
				author={nameOnlyUser}
				sending={false}
			/>
			<CommentSendForm onSubmit={action('sendComment')} commentId={'2'} author={userWithAvatar} sending={true}/>
			<h3>Успешная отправка комментария очищает поле ввода</h3>
			<SendingCommentStory success={true}/>
			<h3>Ошибка при отправке комментария НЕ очищает поле ввода</h3>
			<SendingCommentStory success={false}/>
		</div>
	), { viewport: 'desktop' })
	.addDecorator(withViewport())
	.add('tablet', () => (
		<CommentSendForm onSubmit={action('sendComment')} commentId={'1'} author={nameOnlyUser} sending={false}/>
	), { viewport: "tablet" })
	.addDecorator(withViewport())
	.add('mobile', () => (
		<CommentSendForm onSubmit={action('sendComment')} commentId={'1'} author={nameOnlyUser} sending={false}/>
	), { viewport: "mobile" });

