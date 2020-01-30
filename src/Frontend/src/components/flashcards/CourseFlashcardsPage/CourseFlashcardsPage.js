import React, { Component } from 'react';
import PropTypes from "prop-types";

import CourseCards from "./CourseCards/CourseCards";
import Guides from "../Guides/Guides";
import Button from "ui/Button";
import Loader from "ui/Loader";
import Flashcards from "../Flashcards/Flashcards";

import styles from './courseFlashcardsPage.less';
import { guides } from '../consts';

class CourseFlashcardsPage extends Component {
	constructor(props) {
		super(props);

		this.state = {
			showFlashcards: false,
		}
	}

	componentDidMount() {
		const { courseId, flashcards, loadFlashcards } = this.props;

		if (flashcards.length === 0) {
			loadFlashcards(courseId);
		}
	}

	render() {
		const { showFlashcards } = this.state;
		const { courseId, flashcardsLoading, flashcards, infoByUnits, sendFlashcardRate } = this.props;

		return (
			<Loader active={ flashcardsLoading } type="big">
				{ this.renderHeader() }

				{ !flashcardsLoading &&
				<CourseCards
					infoByUnits={ infoByUnits }
					courseId={ courseId }
				/> }
				{ !flashcardsLoading &&
				<Guides guides={ guides }/>
				}

				{ showFlashcards &&
				<Flashcards
					infoByUnits={ infoByUnits }
					flashcards={ flashcards }
					courseId={ courseId }
					onClose={ this.hideFlashcards }
					sendFlashcardRate={ sendFlashcardRate }
				/> }
			</Loader>
		);
	}

	renderHeader() {
		const hasUnlockedUnit = this.props.infoByUnits
			.some(unit => unit.unlocked);

		return (
			<header className={ styles.header }>
				<div>
					<h2 className={ styles.title }>
						Флеш-карты для самопроверки
					</h2>
					<p className={ styles.description }>
						Помогут лучше запомнить материал курса и подготовиться к экзаменам
					</p>
				</div>
				<Button disabled={ !hasUnlockedUnit }
						use="primary"
						size='large'
						onClick={ this.showFlashcards }>
					Проверить себя
				</Button>
			</header>
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

CourseFlashcardsPage.propTypes = {
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
		theorySlides: PropTypes.arrayOf(
			PropTypes.shape({
				slug: PropTypes.string,
				title: PropTypes.string,
			}),
		),
	})),

	loadFlashcards: PropTypes.func,
	sendFlashcardRate: PropTypes.func,
};

export default CourseFlashcardsPage;
