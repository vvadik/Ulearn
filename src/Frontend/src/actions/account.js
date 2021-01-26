import {
	ACCOUNT__USER_HIJACK,
} from 'src/consts/actions';

const userProgressHijackAction = (isHijacked) => ({
	type: ACCOUNT__USER_HIJACK,
	isHijacked,
});

export const setHijack = (isHijacked) => {
	return (dispatch) => {
		dispatch(userProgressHijackAction(isHijacked));
	}
}
