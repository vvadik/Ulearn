import React from "react";
import PropTypes from "prop-types";
import Link from "@skbkontur/react-ui/components/Link/Link";
import Button from "@skbkontur/react-ui/components/Button/Button";
import Gapped from "@skbkontur/react-ui/components/Gapped/Gapped";
import StubForEmptyComments from "./StubForEmptyComments";

import styles from "./Stub.less";

function StubForUnauthorizedUser(props) {
	const {courseId, slideId, hasThreads} = props;
	const urlToRegister = `${window.location.origin}/Account/Register?returnUrl=/Course/${courseId}/${slideId}`;
	const urlToEnter = `${window.location.origin}/Login?returnUrl=/Course/${courseId}/${slideId}`;

	return (
		<div className={styles.stub}>
			{!hasThreads && <StubForEmptyComments />}
			<span className={styles.textForUnauthorizedUser}>
				Оставлять комментарии могут только зарегистрированные пользователи
			</span>
			<Gapped gap={10} vertical>
				<Link href={urlToRegister}>
					<Button width={200} use="primary" size="large" type="button" align="center">
						Зарегистрироваться</Button>
				</Link>
				<Link href={urlToEnter}>Войти</Link>
			</Gapped>
		</div>
	);
}

StubForUnauthorizedUser.propTypes = {
	courseId: PropTypes.string,
	slideId: PropTypes.string,
};

export default StubForUnauthorizedUser;