import React, { Component } from 'react';
import { BrowserRouter } from 'react-router-dom';

import ErrorBoundary from "./components/common/ErrorBoundary";
import NotFoundErrorBoundary from "./components/common/Error/NotFoundErrorBoundary";
import YandexMetrika from "./components/common/YandexMetrika";
import Header from "./components/common/Header";
import EmailNotConfirmedModal from "src/components/notificationModal/EmailNotConfirmedModal";

import { emailNotConfirmed } from "src/consts/accountProblems";

import { Provider, connect } from "react-redux";

import Router from "./Router";

import api from "./api";
import { ThemeContext, Toast } from "ui";
import theme from "src/uiTheme";
import queryString from "query-string";
import configureStore from "src/configureStore";



const store = configureStore();

// Update notifications count each minute
setInterval(() => {
	if(store.getState().account.isAuthenticated)
		store.dispatch(api.notifications.getNotificationsCount(store.getState().notifications.lastTimestamp))
}, 60 * 1000);

api.setServerErrorHandler((message) => Toast.push(message ? message : 'Произошла ошибка. Попробуйте перезагрузить страницу.'));

class UlearnApp extends Component {
	render() {
		return (
			<Provider store={ store }>
				<InternalUlearnApp/>
			</Provider>
		)
	}
}

class InternalUlearnApp extends Component {
	constructor(props) {
		super(props);
		this.state = {
			initializing: true,
		}
	}

	componentDidMount() {
		this.props.getCurrentUser();
		this.props.getCourses();
		this.setState({
			initializing: false
		});
	}

	componentDidUpdate(prevProps) {
		if(!prevProps.account.isAuthenticated && this.props.account.isAuthenticated) {
			this.props.getNotificationsCount();
		}
	}

	render() {
		const pathname = window.location.pathname.toLowerCase();
		const params = queryString.parse(window.location.search);
		const isLti = pathname.endsWith('/ltislide') || pathname.endsWith('/acceptedalert') || params.isLti; //TODO remove this flag,that hiding header and nav menu
		const isHeaderVisible = !isLti;

		return (
			<BrowserRouter>
				<ThemeContext.Provider value={ theme }>
					<ErrorBoundary>
						{ isHeaderVisible &&
						<React.Fragment>
							<Header initializing={ this.state.initializing }/>
						</React.Fragment>
						}
						<NotFoundErrorBoundary>
							{ !this.state.initializing && // Avoiding bug: don't show page while initializing.
							// Otherwise we make two GET requests sequentially.
							// Unfortunately some our GET handlers are not idempotent (i.e. /Admin/CheckNextExerciseForSlide)
							<Router account={ this.props.account }/>
							}
						</NotFoundErrorBoundary>
						{ this.props.account
						&& this.isEmailNotConfirmed()
						&& <EmailNotConfirmedModal account={ this.props.account }/>
						}
						<YandexMetrika/>
					</ErrorBoundary>
				</ThemeContext.Provider>
			</BrowserRouter>
		);
	}

	isEmailNotConfirmed = () => {
		const { account } = this.props;
		return account.isAuthenticated
			&& account.accountProblems.length > 0
			&& account.accountProblems.some(p => p.problemType === emailNotConfirmed);
	}

	static mapStateToProps(state) {
		return {
			account: state.account,
		}
	}

	static mapDispatchToProps(dispatch) {
		return {
			getCurrentUser: () => dispatch(api.account.getCurrentUser()),
			getCourses: () => dispatch(api.courses.getCourses()),
			getNotificationsCount: () => dispatch(api.notifications.getNotificationsCount())
		}
	}
}

InternalUlearnApp = connect(InternalUlearnApp.mapStateToProps, InternalUlearnApp.mapDispatchToProps)(InternalUlearnApp);

export default UlearnApp;
