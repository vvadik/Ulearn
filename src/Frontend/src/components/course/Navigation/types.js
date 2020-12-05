import PropTypes from 'prop-types';
import { SlideType } from 'src/models/slide';

export const menuItemType = {
	title: PropTypes.string,
	url: PropTypes.string,
	type: PropTypes.oneOf(Object.values(SlideType)),
	score: PropTypes.number,
	maxScore: PropTypes.number,
	description: PropTypes.string,
	isActive: PropTypes.bool,
	visited: PropTypes.bool,
	hide: PropTypes.bool,
	metro: PropTypes.shape({
		complete: PropTypes.bool,
		isFirstItem: PropTypes.bool,
		isLastItem: PropTypes.bool,
		connectToPrev: PropTypes.bool,
		connectToNext: PropTypes.bool,
	}),

	toggleNavigation: PropTypes.func,
};

export const progressType = {
	current: PropTypes.number,
	max: PropTypes.number,
};

export const courseMenuItemType = {
	id: PropTypes.string,
	title: PropTypes.string,
	progress: PropTypes.shape(progressType),
	isActive: PropTypes.bool,
	isNotPublished: PropTypes.bool,
	publicationDate: PropTypes.string,
};

export const groupAsStudentType = {
	id: PropTypes.number,
	courseId: PropTypes.string,
	name: PropTypes.string,
	isArchived: PropTypes.bool,
	apiUrl: PropTypes.string,
};
