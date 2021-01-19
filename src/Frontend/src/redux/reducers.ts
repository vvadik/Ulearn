import { combineReducers } from "redux";
import courseReducer from "./course.js";
import userProgressReducer from "./userProgress";
import navigationReducer from "./navigation.js";
import slidesReducer from "./slides.js";
import accountReducer from "./account.js";
import notificationsReducer from "./notifications.js";
import instructorReducer from "./instructor.js";

const rootReducer = combineReducers({
	account: accountReducer,
	courses: courseReducer,
	userProgress: userProgressReducer,
	notifications: notificationsReducer,
	navigation: navigationReducer,
	slides: slidesReducer,
	instructor: instructorReducer,
});

export default rootReducer;
