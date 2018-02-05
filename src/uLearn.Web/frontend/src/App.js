import React, { Component } from 'react';
import logo from './logo.svg';
import './App.css';
import Button from '@skbkontur/react-ui/Button';
import { Switch, Route } from 'react-router-dom';

import HomeController from './pages/home'

class UlearnApp extends Component {
  render() {
    return (
      <div className="App">
        <header className="App-header">
          <img src={logo} className="App-logo" alt="logo" />
          <h1 className="App-title">Welcome to React</h1>
        </header>
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
