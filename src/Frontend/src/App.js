import React, {Component} from 'react';
import {BrowserRouter} from 'react-router-dom';

import ErrorBoundary from "./components/common/ErrorBoundary";
import NotFoundErrorBoundary from "./components/common/Error/NotFoundErrorBoundary";
import YandexMetrika from "./components/common/YandexMetrika";
import Header from "./components/common/Header";
import {Provider, connect} from "react-redux";
import thunkMiddleware from "redux-thunk";
import {createLogger} from "redux-logger";
import {applyMiddleware, createStore} from "redux";

import Router from "./Router";

import rootReducer from "./redux/reducers";
import api from "./api";
import Toast from "@skbkontur/react-ui/components/Toast/Toast";


let loggerMiddleware = createLogger();

function configureStore(preloadedState) {
	let env = process.env.NODE_ENV || 'development';
	let isDevelopment = env === 'development';

	let middlewares = isDevelopment ?
		applyMiddleware(thunkMiddleware, loggerMiddleware) :
		applyMiddleware(thunkMiddleware);

	return createStore(
		rootReducer,
		preloadedState,
		middlewares
	)
}

let store = configureStore({
	account: {
		isAuthenticated: false,
		isSystemAdministrator: false,
		roleByCourse: {},
		accessesByCourse: {},
	},
	notifications: {
		count: 0,
		lastTimestamp: undefined
	}
});

// Update notifications count each minute
setInterval(() => {
	if (store.getState().account.isAuthenticated)
		store.dispatch(api.notifications.getNotificationsCount(store.getState().notifications.lastTimestamp))
}, 60 * 1000);

api.setServerErrorHandler((message) => Toast.push(message ? message : 'Произошла ошибка. Попробуйте перезагрузить страницу.'));

class UlearnApp extends Component {
	render() {
		let pathname = window.location.pathname.toLowerCase();
		let isLti = pathname.endsWith('/ltislide') || pathname.endsWith('/acceptedalert'); //TODO remove this flag,that hiding header and nav menu
		let isHeaderVisible = !isLti;

		return (
			<Provider store={store}>
				<InternalUlearnApp isHeaderVisible={isHeaderVisible}/>
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
		if (!prevProps.account.isAuthenticated && this.props.account.isAuthenticated) {
			this.props.getNotificationsCount();
		}
	}

	render() {
		const isHeaderVisible = this.props.isHeaderVisible;
		return (
			<BrowserRouter>
				<ErrorBoundary>
					{isHeaderVisible &&
					<React.Fragment>
						<Header initializing={this.state.initializing}/>
					</React.Fragment>
					}
					<NotFoundErrorBoundary>
						{!this.state.initializing && // Avoiding bug: don't show page while initializing.
						// Otherwise we make two GET requests sequentially.
						// Unfortunately some our GET handlers are not idempotent (i.e. /Admin/CheckNextExerciseForSlide)
						<Router/>
						}
					</NotFoundErrorBoundary>
					<YandexMetrika/>
				</ErrorBoundary>
			</BrowserRouter>
		);
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
