import React, { Component } from 'react';
import PropTypes from "prop-types";

import CourseCards from "./CourseCards/CourseCards";
import Guides from "../Guides/Guides";
import Gapped from "@skbkontur/react-ui/Gapped";
import Button from "@skbkontur/react-ui/Button";
import Loader from "@skbkontur/react-ui/Loader";
import Flashcards from "../Flashcards/Flashcards";

import styles from './coursePage.less';
import { guides } from '../consts';
import { rateTypes } from "../../../consts/rateTypes";

class CoursePage extends Component {
	constructor(props) {
		super(props);

		this.state = {
			showFlashcards: false,
		}
	}

	componentDidMount() {
		const { courseId, flashcards, loadFlashcards } = this.props;

		document.getElementsByTagName('main')[0].classList.add(styles.pageContainer);

		if (flashcards.length === 0) {
			loadFlashcards(courseId);
		}
	}

	componentWillUnmount() {
		document.getElementsByTagName('main')[0].classList.remove(styles.pageContainer);
	}

	render() {
		const { showFlashcards } = this.state;
		const { courseId, flashcardsLoading, flashcards, infoByUnits, sendFlashcardRate, statistics, totalFlashcardsCount } = this.props;

		return (
			<Loader active={ flashcardsLoading } type="big">
				<Gapped gap={ 15 } vertical={ true }>

					{ this.renderHeader() }

					{ infoByUnits &&
					<CourseCards
						infoByUnits={ infoByUnits }
						courseId={ courseId }
					/> }

					<Guides guides={ guides }/>

					{ showFlashcards &&
					<Flashcards
						totalFlashcardsCount={ totalFlashcardsCount }
						statistics={ statistics }
						flashcards={ flashcards }
						courseId={ courseId }
						onClose={ () => this.hideFlashcards() }
						sendFlashcardRate={ sendFlashcardRate }
					/> }
				</Gapped>
			</Loader>
		);
	}

	renderHeader() {
		return (
			<div className={ styles.header }>
				<div>
					<h2 className={ styles.title }>
						Флеш-карты для самопроверки
					</h2>
					<p className={ styles.description }>
						Помогут лучше запомнить материал курса и подготовиться к экзаменам
					</p>
				</div>
				<Button use="primary" size='large' onClick={ () => this.showFlashcards() }>
					Проверить себя
				</Button>
			</div>
		);
	}

	showFlashcards = () => {
		this.setState({
			showFlashcards: true,
		});
	};

	hideFlashcards = () => {
		this.setState({
			showFlashcards: false,
		});
	};
}

CoursePage.propTypes = {
	courseId: PropTypes.string,
	infoByUnits: PropTypes.arrayOf(PropTypes.shape({
		unitTitle: PropTypes.string,
		unlocked: PropTypes.bool,
		cardsCount: PropTypes.number,
		unitId: PropTypes.string,
		flashcardsSlideSlug: PropTypes.string,
	})),
	flashcards: PropTypes.arrayOf(PropTypes.shape({
		id: PropTypes.string,
		question: PropTypes.string,
		answer: PropTypes.string,
		unitTitle: PropTypes.string,
		rate: PropTypes.string,
		unitId: PropTypes.string,
		lastRateIndex: PropTypes.number,
	})),
	totalFlashcardsCount: PropTypes.number,
	statistics: PropTypes.shape({
		[rateTypes.notRated]: PropTypes.number,
		[rateTypes.rate1]: PropTypes.number,
		[rateTypes.rate2]: PropTypes.number,
		[rateTypes.rate3]: PropTypes.number,
		[rateTypes.rate4]: PropTypes.number,
		[rateTypes.rate5]: PropTypes.number,
	}),

	loadFlashcards: PropTypes.func,
	sendFlashcardRate: PropTypes.func,
};

export default CoursePage;
