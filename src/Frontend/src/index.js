import React from 'react';
import ReactDOM from 'react-dom';
import UlearnApp from './App';
import { BrowserRouter } from 'react-router-dom';
import registerServiceWorker from './registerServiceWorker';

ReactDOM.render((
    <BrowserRouter>
        <UlearnApp />
    </BrowserRouter>
), document.getElementById('root'));

registerServiceWorker();
