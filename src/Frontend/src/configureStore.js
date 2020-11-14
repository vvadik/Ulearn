import { applyMiddleware, compose, createStore } from "redux";
import thunkMiddleware from "redux-thunk";
import rootReducer from "src/redux/reducers";

export default function configureStore() {
	let env = process.env.NODE_ENV || 'development';
	let isDevelopment = env === 'development';

	let middlewares;
	if(isDevelopment) {
		const composeEnhancers = window.__REDUX_DEVTOOLS_EXTENSION_COMPOSE__ || compose;
		middlewares = composeEnhancers(applyMiddleware(thunkMiddleware));

	} else {
		middlewares = applyMiddleware(thunkMiddleware);
	}

	return createStore(
		rootReducer,
		middlewares
	)
}
