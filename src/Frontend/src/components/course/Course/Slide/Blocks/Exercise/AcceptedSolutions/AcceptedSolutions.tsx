import React from "react";
import { Modal, Tabs, Button, Hint } from "@skbkontur/react-ui";

import texts from "./AcceptedSolutions.texts";
import {
	AcceptedSolution,
	AcceptedSolutionsResponse,
	LikedAcceptedSolutionsResponse
} from "src/models/acceptedSolutions";
import { ShortUserInfo } from "src/models/users";
import StaticCode from "../StaticCode";
import { AcceptedSolutionsApi } from "src/api/acceptedSolutions";

import { Heart, Star } from 'icons';
import styles from './AcceptedSolutions.less';

interface AcceptedSolutionsProps {
	courseId: string,
	slideId: string,
	user: ShortUserInfo,
	isInstructor: boolean,
	onClose: () => void,
	acceptedSolutionsApi: AcceptedSolutionsApi,
}

enum TabsType {
	instructorTab = 'instructorTab',
	studentTab = 'studentTab',
}

interface _AcceptedSolution extends AcceptedSolution {
	promoted: boolean
}

interface State {
	loading: boolean,
	solutions: { [id: number]: _AcceptedSolution },
	promotedSolutions: number[],
	randomLikedSolutions: number[],
	newestSolutions: number[],
	likedAcceptedSolutions: number[] | null,
	activeTab: TabsType;
}

const LikedAcceptedSolutionsCount = 30;

class AcceptedSolutionsModal extends React.Component<AcceptedSolutionsProps, State> {
	constructor(props: AcceptedSolutionsProps) {
		super(props);
		this.state = {
			loading: true,
			solutions: {},
			promotedSolutions: [],
			randomLikedSolutions: [],
			newestSolutions: [],
			likedAcceptedSolutions: [],
			activeTab: props.isInstructor ? TabsType.instructorTab : TabsType.studentTab
		};
	}

	componentDidMount() {
		this.fetchContentFromServer();
	}

	fetchContentFromServer(afterLoad?: () => void) {
		const { courseId, slideId, isInstructor, acceptedSolutionsApi } = this.props;
		const getAcceptedSolutionsPromise = acceptedSolutionsApi.getAcceptedSolutions(courseId, slideId);
		if(!isInstructor) {
			getAcceptedSolutionsPromise
				.then(acceptedSolutionsResponse => {
					this.updateStateWithData(acceptedSolutionsResponse, null);
					if(afterLoad != null) {
						afterLoad();
					}
				})
				.catch(this.processErrorAndClose);
		} else {
			const getLikedAcceptedSolutionsPromise
				= acceptedSolutionsApi.getLikedAcceptedSolutions(courseId, slideId, 0, LikedAcceptedSolutionsCount);
			Promise.all([getAcceptedSolutionsPromise, getLikedAcceptedSolutionsPromise])
				.then(result => {
					const [acceptedSolutionsResponse, likedAcceptedSolutionsResponse] = result;
					this.updateStateWithData(acceptedSolutionsResponse, likedAcceptedSolutionsResponse);
					if(afterLoad != null) {
						afterLoad();
					}
				})
				.catch(this.processErrorAndClose);
		}
	}

	updateStateWithData(acceptedSolutionsResponse: AcceptedSolutionsResponse,
		likedAcceptedSolutionsResponse: LikedAcceptedSolutionsResponse | null
	) {
		const { promotedSolutions, randomLikedSolutions, newestSolutions } = acceptedSolutionsResponse;
		let solutions = newestSolutions.concat(randomLikedSolutions).concat(promotedSolutions);
		const likedSolutions = likedAcceptedSolutionsResponse?.likedSolutions
			.filter(s => promotedSolutions.every(ss => ss.submissionId !== s.submissionId));
		if(likedSolutions) {
			solutions = likedSolutions.concat(solutions);
		}
		const _solutions: _AcceptedSolution[]
			= solutions.map(
			s => ({
				...s,
				promoted: promotedSolutions.some(ss => ss.submissionId === s.submissionId)
			}));
		const solutionsDict = Object.assign({}, ..._solutions.map((x) => ({ [x.submissionId]: x })));
		const stateUpdates: State = {
			...this.state,
			promotedSolutions: promotedSolutions.map(s => s.submissionId),
			randomLikedSolutions: randomLikedSolutions.map(s => s.submissionId),
			newestSolutions: newestSolutions.map(s => s.submissionId),
			likedAcceptedSolutions: likedSolutions?.map(s => s.submissionId) ?? null,
			solutions: solutionsDict,
			loading: false
		};
		this.setState(stateUpdates);
	}

	processErrorAndClose = (error: any): void => {
		error.showToast();
		this.props.onClose();
	};

	render(): React.ReactNode {
		const { onClose, isInstructor } = this.props;
		const { loading, activeTab } = this.state;

		if(loading) {
			return null;
		}
		const maxModalWidth = window.innerWidth - 40;
		const modalWidth: undefined | number = maxModalWidth > 880 ? 880 : maxModalWidth; //TODO пока что это мок, в будущем width будет другой
		return (
			<Modal width={ modalWidth } onClose={ onClose }>
				<Modal.Header>
					{ texts.title }
				</Modal.Header>
				<Modal.Body>
					{ isInstructor &&
					<Tabs className={ styles.tabs } value={ activeTab } onValueChange={ s => this.handleTabChange(s) }>
						<Tabs.Tab id={ TabsType.instructorTab }>{ texts.instructorTabName }</Tabs.Tab>
						<Tabs.Tab id={ TabsType.studentTab }>{ texts.studentTabName }</Tabs.Tab>
					</Tabs>
					}
					{ activeTab === TabsType.instructorTab && this.renderInstructorTab() }
					{ activeTab === TabsType.studentTab && this.renderStudentTab() }
				</Modal.Body>
			</Modal>);
	}

	renderInstructorTab() {
		const { promotedSolutions, likedAcceptedSolutions, solutions } = this.state;

		return <div key={ TabsType.instructorTab }>
			<p>{ texts.instructorInstructions }</p>
			<div className={ styles.solutionsListWrapper }>
				<div className={ styles.solutionsList }>
					{ promotedSolutions.map(s => this.renderSolution(solutions[s])) }
					{ likedAcceptedSolutions!.map(s => this.renderSolution(solutions[s])) }
				</div>
			</div>
		</div>;
	}

	renderStudentTab() {
		const { promotedSolutions, randomLikedSolutions, newestSolutions, solutions } = this.state;
		const randomAndNewestSolutions = randomLikedSolutions.concat(newestSolutions);

		return <div key={ TabsType.studentTab }>
			{ promotedSolutions.length > 0 &&
			<>
				<h4>{ texts.promotedSolutionsHeader }</h4>
				<div className={ styles.solutionsListWrapper }>
					<div className={ styles.solutionsList }>
						{ promotedSolutions.map(s => this.renderSolution(solutions[s])) }
					</div>
				</div>
			</>
			}
			{ randomAndNewestSolutions.length > 0 &&
			<>
				{ promotedSolutions.length > 0 && <h4>{ texts.solutionsHeader }</h4> }
				<p>{ texts.studentInstructions }</p>
				<div className={ styles.solutionsListWrapper }>
					<div className={ styles.solutionsList }>
						{ randomAndNewestSolutions.map(s => this.renderSolution(solutions[s])) }
					</div>
				</div>
			</>
			}
		</div>;
	}

	renderSolution(solution: _AcceptedSolution) {
		const { isInstructor, } = this.props;
		const { activeTab, } = this.state;
		const asInstructor = isInstructor && activeTab === TabsType.instructorTab;
		return <div key={ solution.submissionId } className={ styles.solution }>
			<div className={ styles.controls }>
				{ asInstructor &&
				<Hint text={ solution.promoted ? texts.unpromoteHint : texts.promoteHint }>
					<Button
						className={ styles.button }
						use={ solution.promoted ? "primary" : "default" }
						onClick={ () => this.promote(solution.submissionId) }>
						<Star/>
					</Button>
				</Hint>
				}
				{ !solution.promoted &&
				<Hint text={ asInstructor && solution.likesCount !== null
					? texts.getDisabledLikesHint(solution.likesCount) : null }>
					<Button
						className={ styles.button }
						use={ solution.likedByMe && !asInstructor ? "primary" : "default" }
						disabled={ asInstructor }
						onClick={ () => this.like(solution.submissionId) }>
						<Heart/> { solution.likesCount }
					</Button>
				</Hint>
				}
			</div>
			<div className={ styles.codeCell }>
				{ asInstructor && solution.promoted && solution.promotedBy &&
				<div>{ texts.getPromotedByText(solution.promotedBy) }</div> }
				<StaticCode code={ solution.code } language={ solution.language }/>
			</div>
		</div>;
	}

	handleTabChange = (value: string): void => {
		this.fetchContentFromServer(() => this.setState({ activeTab: TabsType[value as keyof typeof TabsType] }));
	};

	like = (submissionId: number): void => {
		const isLike = !this.state.solutions[submissionId].likedByMe;
		const action = isLike
			? this.props.acceptedSolutionsApi.likeAcceptedSolution
			: this.props.acceptedSolutionsApi.dislikeAcceptedSolution;
		action(submissionId)
			.then(() => {
				const s = this.state.solutions[submissionId];
				const solutions = {
					...this.state.solutions,
					[submissionId]: { ...s, likedByMe: isLike, likesCount: s.likesCount! + (isLike ? 1 : -1) }
				};
				this.setState({ solutions });
			})
			.catch(error => error.showToast());
	};

	promote = (submissionId: number): void => {
		const isPromote = !this.state.solutions[submissionId].promoted;
		const action = isPromote
			? this.props.acceptedSolutionsApi.promoteAcceptedSolution
			: this.props.acceptedSolutionsApi.unpromoteAcceptedSolution;
		action(this.props.courseId, submissionId)
			.then(() => {
				const s = this.state.solutions[submissionId];
				const solutions: { [id: number]: _AcceptedSolution } = {
					...this.state.solutions,
					[submissionId]: {
						...s,
						promoted: isPromote,
						promotedBy: isPromote ? this.props.user : undefined
					}
				};
				this.setState({ solutions });
			})
			.catch(error => error.showToast());
	};
};

export { AcceptedSolutionsModal, AcceptedSolutionsProps };
