import React, { Component } from 'react';
import { Switch, Route, BrowserRouter } from 'react-router-dom';

import AnyPage from "./pages/AnyPage";
import ErrorBoundary from "./components/common/ErrorBoundary";
import YandexMetrika from "./components/common/YandexMetrika";
import Header from "./components/common/Header";
import { Provider, connect } from "react-redux";
import thunkMiddleware from "redux-thunk"
import { createLogger } from "redux-logger"
import { applyMiddleware, createStore } from "redux";

import "./common.less"
import "./App.less"

import rootReducer from "./redux/reducers"
import api from "./api"

let loggerMiddleware = createLogger();

function configureStore(preloadedState) {
    return createStore(
        rootReducer,
        preloadedState,
        applyMiddleware(
            thunkMiddleware,
            loggerMiddleware
        )
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


class UlearnApp extends Component {
    render() {
        return (
            <Provider store={store}>
                <InternalUlearnApp />
            </Provider>
        )
    }
}

class InternalUlearnApp extends Component {
    constructor(props) {
        super(props);
        this.state = {
            initializing: true
        }
    }

    componentDidMount() {
        this.props.getCurrentUser();
        this.props.getCourses();
    }

    componentWillReceiveProps(nextProps, nextState) {
        this.setState({
            initializing: false
        })
        if (! this.props.account.isAuthenticated && nextProps.account.isAuthenticated) {
            this.props.getNotificationsCount();
        }
    }

    render() {
        return (
            <BrowserRouter>
                <ErrorBoundary>
                    <Header initializing={this.state.initializing}/>
                    <div className="header-content-divider" />
                    <Switch>
                        <Route component={AnyPage} />
                    </Switch>
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
