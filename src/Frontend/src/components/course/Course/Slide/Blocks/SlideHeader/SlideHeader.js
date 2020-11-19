import React from "react";

import { EyeClosed, } from "icons";
import PropTypes from "prop-types";

import texts from './SlideHeader.texts';

import styles from "../BlocksWrapper.less";


function SlideHeader({ score, isSkipped, isHiddenSlide, }) {
	if((score == null || score.maxScore === 0) && !isHiddenSlide) {
		return null;
	}

	if(isHiddenSlide) {
		return renderHiddenSlideHeader();
	}

	return renderScoreHeader();


	function renderHiddenSlideHeader() {
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

	function renderScoreHeader() {
		return (
			<div className={ styles.header }>
				<span className={ styles.headerText }>
					{ texts.getSlideScore(score, !isSkipped) }
				</span>
				{ isSkipped &&
				<span className={ styles.headerSkippedText }>{ texts.skippedHeaderText }</span>
				}
			</div>
		);
	}
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
		const { className, isBlock, isHidden, isContainer, score, withoutBottomPaddigns, withoutTopPaddings, showEyeHint, isHeaderOfHiddenSlide, } = this.props;
		const { showed, showStudentsModalOpened, } = this.state;
		const isHiddenBlock = isBlock && isHidden;
		const isHiddenSlide = !isBlock && isHidden;

		return (
			<React.Fragment>
				{ score && score.maxScore > 0 && this.renderScoreHeader() }
				{ showStudentsModalOpened && this.renderModal() }
				{ (isHiddenSlide || isHeaderOfHiddenSlide) && this.renderHiddenSlideHeader() }
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

SlideHeader.propTypes = {
	score: PropTypes.object,
	isSkipped: PropTypes.bool,
	isHiddenSlide: PropTypes.bool,
}


export default SlideHeader;
