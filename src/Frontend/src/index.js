import React from 'react';
import ReactDOM from 'react-dom';
import UlearnApp from './App';
import registerServiceWorker from './registerServiceWorker';

ReactDOM.render((
    <UlearnApp />
), document.getElementById('root'));

registerServiceWorker();
