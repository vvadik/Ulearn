import { AnyAction, combineReducers, Reducer, } from "redux";
import courseReducer from "./course";
import userProgressReducer from "./userProgress";
import navigationReducer from "./navigation";
import slidesReducer from "./slides";
import accountReducer from "./account";
import notificationsReducer from "./notifications";
import instructorReducer from "./instructor";
import deviceReducer from "./device";
import commentsReducer from "./comments";
import { ACCOUNT__USER_INFO_UPDATED } from "src/actions/account.types";

const rootReducer = combineReducers({
	account: accountReducer,
	courses: courseReducer,
	userProgress: userProgressReducer,
	notifications: notificationsReducer,
	navigation: navigationReducer,
	slides: slidesReducer,
	instructor: instructorReducer,
	device: deviceReducer,
	comments: commentsReducer,
});

type RootState = ReturnType<typeof rootReducer>

function resetReducer(state: RootState, action: AnyAction): RootState {
	const newState = { ...state };
	if(action.type === ACCOUNT__USER_INFO_UPDATED) {
		if(action.isAuthenticated && !newState.account.isAuthenticated) {
			newState.slides.slidesByCourses = {}; //deleting all loaded slides
		}
	}

	return rootReducer(newState, action);
}

export default resetReducer as Reducer;
export { RootState };
