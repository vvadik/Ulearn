import React from 'react';

import styles from "./style.less";

function Error404(): React.ReactElement {
	return (
		<div className={ styles.wrapper }>
			<img src="/Content/404.jpg" alt="в поисках адреса страницы"
				 className={ styles.image }/>
			<div className={ styles.text }>
				<h2>СТРАНИЦА НЕ НАЙДЕНА</h2>
				<p>
					<a href="/">Вернуться на главную</a>
				</p>
				<p>
					Если адрес страницы верный, напишите нам о&nbsp;том, что произошло,
					на&nbsp;<a href="mailto:support@ulearn.me">support@ulearn.me</a>.
				</p>
			</div>
		</div>
	);
}

export default Error404;
