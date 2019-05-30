import React, { Component } from "react";
import PropTypes from "prop-types";
import Navigation from "../Navigation";
import AnyPage from '../../../pages/AnyPage'
import styles from "./Course.less"



class Course extends Component {
	constructor(props) {
		super(props);

		this.state = {
			progressLoad: false,
			showCourseNav: false,
		};
	}

	componentDidMount () {
		const { loadCourse, loadUserProgress, isAuthenticated, courseId, courseInfo, progress } = this.props;

		if (!courseInfo) {
			loadCourse(courseId);
		}

		if (progress) {
			this.setState({
				showCourseNav: Object.keys(progress).length > 0,
			});
		}

		if (isAuthenticated && !progress) {
			loadUserProgress(courseId);
		}

		if (!isAuthenticated) {
			// загрузить из localStorage
		}
	}

	static getDerivedStateFromProps (props, state) {
		if (!state.progressLoad && props.progress) {
			return {
				progressLoad: true,
				showCourseNav: Object.keys(props.progress).length > 0,
			};
		}
	}

	render () {
		const { courseInfo, progress } = this.props;

		if (!courseInfo) {
			return null;
		}

		return (
			<div className={ styles.root }>
				<Navigation
					progress={0.5}
					courseName={ courseInfo.title }
					title={ courseInfo.units[0].title }
					items={ courseInfo.units[0].slides } />
				<div>
					<AnyPage />
				</div>
			</div>
		);
	}
}

Course.propTypes = {
	isAuthenticated: PropTypes.bool,
	courseId: PropTypes.string,
	slideId: PropTypes.string,
	courseInfo: PropTypes.object, // TODO: описать
	progress: PropTypes.object, // TODO: описать
	loadCourse: PropTypes.func,
	loadUserProgress: PropTypes.func,
};

export default Course;