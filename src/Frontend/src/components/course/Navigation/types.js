import PropTypes from 'prop-types';
import { itemTypes } from './constants';

export const menuItemType = PropTypes.shape({
	text: PropTypes.string.isRequired,
	url: PropTypes.string,
	score: PropTypes.number,
	type: PropTypes.oneOf(Object.values(itemTypes)),
	description: PropTypes.string,
	isActive: PropTypes.bool,
	metro: PropTypes.shape({
		complete: PropTypes.bool,
		type: PropTypes.oneOf(Object.values(itemTypes)),
		isFirstItem: PropTypes.bool,
		isLastItem: PropTypes.bool,
		connectToPrev: PropTypes.bool,
		connectToNext: PropTypes.bool,
	})
});


export const courseMenuItemType = PropTypes.shape({
	text: PropTypes.string.isRequired,
	url: PropTypes.string,
	progress: PropTypes.number,
	isActive: PropTypes.bool,
});