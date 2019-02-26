import React, { Component } from 'react';
import connect from "react-redux/es/connect/connect";
import PropTypes from "prop-types";
import api from "./../api/index";
import Loader from "@skbkontur/react-ui/components/Loader/Loader";
import CommentSendForm from "../components/comments/CommentSendForm/CommentSendForm";
import {COURSES__COURSE_ENTERED} from "../consts/actions";

import styles from "../components/comments/CommentSendForm/CommentSendForm.less";

class CommentsList extends Component {
	state = {
		comments: [],
		loading: false,
	};

	get role() {
		const courseId = this.props.enterToCourse.courseId;
		return this.props.account.roleByCourse[courseId];
	}

	_instructor = null;
	get isInstructor() {
		if (this._instructor === null) {
			this._instructor = this.props.account.isSystemAdministrator ||
				['courseAdmin', 'instructor'].includes(this.role);
		}
		return this._instructor;
	}

	componentDidMount() {
		const slideId = this.props.match.params.slideId;

		this.props.enterToCourse(this.courseId, slideId);
		this.loadComments(this.courseId, slideId, this.isInstructor);
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
				<CommentSendForm onSubmit={this.onSubmit}/>
				<div>Здесь будет список комментариев</div>
			</Loader>
		)
	}

	onSubmit() {
		const text = this.state.text;
		const replyTo = 0;
		const { slideId, courseId } = this.props.enterToCourse.slideId;

		this.setState({ loading: true });
		api.comments.addComment(courseId, slideId, text, replyTo, this.isInstructor)
			.catch(console.error)
			.finally(this.setState({ loading: false }));
	}
}

function mapStateToProps(state) {
	return {
		account: state.account,
	}
}

function mapDispatchToProps(dispatch, state) {
	return {
		enterToCourse: (text) => dispatch({
			type: COURSES__COURSE_ENTERED,
			courseId: state.courseId,
			slideId: state.slideId,
		}),
	}
}

CommentsList.propTypes = {
	account: PropTypes.object,
	match: PropTypes.object,
	enterToCourse: PropTypes.func,
};

CommentsList = connect(mapStateToProps, mapDispatchToProps)(CommentsList);
export default CommentsList;
