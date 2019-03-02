import React, { Component, createContext } from 'react';
import PropTypes from "prop-types";
import Button from "@skbkontur/react-ui/components/Button/Button";

import styles from './Stub.less';

function Stub (props) {
	return (
		<div className={styles.stub}>
			<span className={styles.stubText}>Оставлять комментарии могут только зарегистрированные пользователи</span>
			<Button width={200} use="primary" size="large" type="button" align="center">Зарегистрироваться</Button>
			<Button use="link">Войти</Button>
		</div>
	);
}

export default Stub;