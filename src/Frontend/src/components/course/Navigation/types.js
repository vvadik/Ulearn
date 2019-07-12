import PropTypes from 'prop-types';
import { itemTypes } from './constants';

export const menuItemType = {
	title: PropTypes.string,
	url: PropTypes.string,
	type: PropTypes.oneOf(Object.values(itemTypes)),
	score: PropTypes.number,
	maxScore: PropTypes.number,
	description: PropTypes.string,
	isActive: PropTypes.bool,
	metro: PropTypes.shape({
		complete: PropTypes.bool,
		isFirstItem: PropTypes.bool,
		isLastItem: PropTypes.bool,
		connectToPrev: PropTypes.bool,
		connectToNext: PropTypes.bool,
	})
};


export const courseMenuItemType = {
	id: PropTypes.string,
	title: PropTypes.string,
	progress: PropTypes.number,
	isActive: PropTypes.bool,
};