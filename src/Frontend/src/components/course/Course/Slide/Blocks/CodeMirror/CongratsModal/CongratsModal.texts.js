import React from "react";
import { Link } from "react-router-dom";
import { accountPath } from "src/consts/routes";

export default {
	title: 'Ура!',
	getBodyText: () => (
		<React.Fragment>
			<p>Вы отправили задачу на ревью.</p>
			Преподаватель проверит решение<br/>
			и поставит баллы за ревью.<br/>
			Об этом придёт уведомление на почту<br/>
			или в Telegram (если он <Link to={ accountPath }>настроен</Link>).
		</React.Fragment>
	),
	closeButton: 'Вернуться к заданию',

	selfCheckTitle: 'А пока…',
	selfCheckContent: <React.Fragment>
		…есть возможность улучшить решение.<br/>
		Для этого проверьте решение по чеклисту <br/>
		и учтите комментарии от бота. <br/>
		После этого отправьте новую версию решения.
	</React.Fragment>,
};