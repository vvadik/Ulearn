import React, { Component } from 'react';
import { Switch, Route, BrowserRouter } from 'react-router-dom';

import AnyPage from "./pages/AnyPage";
import ErrorBoundary from "./components/common/ErrorBoundary";
import YandexMetrika from "./components/common/YandexMetrika";
import Header from "./components/common/Header";
import { Provider } from "react-redux";
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
        roleByCourse: {}
    },
    notifications: {
        count: 0,
    }
});
store.dispatch(api.account.getCurrentUser());
store.dispatch(api.courses.getCourses());
store.dispatch(api.notifications.getNotificationsCount());

class UlearnApp extends Component {
  render() {
    return (
        <Provider store={store}>
            <BrowserRouter>
                <ErrorBoundary>
                    <Header/>
                    <div className="header-content-divider" />
                    <Switch>
                        <Route component={AnyPage} />
                    </Switch>
                    <YandexMetrika/>
                </ErrorBoundary>
            </BrowserRouter>
        </Provider>
    );
  }
}

export default UlearnApp;
