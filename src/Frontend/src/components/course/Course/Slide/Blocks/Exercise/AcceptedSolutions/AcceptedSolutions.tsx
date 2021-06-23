import React from "react";
import { Modal, Tabs } from "@skbkontur/react-ui";

import texts from "./AcceptedSolutions.texts";
import { getAcceptedSolutions, getLikedAcceptedSolutions } from "src/api/acceptedSolutions";
import {
	AcceptedSolutionsResponse,
	LikedAcceptedSolutionsResponse
} from "src/models/acceptedSolutions";

interface AcceptedSolutionsProps {
	courseId: string,
	slideId: string,
	userId: string,
	isInstructor: boolean,
	onClose: () => void
}

enum TabsType {
	instructorTab = 'instructorTab',
	studentTab = 'studentTab',
}

interface State {
	loading: boolean,
	error: string | null,
	acceptedSolutionsResponse: AcceptedSolutionsResponse | null,
	likedAcceptedSolutionsResponse: LikedAcceptedSolutionsResponse | null,
	activeTab: TabsType;
}

const LikedAcceptedSolutionsCount = 30;

class AcceptedSolutionsModal extends React.Component<AcceptedSolutionsProps, State> {
	constructor(props: AcceptedSolutionsProps) {
		super(props);
		this.state = {
			loading: true,
			error: null,
			acceptedSolutionsResponse: null,
			likedAcceptedSolutionsResponse: null,
			activeTab: props.isInstructor ? TabsType.instructorTab : TabsType.studentTab
		};
	}

	componentDidMount() {
		this.fetchContentFromServer();
	}

	fetchContentFromServer() {
		const { courseId, slideId, isInstructor, } = this.props;
		const getAcceptedSolutionsPromise = getAcceptedSolutions(courseId, slideId);
		if(!isInstructor) {
			getAcceptedSolutionsPromise
				.then(acceptedSolutionsResponse => this.setState({ acceptedSolutionsResponse, loading: false }))
				.catch(this.processError.bind(this));
		} else {
			const getLikedAcceptedSolutionsPromise = getLikedAcceptedSolutions(courseId, slideId, 0,
				LikedAcceptedSolutionsCount);
			Promise.all([getAcceptedSolutionsPromise, getLikedAcceptedSolutionsPromise])
				.then(result => {
					const [acceptedSolutionsResponse, likedAcceptedSolutionsResponse] = result;
					this.setState({ acceptedSolutionsResponse, likedAcceptedSolutionsResponse, loading: false });
				})
				.catch(this.processError.bind(this));
		}
	}

	processError(error: any): void {
		this.setState({ loading: false, error: `Ошибка с кодом ${ error.status }` });
	}

	handleTabChange(value: string): void {
		this.setState({ activeTab: TabsType[value as keyof typeof TabsType] });
	};

	render(): React.ReactNode {
		const { onClose, isInstructor } = this.props;
		const { error, loading, activeTab } = this.state;

		if(loading) {
			return null;
		}
		return (
			<Modal onClose={ onClose }>
				<Modal.Header>
					{ texts.title }
				</Modal.Header>
				<Modal.Body>
					{ error }
					{ !error && isInstructor &&
					<Tabs value={ activeTab } onValueChange={ this.handleTabChange }>
						<Tabs.Tab id={ TabsType.instructorTab }>{texts.instructorTabName}</Tabs.Tab>
						<Tabs.Tab id={ TabsType.studentTab }>{texts.studentTabName}</Tabs.Tab>
					</Tabs>
					}
					{ !error && activeTab === TabsType.instructorTab && this.renderInstructorTab() }
					{ !error && activeTab === TabsType.studentTab && this.renderStudentTab() }
				</Modal.Body>
			</Modal>);
	}

	renderInstructorTab() {
		return <div key={ TabsType.instructorTab }>
			Инструктор
		</div>;
	}

	renderStudentTab() {
		return <div key={ TabsType.studentTab }>
			Студент
		</div>;
	}
}

export { AcceptedSolutionsModal, AcceptedSolutionsProps, };
