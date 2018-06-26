import React, { Component } from 'react';
import './App.css';
import { Switch, Route } from 'react-router-dom';

import AnyPage from "./pages/AnyPage";
import ErrorBoundary from "./components/common/ErrorBoundary";
import YandexMetrika from "./components/common/YandexMetrika";

class UlearnApp extends Component {
  render() {
    return (
        <ErrorBoundary>
            <Switch>
                <Route component={AnyPage} />
            </Switch>
            <YandexMetrika/>
        </ErrorBoundary>
    );
  }
}

export default UlearnApp;
