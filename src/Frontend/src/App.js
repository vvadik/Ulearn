import React, { Component } from 'react';
import { Switch, Route } from 'react-router-dom';

import AnyPage from "./pages/AnyPage";
import ErrorBoundary from "./components/common/ErrorBoundary";
import YandexMetrika from "./components/common/YandexMetrika";
import Header from "./components/common/Header";

class UlearnApp extends Component {
  render() {
    return (
        <ErrorBoundary>
            <Header isAuthenticated={false}/>
            <Switch>
                <Route component={AnyPage} />
            </Switch>
            <YandexMetrika/>
        </ErrorBoundary>
    );
  }
}

export default UlearnApp;
