import React, { Component, ErrorInfo } from "react";

import { Warning } from "icons";
import { Toast } from "ui";

import Raven from 'raven-js';
import cn from "classnames";

import styles from './style.less';

interface Props {
	className?: string,
}

interface State {
	error: Error | null,
}

class HeaderComponentErrorBoundary extends Component<Props, State> {
	state: State = {
		error: null,
	};

	componentDidCatch(error: Error, errorInfo: ErrorInfo): void {
		this.setState({ error });
		Raven.captureException(error, { extra: errorInfo });
		Toast.push('Произошла ошибка. Попробуйте перезагрузить страницу.');
	}

	render(): React.ReactNode {
		const { error } = this.state;
		const { className } = this.props;

		if(error) {
			return (
				<div className={ cn(styles.headerError, className || '') }
					 onClick={ this.showRavenReportDialog }>
					<Warning color="#f77" size={ 20 }/>
				</div>
			);
		}
		return this.props.children;
	}

	showRavenReportDialog = (): void => {
		Raven.lastEventId();
		Raven.showReportDialog();
	};
}

export default HeaderComponentErrorBoundary;
