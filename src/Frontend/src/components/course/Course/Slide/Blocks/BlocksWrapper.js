import React from "react";

import { Button, Hint, Link, Modal, Gapped, Checkbox, } from "ui";
import { EyeOpened, EyeClosed } from "icons";

import classNames from "classnames";

import PropTypes from "prop-types";

import styles from "./BlocksWrapper.less";
import getPluralForm from "src/utils/getPluralForm";

const hiddenHintText = "Студенты не видят этот блок";
const hiddenSlideText = "Студенты не видят этот слайд.";
const hiddenSlideTextWithStudents = (showedGroupsIds) => `Студенты ${ showedGroupsIds.join(', ') } видят этот слайд.`;
const getSlideScore = ({ score, maxScore }) => `${ score } ${ getPluralForm(score, 'балл', 'балла', 'баллов') } из ${ maxScore }`;

class BlocksWrapper extends React.Component {
	constructor(props) {
		super(props);

		this.state = {
			showed: false,
			showStudentsModalOpened: false,
			groups: [], // TODO (rozentor) not added in current release
		};
	}

	render() {
		const { children, className, isBlock, isHidden, isContainer, score, withoutBottomPaddigns, withoutTopPaddings, showEyeHint, } = this.props;
		const { showed, showStudentsModalOpened, } = this.state;
		const isHiddenBlock = isBlock && isHidden;
		const isHiddenSlide = !isBlock && isHidden;
		const wrapperClassNames = classNames(
			styles.wrapper,
			styles.withPaddings,
			{ [styles.withoutTopPaddings]: withoutTopPaddings },
			{ [styles.withoutBottomPaddigns]: withoutBottomPaddigns },
			{ [styles.hiddenBackgroundColor]: isHiddenBlock },
			{ [styles.hiddenSlide]: isHiddenSlide },
			{ [styles.showed]: showed },
			className
		);

		return (
			<React.Fragment>
				{ score && score.maxScore > 0 && this.renderScoreHeader() }
				{ isContainer
					? children
					: <React.Fragment>
						{ showStudentsModalOpened && this.renderModal() }
						{ isHiddenSlide && this.renderHiddenSlideHeader() }
						<div className={ wrapperClassNames } ref={ (ref) => this.wrapper = ref }>
							{ showEyeHint && isHiddenBlock && this.renderEyeHint() }
							{ children }
						</div>
					</React.Fragment>
				}
			</React.Fragment>);
	}

	renderEyeHint = () => {
		const wrapperClass = classNames(styles.eyeClosedWrapper);

		return (
			<div className={ wrapperClass }>
				<Hint pos={ "top right" } text={ hiddenHintText }>
					<EyeClosed/>
				</Hint>
			</div>
		);
	}

	renderScoreHeader = () => {
		const { score, } = this.props;

		return (
			<div className={ styles.header }>
				<span className={ styles.headerText }>
					{ getSlideScore(score) }
				</span>
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
}


BlocksWrapper.propTypes = {
	className: PropTypes.string,
	isBlock: PropTypes.bool,
	withoutBottomPaddigns: PropTypes.bool,
	withoutTopPaddings: PropTypes.bool,
	isHidden: PropTypes.bool,
	isContainer: PropTypes.bool,
	score: PropTypes.object,
	showEyeHint: PropTypes.bool,
}

export default BlocksWrapper;
