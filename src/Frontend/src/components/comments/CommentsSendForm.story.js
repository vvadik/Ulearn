import React from 'react';
import {storiesOf} from '@storybook/react';
import {action} from '@storybook/addon-actions';
import CommentSendForm from './CommentSendForm';

import './../../common.less';

const nameOnlyUser = {
	"visibleName": "lol",
	"avatarUrl": null,
};

class Story extends React.Component {
	state = {
		id: Date.now().toString(),
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
			this.setState({
				//id: Date.now().toString(),
				sending: false,
			})
		}, 500)

	}
}

storiesOf('Comments/CommentSendForm', module)
	.add('default', () => (
		<div>
			<CommentSendForm onSubmit={action('sendComment')} commentId={'1'} author={nameOnlyUser} sending={false}/>
			<CommentSendForm onSubmit={action('sendComment')} commentId={'2'} author={nameOnlyUser} sending={true}/>
			<Story/>
		</div>
	))
	.add('sending', () => (
		<CommentSendForm onSubmit={action('sendComment')} sending={true} author={nameOnlyUser}/>
	));

