import React, { Component } from 'react';
import logo from './logo.svg';
import './App.css';
import Header from './components/common/Header';
import { Switch, Route } from 'react-router-dom';

import HomeController from './pages/home'

class UlearnApp extends Component {
  render() {
    return (
      <div className="App">
        <Header />
        <p className="App-intro">
          To get started, edit <code>src/App.js</code> and save to reload.
        </p>
        <Switch>
            <Route exact path="/" component={HomeController} />
        </Switch>
      </div>
    );
  }
}

export default UlearnApp;
