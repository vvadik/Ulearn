import React, {Component} from 'react'
import PropTypes from "prop-types";
import CourseCards from "./CourseCards/CourseCards";
import Guides from "../Guides/Guides";
import styles from './coursePage.less'
import Gapped from "@skbkontur/react-ui/Gapped";
import Button from "@skbkontur/react-ui/Button";
import api from "../../../api";
import cardsByUnitExample from "./CourseCards/cardsByUnitExample";
import Flashcards from "../Flashcards/Flashcards";

class CoursePage extends Component {
	constructor(props) {
		super(props);
		const {guides} = this.props;

		this.state = {
			guides: guides,
			flashcardsInfos: [],
			isFlashcardsVisible: false
		}
	}

	get courseId() {
		return this.props.match.params.courseId.toLowerCase();
	}

	componentDidMount() {
		const courseId = this.courseId;

		//TODO(rozentor)  await for backend implementation and uncomment lines with parsing
		api.cards.getCourseFlashcardsStat(courseId)
		//.then(r => r.json())
		//	.then(r => this.setState({flashcardsInfos: r.flashcardsInfos}));
			.then(_ => this.setState({flashcardsInfos: cardsByUnitExample}));
		api.cards
			.getFlashcards(courseId)
			.then(r => {
				this.flashcards = r.flashcards;
			});
	}

	render() {
		const {flashcardsInfos, guides, isFlashcardsVisible} = this.state;

		return (
			<Gapped gap={15} vertical={true}>
				<div className={styles.textContainer}>
					<h2 className={styles.title}>
						Флеш-карты для самопроверки
					</h2>
					<p className={styles.description}>
						Помогут лучше запомнить материал курса и подготовиться к экзаменам
					</p>
					<div className={styles.launchAllButtonContainer}>
						<Button use="primary" size='large' onClick={() => this.setFlashcardsVisibility(true)}>
							Проверить себя
						</Button>
					</div>
				</div>
				<CourseCards flashcardsInfos={flashcardsInfos} courseId={this.courseId}/>
				<Guides guides={guides}/>
				{isFlashcardsVisible &&
				<Flashcards
					flashcards={this.flashcards}
					match={this.props.match}
					onClose={() => this.setFlashcardsVisibility(false)}
				/>}
			</Gapped>
		);
	}

	setFlashcardsVisibility = (isVisible) => {
		this.setState({
			isFlashcardsVisible: isVisible
		});
	}
}

CoursePage.propTypes = {
	flashcardsInfos: PropTypes.arrayOf(PropTypes.shape({
		unitTitle: PropTypes.string,
		unlocked: PropTypes.bool,
		cardsCount: PropTypes.number,
		unitId: PropTypes.string
	})),
	guides: PropTypes.arrayOf(PropTypes.string).isRequired,
	match: PropTypes.object
};

export default CoursePage;
