import React from "react";
import Icon from "@skbkontur/react-icons";

import styles from "../Comment.less";

const actions = {
	reply: {
		action: 'reply',
		icon: 'ArrowCorner1',
		text: 'Ответить',
	},
};

export default function Actions(props) {
	const { user, author, userRoles, parentCommentId, url, dispatch,
		canModerateComments, isCorrectAnswer, isPinnedToTop } = props;

	return (
		<div className={styles.actions}>
			{ !parentCommentId &&
					/*<Action type='reply' />}*/
					<Button onClick={() => dispatch('reply')} icon={'ArrowCorner1'} text={'Ответить'} /> }
			{ user.id === author.id &&
				<div>
					{<Button onClick={() => dispatch('edit')} icon={'Edit'} text={'Редактировать'} />}
					{<Button onClick={() => dispatch('delete')} icon={'Delete'} text={'Удалить'} />}
				</div>}
			{ canModerateComments(userRoles, 'viewAllStudentsSubmissions') &&
				<Link url={url} /> }
			{ canModerateComments(userRoles, 'editPinAndRemoveComments') &&
				<div>
					{ parentCommentId
						? <Button onClick={() => dispatch('toggleCorrect')}
								  icon={'Star2'}
								  text={isCorrectAnswer ? 'Снять отметку' : 'Отметить правильным'} />
						: <Button onClick={() => dispatch('togglePinned')}
								icon={'Pin'}
								text={isPinnedToTop ? 'Открепить' : 'Закрепить'} /> }
				</div> }
		</div>
	)
}

const Button = ({ onClick, icon, text }) => (
	<button type="button" className={styles.sendAnswer} onClick={onClick}>
		<Icon name={icon} />
		<span className={styles.buttonText}>{text}</span>
	</button>
);

const Link = ({ url }) => (
	<a href={url} className={styles.sendAnswer}>
		<Icon name='DocumentLite' />
		<span className={styles.linkText}>Посмотреть решения</span>
	</a>
);