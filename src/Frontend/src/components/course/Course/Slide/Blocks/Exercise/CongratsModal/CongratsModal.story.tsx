import React from "react";
import { CongratsModal, } from "./CongratsModal";

const props = {
	onClose: () => ({}),
	waitingForManualChecking: false,
	score: 5,
	showAcceptedSolutions: false,
};

export const FivePoints = (): React.ReactNode => <CongratsModal { ...props }/>;
export const FifteenPoints = (): React.ReactNode => <CongratsModal { ...props } score={ 50 }/>;
export const WaitingManual = (): React.ReactNode => <CongratsModal { ...props } waitingForManualChecking/>;
export const ShowAcceptedSolutions = (): React.ReactNode => <CongratsModal { ...props } showAcceptedSolutions/>;
export const ManualWithAcceptedSolutions = (): React.ReactNode => <CongratsModal { ...props } waitingForManualChecking showAcceptedSolutions/>;


export default {
	title: 'Exercise/CongratsModal',
	component: CongratsModal,
};
