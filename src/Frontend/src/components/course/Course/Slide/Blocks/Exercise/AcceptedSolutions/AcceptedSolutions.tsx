import React from "react";

import { Modal, } from "ui";
import DownloadedHtmlContent from "src/components/common/DownloadedHtmlContent.js";

import { constructPathToAcceptedSolutions } from "src/consts/routes";

import texts from "./AcceptedSolutions.texts";

interface AcceptedSolutionsProps {
	courseId: string,
	slideId: string,
	onClose: () => void
}

interface State {
	
}

class AcceptedSolutionsModal extends React.Component<AcceptedSolutionsProps, State> {
	render(): React.ReactNode {
		const { courseId, slideId, onClose, } = this.props;

		return (<DownloadedHtmlContent
			url={ constructPathToAcceptedSolutions(courseId, slideId) }
			injectInWrapperAfterContentReady={ (html: React.ReactNode) =>
				<Modal onClose={ onClose }>
					<Modal.Header>
						{ texts.title }
					</Modal.Header>
					<Modal.Body>
						{ texts.content }
						{ html }
					</Modal.Body>
				</Modal> }
		/>);
	}
}

export { AcceptedSolutionsModal, AcceptedSolutionsProps, };
