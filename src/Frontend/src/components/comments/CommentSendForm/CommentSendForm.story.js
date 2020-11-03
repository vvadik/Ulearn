import React from "react";
import { action } from "@storybook/addon-actions";
import { withViewport } from "@storybook/addon-viewport";
import CommentSendForm from "./CommentSendForm";

import "../../../common.less";

const nameOnlyUser = {
	id: "1",
	visibleName: "lol",
	avatarUrl: null,
};

const userWithAvatar = {
	id: "11",
	visibleName: "Vasiliy Terkin",
	avatarUrl:
		"https://staff.skbkontur.ru/content/images/default-user-woman.png",
};

class SendingCommentStory extends React.Component {
	state = {
		id: 1,
		sending: false,
	};

	render() {
		return (
			<CommentSendForm
				handleSubmit={this.handleSubmit}
				commentId={this.state.id}
				author={nameOnlyUser}
				sending={this.state.sending}
			/>
		);
	}

	handleSubmit = () => {
		this.setState({
			sending: true,
		});

		setTimeout(() => {
			let newState = {
				sending: false,
			};
			if (this.props.success) newState.id = 2;
			this.setState(newState);
		}, 500);
	};
}

export default {
	title: "Comments/CommentSendForm",
	decorators: [withViewport(), withViewport(), withViewport()],
};

export const Desktop = () => (
	<div>
		<h2>Формы с разными кнопками отправки</h2>
		<h3>Оставить комментарий</h3>
		<CommentSendForm
			handleSubmit={action("sendComment")}
			commentId={1}
			author={nameOnlyUser}
			sending={false}
		/>
		<h3>Отправить ответ на комментарий</h3>
		<CommentSendForm
			handleSubmit={action("addReplyToComment")}
			submitTitle={"Отправить"}
			commentId={1}
			author={nameOnlyUser}
			sending={false}
		/>
		<h3>Редактировать комментарий с кнопкой отмены отправки</h3>
		<CommentSendForm
			handleSubmit={action("editComment")}
			submitTitle={"Сохранить"}
			handleCancel={action("cancelComment")}
			commentId={1}
			author={nameOnlyUser}
			sending={false}
		/>
		<h3>Форма в состоянии отправки</h3>
		<CommentSendForm
			handleSubmit={action("sendComment")}
			commentId={2}
			author={userWithAvatar}
			sending={true}
		/>
		<h3>Успешная отправка комментария очищает поле ввода</h3>
		<SendingCommentStory success={true} />
		<h3>Ошибка при отправке комментария НЕ очищает поле ввода</h3>
		<SendingCommentStory success={false} />
	</div>
);

Desktop.storyName = "desktop";
Desktop.parameters = { viewport: "desktop" };

export const Tablet = () => (
	<CommentSendForm
		handleSubmit={action("sendComment")}
		commentId={1}
		author={nameOnlyUser}
		sending={false}
	/>
);

Tablet.storyName = "tablet";
Tablet.parameters = { viewport: "tablet" };

export const Mobile = () => (
	<CommentSendForm
		handleSubmit={action("sendComment")}
		commentId={1}
		author={nameOnlyUser}
		sending={false}
	/>
);

Mobile.storyName = "mobile";
Mobile.parameters = { viewport: "mobile" };
