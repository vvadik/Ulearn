import React from 'react';
import ReactDOM from 'react-dom';
import UlearnApp from './App';
// import registerServiceWorker from './registerServiceWorker';

ReactDOM.render((
    <UlearnApp />
), document.getElementById('root'));

// registerServiceWorker();

function unregisterServiceWorker() {
	if (window.navigator && navigator.serviceWorker) {
		navigator.serviceWorker.getRegistrations()
			.then(function (registrations) {
				for (let registration of registrations) {
					registration.unregister();
				}
			});
	}
}

unregisterServiceWorker();