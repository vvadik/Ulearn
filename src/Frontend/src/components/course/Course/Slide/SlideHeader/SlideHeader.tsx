import React from "react";

import { SlideType } from "src/models/slide";
import { ScoreHeader } from "./ScoreHeader";

import texts from './SlideHeader.texts';

import { EyeClosed, } from "@skbkontur/react-icons"
import styles from "../Blocks/BlocksWrapper.less";

interface SlideHeaderProps {
	courseId: string,
	slideId: string,
	isHiddenSlide: boolean,
	slideType: SlideType,
}

const SlideHeader: React.FC<SlideHeaderProps> = ({ courseId, slideId, isHiddenSlide, slideType }) => {
	if(isHiddenSlide) {
		return <HiddenSlideHeader/>
	}
	if(slideType === SlideType.Exercise || slideType === SlideType.Quiz) {
		return <ScoreHeader courseId={ courseId } slideId={ slideId }/>;
	}
	return null;
}

const HiddenSlideHeader: React.FC = () => {
	return (
		<div className={ styles.hiddenHeader }>
			<span className={ styles.hiddenHeaderText }>
				<span className={ styles.hiddenSlideEye }>
					<EyeClosed/>
				</span>
				{ texts.hiddenSlideText }
			</span>
		</div>
	);
}

/* Everything to make component to be a class with ability to show slide to groups TODO not included in current release
	constructor(props) {
		super(props);

		this.state = {
			showed: false,
			showStudentsModalOpened: false,
			groups: [], //
		};
	}

	render = () => {
		const { className, isBlock, isHidden, score, withoutBottomPaddings, withoutTopPaddings, } = this.props;
		const { showed, showStudentsModalOpened, } = this.state;
		const isHiddenBlock = isBlock && isHidden;
		const isHiddenSlide = !isBlock && isHidden;

		return (
			<React.Fragment>
				{ score && score.maxScore > 0 && this.renderScoreHeader() }
				{ showStudentsModalOpened && this.renderModal() }
				{ isHiddenSlide && this.renderHiddenSlideHeader() }
			</React.Fragment>);
	}

	renderScoreHeader = () => {
		const { score, isSkipped, } = this.props;

		return (
			<div className={ styles.header }>
				<span className={ styles.headerText }>
					{ getSlideScore(score, !isSkipped) }
				</span>
				{ isSkipped &&
				<span className={ styles.headerSkippedText }>{ skippedHeaderText }</span>
				}
			</div>
		);
	}

	renderHiddenSlideHeader = () => {
		const { showed, groups } = this.state;
		const headerClassNames = classNames(
			styles.hiddenHeader,
			{ [styles.showed]: this.state.showed }
		);

		const showedGroupsIds = groups.filter(({ checked }) => checked).map(({ id }) => id);
		const text = showedGroupsIds.length === 0
			? hiddenSlideText
			: hiddenSlideTextWithStudents(showedGroupsIds);

		return (
			<div className={ headerClassNames }>
				<span className={ styles.hiddenHeaderText }>
					<span className={ styles.hiddenSlideEye }>
						{
							showed
								? <EyeOpened/>
								: <EyeClosed/>
						}
					</span>
					{ text }
					{ groups.length > 0 &&
					<Link onClick={ this.openModal }>
						{ showed ? "Скрыть" : "Показать" }
					</Link> }
				</span>
			</div>
		);
	}

	renderModal() {
		const { groups } = this.state;

		return (
			<Modal width={ 395 } onClose={ this.closeModal }>
				<Modal.Header>Кому?</Modal.Header>
				<Modal.Body>
					<Gapped gap={ 20 } vertical>
						{ groups.map(({ id, checked }) =>
							<Checkbox
								key={ id }
								checked={ checked }
								onValueChange={ () => this.handleGroupClick(id) }>
								{ id }
							</Checkbox>) }
					</Gapped>
				</Modal.Body>
				<Modal.Footer panel={ true }>
					<Gapped gap={ 10 }>
						<Button use={ "primary" } onClick={ this.show }>Показать</Button>
						<Button onClick={ this.closeModal }>Отменить</Button>
					</Gapped>
				</Modal.Footer>
			</Modal>
		);
	}

	handleGroupClick = (id) => {
		const { groups } = this.state;
		const newGroups = JSON.parse(JSON.stringify(groups));
		const group = newGroups.find(g => g.id === id);
		group.checked = !group.checked;
		this.setState({ groups: newGroups });
	}

	show = () => {
		const showed = this.anyGroupsChecked();
		this.setState({ showStudentsModalOpened: false, showed, });
	}

	anyGroupsChecked = () => {
		const { groups } = this.state;
		return groups.some(({ checked }) => checked);
	}

	openModal = () => {
		this.setState({ showStudentsModalOpened: true });
	}

	closeModal = () => {
		this.setState({ showStudentsModalOpened: false });
	}
 */


export default SlideHeader;
