import {
	NAVIGATION__TOGGLE,
} from 'src/consts/actions';

const initialState = {
	opened: false,
};

export default function navigation(state = initialState, action) {
	switch (action.type) {
		case NAVIGATION__TOGGLE:
			return {
				...state,
				opened: !state.opened,
			};
		default:
			return state;
	}
}
