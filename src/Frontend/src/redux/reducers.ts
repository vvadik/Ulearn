import { combineReducers } from "redux";
import courseReducer from "./course";
import userProgressReducer from "./userProgress";
import navigationReducer from "./navigation";
import slidesReducer from "./slides";
import accountReducer from "./account";
import notificationsReducer from "./notifications";
import instructorReducer from "./instructor";
import deviceReducer from "./device";

const rootReducer = combineReducers({
	account: accountReducer,
	courses: courseReducer,
	userProgress: userProgressReducer,
	notifications: notificationsReducer,
	navigation: navigationReducer,
	slides: slidesReducer,
	instructor: instructorReducer,
	device: deviceReducer,
});

export default rootReducer;
