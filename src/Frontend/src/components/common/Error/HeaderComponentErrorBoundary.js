import React, {Component} from "react";

import { Warning } from "icons/Warning";
import { Toast } from "ui";
import Raven from 'raven-js';

import styles from './style.less';

class HeaderComponentErrorBoundary extends Component {
	state = {
		error: null,
	};

	componentDidCatch(error, errorInfo) {
		this.setState({error});
		Raven.captureException(error, {extra: errorInfo});
		Toast.push('Произошла ошибка. Попробуйте перезагрузить страницу.');
	}

	render() {
		if (this.state.error) {
			const additionalClassName = this.props.className ? this.props.className : '';
			return (
				<div className={`${styles.headerError} ${additionalClassName}`} onClick={this.showRavenReportDialog}>
					<Warning color="#f77" size={20}/>
				</div>
			);
		}
		return this.props.children;
	}

	showRavenReportDialog = () => {
		Raven.lastEventId();
		Raven.showReportDialog();
	}
}

export default HeaderComponentErrorBoundary;