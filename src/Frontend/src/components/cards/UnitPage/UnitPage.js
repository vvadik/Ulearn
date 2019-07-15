import React, {Component} from 'react'
import PropTypes from "prop-types";
import UnitCard from "./UnitCard/UnitCard";
import Guides from "../Guides/Guides";
import styles from './unitPage.less'
import Gapped from "@skbkontur/react-ui/Gapped";
import ProgressBar from "../ProgressBar/ProgressBar";
import ShortQuestions from "./ShortQuestions/ShortQuestions";
import Flashcards from "../Flashcards/Flashcards";
import api from "../../../api";

class UnitPage extends Component {
	constructor(props) {
		super(props);
		const {statistics = {}, totalFlashcardsCount = 0} = this.props;
		const haveProgress = statistics.notRated !== totalFlashcardsCount;

		this.state = {
			haveProgress: haveProgress,
			isFlashcardsVisible: false
		}
	}

	get courseId() {
		return this.props.match.params.courseId.toLowerCase();
	}

	componentDidMount() {
		const courseId = this.courseId;

		api.cards
			.getFlashcards(courseId, 0)
			.then(r => this.flashcards = r.flashcards);
	}

	render() {
		const {unitTitle = 'unknown', totalFlashcardsCount = 0} = this.props;
		const {isFlashcardsVisible} = this.state;

		return (
			<Gapped gap={8} vertical={true}>
				<h3 className={styles.title}>
					Вопросы для самопроверки
				</h3>
				<UnitCard haveProgress={this.haveProgress}
						  totalFlashcardsCount={totalFlashcardsCount}
						  unitTitle={unitTitle}
						  handleStartButton={() => this.openFlashcards()}
				/>
				{this.renderFooter()}
				{isFlashcardsVisible &&
				<Flashcards
					onClose={() => this.closeFlashcards()}
					flashcards={this.flashcards}
				/>}
			</Gapped>
		);
	}

	renderFooter() {
		const {statistics = {}, totalFlashcardsCount = 0, guides = [], questionsWithAnswers = []} = this.props;

		if (this.haveProgress) {
			return (
				<div>
					<p className={styles.progressBarTitle}>
						Результаты последнего прохождения
					</p>
					<ProgressBar statistics={statistics} totalFlashcardsCount={totalFlashcardsCount}/>
					<ShortQuestions className={styles.shortQuestions} questionsWithAnswers={questionsWithAnswers}/>
				</div>
			);
		} else {
			return <Guides guides={guides}/>;
		}
	}

	loadStatistics() {
		api.cards
			.getUnitFlashcardsStat(this.courseId)
			.then(r => r.json())
			.then(r => {
				this.props.statistics = r.statistics;
				this.props.totalFlashcardsCount = r.totalFlashcardsCount;
			});
	}

	openFlashcards() {
		this.setState({
			isFlashcardsVisible: true
		});
	}

	closeFlashcards() {
		const courseId = this.courseId;

		api.cards
			.getUnitFlashcardsStat(courseId)
			.then(r => r.json())
			.then(r => {
				this.props.statistics = r.statistics;
				this.props.totalFlashcardsCount = r.totalFlashcardsCount;
				this.setState({
					isFlashcardsVisible: false
				});
			})
	}

	mapFlashcardsToQuestionWithAnswers() {
		return this.flashcards.map
	}
}

UnitPage.propTypes = {
	unitTitle: PropTypes.string.isRequired,
	guides: PropTypes.arrayOf(PropTypes.string).isRequired,
	questionsWithAnswers: PropTypes.arrayOf(PropTypes.shape({
		question: PropTypes.string,
		answer: PropTypes.string
	})).isRequired,
	totalFlashcardsCount: PropTypes.number.isRequired,
	statistics: PropTypes.shape({
		notRated: PropTypes.number,
		rate1: PropTypes.number,
		rate2: PropTypes.number,
		rate3: PropTypes.number,
		rate4: PropTypes.number,
		rate5: PropTypes.number
	}).isRequired,
	match: PropTypes.object
};

export default UnitPage;
