import { NAVIGATION__TOGGLE } from "../consts/actions";

const toggleNavigationAction = () => ({
	type: NAVIGATION__TOGGLE,
});


export const toggleNavigation = () => {
	return (dispatch) => {
		dispatch(toggleNavigationAction());
	}
};