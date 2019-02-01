import React, { Component } from 'react';
import connect from "react-redux/es/connect/connect";
import PropTypes from "prop-types";
import api from "../../api/index";
import Loader from "@skbkontur/react-ui/components/Loader/Loader";
import CommentSendForm from "../CommentSendForm";

import styles from "../comment.less";

class CommentsList extends Component {
	state = {
		comments: [],
		loading: false,
	};

	componentDidMount() {
		const slideId = this.props.match.params.slideId;

		this.props.enterToCourse(this.courseId, slideId);
		this.loadComments(this.courseId, slideId, this.isInstructor);
	}

	get courseId() {
		return this.props.match.params.courseId.toLowerCase();
	}

	get role() {
		return this.props.account.roleByCourse[this.courseId];
	}

	_instructor = null;
	get isInstructor() {
		if (this._instructor === null) {
			this._instructor = this.props.account.isSystemAdministrator ||
				['courseAdmin', 'instructor'].includes(this.role);
		}
		return this._instructor;
	}

	loadComments = (courseId, slideId) => {
		this.setState({ sending: true });

		api.comments.getComments(courseId, slideId)
			.then(json => {
				let comments = json.comments;
				this.setState({
					comments: comments,
					sending: false,
				})
			})
			.catch(console.error)
			.finally(this.setState({ sending: false })
			)
	};

	render() {
		return (
			<Loader active={this.state.loading}>
				<h1 className={styles.header}>Комментарии</h1>
				<CommentSendForm />
				<div>Здксь будет список комментариев</div>
			</Loader>
		)
	}

	static mapStateToProps(state) {
		return {
			account: state.account,
		}
	};

	static mapDispatchToProps(dispatch) {
		return {
			enterToCourse: (courseId, slideId) => dispatch({
				type: 'COURSES__COURSE_ENTERED',
				courseId: courseId,
				slideId: slideId,
			}),
		}
	};
}

CommentsList.propTypes = {
	account: PropTypes.object,
	match: PropTypes.object,
	enterToCourse: PropTypes.func,
};

CommentsList = connect(CommentsList.mapStateToProps, CommentsList.mapDispatchToProps)(CommentsList);
export default CommentsList;
