import PropTypes from 'prop-types';
import { SLIDETYPE } from '../../../consts/general';

export const menuItemType = {
	title: PropTypes.string,
	url: PropTypes.string,
	type: PropTypes.oneOf(Object.values(SLIDETYPE)),
	score: PropTypes.number,
	maxScore: PropTypes.number,
	description: PropTypes.string,
	isActive: PropTypes.bool,
	visited: PropTypes.bool,
	metro: PropTypes.shape({
		complete: PropTypes.bool,
		isFirstItem: PropTypes.bool,
		isLastItem: PropTypes.bool,
		connectToPrev: PropTypes.bool,
		connectToNext: PropTypes.bool,
	}),

	toggleNavigation: PropTypes.func,
};


export const courseMenuItemType = {
	id: PropTypes.string,
	title: PropTypes.string,
	progress: PropTypes.number,
	isActive: PropTypes.bool,
};