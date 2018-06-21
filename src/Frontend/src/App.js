import React, { Component } from 'react';
import './App.css';
import { Switch, Route } from 'react-router-dom';

import AnyPage from "./pages/AnyPage";

class UlearnApp extends Component {
  render() {
    return (
      // <div className="App">
      //   <Header />
        <Switch>
            <Route component={AnyPage} />
        </Switch>
      // </div>
    );
  }
}

export default UlearnApp;
