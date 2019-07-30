import React, { Component } from "react";
import PropTypes from "prop-types";
import { connect } from "react-redux";

import CourseNavigationHeader from "../CourseNavigationHeader";
import CourseNavigationContent from "../CourseNavigationContent"
import { courseMenuItemType } from "../../types";
import Flashcards from "../Flashcards";
import { flashcards } from "../../../../../consts/routes";

import { toggleNavigation } from "../../../../../actions/navigation";

import styles from './CourseNavigation.less';


class CourseNavigation extends Component {
	componentDidMount() {
		document.getElementsByTagName('body')[0]
			.style.overflow = 'hidden';
	}

	componentWillUnmount() {
		document.getElementsByTagName('body')[0]
			.style.overflow = 'auto';
	}

	render() {
		const { title, description, items, courseId, slideId, containsFlashcards } = this.props;

		return (
			<aside className={ styles.root }>
				<div className={ styles.overlay } onClick={ this.props.toggleNavigation }/>
				<div className={ styles.contentWrapper }>
					<CourseNavigationHeader title={ title } description={ description }/>
					{ items && items.length && <CourseNavigationContent items={ items }/> }
					{ containsFlashcards &&
					<Flashcards courseId={ courseId } isActive={ slideId === flashcards }/> }
				</div>
			</aside>
		);
	}
}

CourseNavigation.propTypes = {
	title: PropTypes.string,
	description: PropTypes.string,
	items: PropTypes.arrayOf(PropTypes.shape(courseMenuItemType)),
	courseId: PropTypes.string,
	slideId: PropTypes.string,
	containsFlashcards: PropTypes.bool,

	toggleNavigation: PropTypes.func,
};

const mapStateToProps = (state) => {
	return {};
};

const mapDispatchToProps = (dispatch) => ({
	toggleNavigation: () => dispatch(toggleNavigation()),
});

export default connect(mapStateToProps, mapDispatchToProps)(CourseNavigation);
