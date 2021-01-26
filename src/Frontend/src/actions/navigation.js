import { NAVIGATION__TOGGLE } from "src/consts/actions";

const toggleNavigationAction = () => ({
	type: NAVIGATION__TOGGLE,
});


export const toggleNavigation = () => {
	return (dispatch) => {
		dispatch(toggleNavigationAction());
	}
};
