import React from 'react';
import PropTypes from "prop-types";
import Link from "@skbkontur/react-ui/components/Link/Link";
import Button from "@skbkontur/react-ui/components/Button/Button";
import Gapped from "@skbkontur/react-ui/components/Gapped/Gapped";

import styles from './Stub.less';

function Stub(props) {
	const { courseId, slideId } = props;
	const urlToRegister =`${window.location.origin}/Account/Register?returnUrl=/Course/${courseId}/${slideId}`;
	const urlToEnter = `${window.location.origin}/Login?returnUrl=/Course/${courseId}/${slideId}`;

	return (
		<div className={styles.stub}>
			<span className={styles.stubText}>Оставлять комментарии могут только зарегистрированные пользователи</span>
			<Gapped gap={10} vertical>
				<Link href={urlToRegister}>
					<Button width={200} use="primary" size="large" type="button" align="center">
					Зарегистрироваться</Button>
				</Link>
				<Link href={urlToEnter}><Button use="link">Войти</Button></Link>
			</Gapped>
		</div>
	);
}

Stub.propTypes = {
	courseId: PropTypes.string,
	slideId: PropTypes.string,
};

export default Stub;