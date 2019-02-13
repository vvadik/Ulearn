import React from 'react';
import { storiesOf } from '@storybook/react';
// import { withViewport } from '@storybook/addon-viewport';
import {action} from "@storybook/addon-actions";
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


class OpenCommentForm extends React.Component {
	state = {
		openForm: false,
	};

	render() {
		return (
			<React.Fragment>
				<Comment
					onClick={this.onClick}
					author={nameOnlyUser}
					commentText='Дэн Абрамов предлагает не беспокоиться, но его слова действуют не на всех'
					dateAdded={'2019-01-01T01:37:56'}
					likesCount={10}
					sending={false}/>
				{ this.state.openForm && <CommentSendForm onSubmit={action('sendComment')} commentId={'2'} author={userWithAvatar} sending={false}/> }
			</React.Fragment>
		)
	}

	onClick = () =>  {
	this.setState({
		openForm: true,
	});
	console.log('!');
}
}

storiesOf('Comments/Comment', module)
	.add('desktop', () => (
		<Comment
			author={nameOnlyUser}
			commentText='Сама природа JS и его способы использования готовят нас к тому, что никогда не настанет светлых времен с современными рантаймами.'
			dateAdded={'2019-01-01T01:37:56'}
			likesCount={10}
			isLiked={true}
			sending={false}
			showReplyButton={true}>
			<div>Форма ответа на комментарий</div>
		</Comment>
	), { viewport: 'desktop' });