import React, { Component } from "react";

import api from "src/api";

import { Tabs } from "ui";
import CommentsList from "../CommentsList/CommentsList";

import { isInstructor, UserInfo, } from "src/utils/courseRoles";

import { TabsType } from "src/consts/tabsType";
import { SlideType } from "src/models/slide";
import { Comment, CommentPolicy } from "src/models/comments";

import styles from "./CommentsView.less";


interface Props {
	courseId: string;
	slideId: string;
	slideType: SlideType;

	user: UserInfo;

	commentsApi: typeof api.comments;

	openInstructorsComments?: boolean;
	isSlideReady: boolean;
}

interface State {
	commentPolicy: CommentPolicy | null;
	activeTab: TabsType;
	instructorsCommentCount: number;
	instructorsComments: Comment[];

	openModal: boolean;
	tabHasAutomaticallyChanged: boolean;
}

class CommentsView extends Component<Props, State> {
	private headerRef: React.RefObject<HTMLDivElement> = React.createRef();

	constructor(props: Props) {
		super(props);

		this.state = {
			instructorsComments: [],
			commentPolicy: null,
			activeTab: this.props.openInstructorsComments ? TabsType.instructorsComments : TabsType.allComments,
			openModal: false,
			instructorsCommentCount: 0,
			tabHasAutomaticallyChanged: false,
		};
	}

	static defaultProps = {
		commentsApi: api.comments,
	};

	componentDidMount(): void {
		const { courseId, slideId, user, } = this.props;

		this.loadCommentPolicy(courseId);

		if(isInstructor(user)) {
			this.loadComments(courseId, slideId);
		}
	}

	componentDidUpdate(prevProps: Props): void {
		const { slideId, courseId, user, } = this.props;

		if(slideId !== prevProps.slideId) {
			if(isInstructor(user)) {
				this.loadComments(courseId, slideId);
			}
		}
	}

	loadCommentPolicy = (courseId: string): void => {
		const { commentsApi, } = this.props;

		commentsApi.getCommentPolicy(courseId)
			.then(commentPolicy => {
				this.setState({
					commentPolicy: commentPolicy,
				});
			})
			.catch(console.error);
	};

	loadComments = (courseId: string, slideId: string): void => {
		const { commentsApi, } = this.props;

		commentsApi.getComments(courseId, slideId, true)
			.then(json => {
				const comments = json.topLevelComments;
				this.setState({
					instructorsComments: comments,
					instructorsCommentCount: comments.length,
				});
			})
			.catch(console.error);
	};

	render(): React.ReactElement {
		const { user, courseId, slideId, slideType, commentsApi, isSlideReady, } = this.props;
		const { activeTab, commentPolicy, } = this.state;

		return (
			<div className={ styles.wrapper }>
				{ this.renderHeader() }
				<div key={ activeTab }>
					<CommentsList
						slideType={ slideType }
						handleInstructorsCommentCount={ this.handleInstructorsCommentCount }
						handleTabChange={ this.handleTabChange }
						headerRef={ this.headerRef }
						forInstructors={ activeTab === TabsType.instructorsComments }
						commentsApi={ commentsApi }
						commentPolicy={ commentPolicy }
						user={ user }
						slideId={ slideId }
						courseId={ courseId }
						isSlideReady={ isSlideReady }>
					</CommentsList>
				</div>
			</div>
		);
	}

	renderHeader(): React.ReactElement {
		const { user, } = this.props;
		const { activeTab, instructorsCommentCount, } = this.state;

		return (
			<header className={ styles.header } ref={ this.headerRef }>
				{ isInstructor(user) &&
				<div className={ styles.tabs }>
					<Tabs value={ activeTab } onValueChange={ this.handleTabChangeByUser }>
						<Tabs.Tab id={ TabsType.allComments }>К слайду</Tabs.Tab>
						<Tabs.Tab id={ TabsType.instructorsComments }>
							Для преподавателей
							{ instructorsCommentCount > 0 &&
							<span className={ styles.commentsCount }>{ instructorsCommentCount }</span> }
						</Tabs.Tab>
						{ activeTab === TabsType.instructorsComments &&
						<span className={ styles.textForInstructors }> {/*TODO DO NOT SHOW ON MOBILE*/ }
							Студенты не видят эти комментарии
						</span> }
					</Tabs>
				</div> }
			</header>
		);
	}

	handleTabChangeByUser = (id: string): void =>
		this.handleTabChange(id as TabsType, true);

	handleTabChange = (id: TabsType, isUserAction: boolean): void => {
		const { user, } = this.props;
		const { activeTab, tabHasAutomaticallyChanged, } = this.state;

		if(isInstructor(user)) {
			if(!isUserAction && tabHasAutomaticallyChanged) {
				return;
			}

			if(id !== activeTab) {
				this.setState({
					activeTab: id,
				});
			}

			if(!isUserAction) {
				this.setState({
					tabHasAutomaticallyChanged: true
				});
			}
		}
	};

	handleInstructorsCommentCount = (action: 'add' | string): void => {
		const { instructorsCommentCount, } = this.state;

		if(action === "add") {
			this.setState({
				instructorsCommentCount: instructorsCommentCount + 1,
			});
		} else {
			this.setState({
				instructorsCommentCount: instructorsCommentCount - 1,
			});
		}
	};
}

export default CommentsView;
