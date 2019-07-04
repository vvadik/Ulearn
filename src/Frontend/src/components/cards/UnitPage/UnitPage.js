import React, {Component} from 'react'
import PropTypes from "prop-types";
import UnitCard from "./UnitCard/UnitCard";
import Guides from "../Guides/Guides";
import styles from './unitPage.less'
import Gapped from "@skbkontur/react-ui/Gapped";
import UnitProgressBar from "./UnitProgressBar/UnitProgressBar";

class UnitPage extends Component {
	render() {
		const {unitTitle, byScore, total} = this.props;
		const haveProgress = byScore.unseen !== total;

		return (
			<Gapped gap={8} vertical={true}>
				<h3 className={styles.title}>
					Вопросы для самопроверки
				</h3>
				<UnitCard haveProgress={haveProgress} total={total} unitTitle={unitTitle}/>
				{this.renderFooter(haveProgress)}
			</Gapped>
		);
	}

	renderFooter(haveProgress) {
		const {guides, byScore, total} = this.props;

		if (haveProgress) {
			return <UnitProgressBar byScore={byScore} total={total}/>;
		} else {
			return <Guides guides={guides}/>;
		}
	}
}

UnitPage.propTypes = {
	unitTitle: PropTypes.string.isRequired,
	guides: PropTypes.arrayOf(PropTypes.string).isRequired,
	total: PropTypes.number.isRequired,
	byScore: PropTypes.shape({
		unseen: PropTypes.number,
		1: PropTypes.number,
		2: PropTypes.number,
		3: PropTypes.number,
		4: PropTypes.number,
		5: PropTypes.number
	}).isRequired
};

export default UnitPage;
