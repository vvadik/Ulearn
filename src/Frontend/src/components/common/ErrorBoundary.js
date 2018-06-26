import React, { Component } from "react";

/* It's error boundary for React 16 and above. It supports Raven from sentry.io.
 * See https://github.com/getsentry/raven-js/blob/master/docs/integrations/react.rst and
 * https://reactjs.org/blog/2017/07/26/error-handling-in-react-16.html for details */

const Raven = window.Raven;

class ErrorBoundary extends Component {
    constructor(props) {
        super(props);
        this.state = { error: null };
    }

    componentDidCatch(error, errorInfo) {
        this.setState({ error });
        Raven.captureException(error, { extra: errorInfo });
    }

    render() {
        if (this.state.error) {
            //render fallback UI
            return (
                <div
                    className="error"
                    onClick={() => Raven.lastEventId() && Raven.showReportDialog()}>
                    <p>We're sorry — something's gone wrong.</p>
                    <p>Our team has been notified, but click here fill out a report.</p>
                </div>
            );
        } else {
            //when there's not an error, render children untouched
            return this.props.children;
        }
    }
}

export default ErrorBoundary