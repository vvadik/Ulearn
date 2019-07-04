import React, {Component} from 'react'
import PropTypes from "prop-types";
import UnitCard from "./UnitCard/UnitCard";
import Guides from "../Guides/Guides";
import styles from './unitPage.less'
import Gapped from "@skbkontur/react-ui/Gapped";
import UnitProgressBar from "./UnitProgressBar/UnitProgressBar";

class UnitPage extends Component {
	render() {
		const {unitTitle, guides, byScore,total} = this.props;
		const haveProgress = byScore[0] !== total;
		return (
			<Gapped gap={8} vertical={true}>
				<h3 className={styles.title}>
					Вопросы для самопроверки
				</h3>
				<UnitCard haveProgress={haveProgress} total={total} unitTitle={unitTitle}/>
				{
					haveProgress
					? <UnitProgressBar byScore={byScore}/>
					: <Guides guides={guides}/>
				}
			</Gapped>
		);
	}
}

UnitPage.propTypes = {
	unitTitle : PropTypes.string.isRequired,
	guides: PropTypes.array.isRequired,
	total: PropTypes.number.isRequired,
	byScore: PropTypes.array.isRequired
};

export default UnitPage;