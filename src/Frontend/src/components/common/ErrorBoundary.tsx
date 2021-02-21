import React, { Component, ErrorInfo } from "react";

import { Toast } from "ui";
import * as Sentry from "@sentry/react";

interface State {
	error: Error | null;
}

class ErrorBoundary extends Component<unknown, State> {
	constructor(props: unknown) {
		super(props);
		this.state = { error: null };
	}

	componentDidCatch(error: Error, errorInfo: ErrorInfo): void {
		this.setState({ error });
		Sentry.captureException(error, { extra: { ...errorInfo } });
	}

	render(): React.ReactNode {
		if(this.state.error) {
			/* render fallback UI */
			return (
				<React.Fragment>
					<div
						onClick={ this.onClick }>
						<p>We're sorry — something's gone wrong.</p>
						<p>Our team has been notified, but click here fill out a report.</p>
					</div>
					{ Toast.push('Произошла ошибка. Попробуйте перезагрузить страницу.') }
				</React.Fragment>
			);
		}
		/* when there's not an error, render children untouched */
		return this.props.children;
	}

	onClick = (): void => {
		Sentry.showReportDialog();
	};
}

export default ErrorBoundary;
