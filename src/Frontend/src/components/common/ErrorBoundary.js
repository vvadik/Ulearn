import React, { Component } from "react";

import styles from './Header.less';
import { Toast } from "ui";
import Raven from 'raven-js';

class ErrorBoundary extends Component {
	constructor(props) {
		super(props);
		this.state = {error: null};
	}

	componentDidCatch(error, errorInfo) {
		this.setState({error});
		Raven.captureException(error, {extra: errorInfo});
	}

	render() {
		if (this.state.error) {
			/* render fallback UI */
			return (
				<React.Fragment>
					<div
						className={styles["error"]}
						onClick={() => Raven.lastEventId() && Raven.showReportDialog()}>
						<p>We're sorry — something's gone wrong.</p>
						<p>Our team has been notified, but click here fill out a report.</p>
					</div>
					{Toast.push('Произошла ошибка. Попробуйте перезагрузить страницу.')}
				</React.Fragment>
			);
		}
		/* when there's not an error, render children untouched */
		return this.props.children;
	}
}

export default ErrorBoundary